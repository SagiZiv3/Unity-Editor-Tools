using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.SerializedReferenceInitializer.Editor
{
    internal static class ObjectUtils
    {
        /// <summary>
        /// Finds all instances of a specific type in the current stage.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>An <see cref="IEnumerable{ObjectInfo}"/> of <see cref="ObjectInfo"/> representing all instances found.</returns>
        public static IEnumerable<ObjectInfo> FindAllInstancesInCurrentStage(Type type)
        {
            // `Object.FindObjectOfType` only searches in the root scene and doesn't support the prefabs preview scene.
            // Unfortunately, the function we need to call is generic so we have to use reflection to call it...
            var currentStage = StageUtility.GetCurrentStage();
            // Source: https://stackoverflow.com/a/4667999/9977758
            // Get the generic type definition
            MethodInfo method = currentStage.GetType().GetMethod(nameof(currentStage.FindComponentsOfType),
                BindingFlags.Public | BindingFlags.Instance);

            method = method.MakeGenericMethod(type);
            Object[] objects = (Object[])method.Invoke(currentStage, null);
            return objects
                .Select(obj => new ObjectInfo
                (
                    obj,
                    GetFullComponentPath((Component)obj)
                ));
        }

        /// <summary>
        /// Finds all instances of the specified type found in the hierarchy of a given GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to search in.</param>
        /// <param name="type">The type of components to find.</param>
        /// <returns>An <see cref="IEnumerable{ObjectInfo}"/> of <see cref="ObjectInfo"/> representing all instances found.</returns>
        public static IEnumerable<ObjectInfo> FindAllInstancesOnGameObject(GameObject gameObject, Type type)
        {
            Component[] objects = gameObject.GetComponentsInChildren(type);
            return objects
                .Select(obj => new ObjectInfo
                (
                    obj,
                    GetFullComponentPath(obj)
                ));
        }

        /// <summary>
        /// Finds all instances of the specified type in the project's Asset Database.
        /// </summary>
        /// <param name="type">The type of object to search for.</param>
        /// <returns>An enumerable collection of <see cref="IEnumerable{ObjectInfo}"/> objects representing the found instances.</returns>
        public static IEnumerable<ObjectInfo> FindAllInstancesInProjectFolder(Type type)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(assetPath => new ObjectInfo
                (
                    AssetDatabase.LoadAssetAtPath(assetPath, type),
                    assetPath
                ));
        }

        /// <summary>
        /// Get the full path of a component by traversing up the hierarchy of the transform it's attached to.
        /// </summary>
        /// <param name="o">The component to get the full path for.</param>
        /// <returns>The full path of the component.</returns>
        private static string GetFullComponentPath(Component o)
        {
            if (o == null)
                return string.Empty;
            Transform transform = o.transform;
            var objects = new Stack<string>();
            do
            {
                objects.Push(transform.name);
                transform = transform.parent;
            } while (transform != null);

            return string.Join("/", objects);
        }
    }

    /// <summary>
    /// A struct containing information about a Unity object instance and its asset path.
    /// </summary>
    internal readonly struct ObjectInfo
    {
        public readonly Object obj;
        public readonly string path;

        public ObjectInfo(Object obj, string path)
        {
            this.obj = obj;
            this.path = path;
        }
    }
}