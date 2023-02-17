using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.SerializedReferenceInitializer.Editor
{
    internal abstract class ObjectsCreator
    {
        protected Type Type { get; }

        protected ObjectsCreator(Type type)
        {
            Type = type;
        }

        public abstract object Create();
    }

    /// <summary>
    /// Creates an instance by invoking the empty constructor.
    /// </summary>
    internal sealed class SimpleClassCreator : ObjectsCreator
    {
        public SimpleClassCreator(Type type) : base(type)
        {
        }

        public override object Create()
        {
            return Activator.CreateInstance(Type);
        }
    }

    /// <summary>
    /// Creates an instance of the wrapper type as well as an instance of the Unity object that it wraps.
    /// The reference to the Unity object instance is passed to the wrapper class instance.
    /// </summary>
    internal sealed class UnityObjectCreator : ObjectsCreator
    {
        private readonly IUnityObjectRetriever unityObjectRetriever;

        public UnityObjectCreator(Type wrapperClassType, IUnityObjectRetriever unityObjectRetriever) : base(
            wrapperClassType)
        {
            this.unityObjectRetriever = unityObjectRetriever;
        }

        public override object Create()
        {
            object instance = unityObjectRetriever.GetInstance();
            return CreateWrapperClassInstance(instance, Type);
        }

        internal static object CreateWrapperClassInstance(object wrappedClassInstance, Type wrapperClassType)
        {
            var cons = wrapperClassType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            object wrapper = cons[0].Invoke(new[] { wrappedClassInstance });
            return wrapper;
        }
    }

    /// <summary>
    /// An interface that returns a reference to an instance of a UnityEngine.Object.
    /// </summary>
    internal interface IUnityObjectRetriever
    {
        Object GetInstance();
    }

    /// <summary>
    /// Returns the reference to an already existing instance of a UnityEngine.Object. 
    /// </summary>
    internal sealed class ObjectGetter : IUnityObjectRetriever
    {
        private readonly Object reference;

        public ObjectGetter(Object reference)
        {
            this.reference = reference;
        }

        Object IUnityObjectRetriever.GetInstance() => reference;
    }

    /// <summary>
    /// Create a new MonoBehaviour instance on the given object and returns the reference to it.
    /// </summary>
    internal sealed class MonoBehaviourCreator : IUnityObjectRetriever
    {
        private readonly Type wrappedClassType;
        private readonly Component obj;

        public MonoBehaviourCreator(Type wrappedClassType, Component obj)
        {
            this.wrappedClassType = wrappedClassType;
            this.obj = obj;
        }

        Object IUnityObjectRetriever.GetInstance()
        {
            Component component = Undo.AddComponent(obj.gameObject, wrappedClassType);
            return component;
        }
    }

    /// <summary>
    /// Creates a new instance in the currently active directory in the Project-view window, and returns a reference to it.
    /// </summary>
    internal sealed class ScriptableObjectCreator : IUnityObjectRetriever
    {
        private readonly Object referenceObject;
        private readonly Type wrappedClassType;

        public ScriptableObjectCreator(Type wrappedClassType, Object referenceObject)
        {
            this.wrappedClassType = wrappedClassType;
            this.referenceObject = referenceObject;
        }

        Object IUnityObjectRetriever.GetInstance()
        {
            // This method is internal, so we have to use reflection to access it...
            var getCurrentFolderMethod = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);
            string currentFolder = getCurrentFolderMethod?.Invoke(null, null)?.ToString() ?? "Assets";
            string assetPath = $"{currentFolder}/New {ObjectNames.NicifyVariableName(wrappedClassType.Name)}.asset";
            ScriptableObject instance = ScriptableObject.CreateInstance(wrappedClassType);

            // Could use `ProjectWindowUtil.CreateAsset` instead, but I want to re-select the object after the renaming.
            CreateNewAsset createNewAsset = ScriptableObject.CreateInstance<CreateNewAsset>();
            createNewAsset.SetObject(referenceObject);
            // Start the asset creation process and open the Rename dialog in the Project window.
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(instance.GetInstanceID(), createNewAsset, assetPath,
                EditorIcons.GetIconForObject(instance), null);
            return instance;
        }

        private sealed class CreateNewAsset : EndNameEditAction
        {
            private Object obj;

            public void SetObject(Object o) => obj = o;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object asset = EditorUtility.InstanceIDToObject(instanceId);
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();
                Selection.activeObject = obj;
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Object asset = EditorUtility.InstanceIDToObject(instanceId);
                // Delete the unsaved instance.
                DestroyImmediate(asset);
                // Undo the modification to the field so we don't save a reference to a destroyed object.
                Undo.PerformUndo();
            }
        }
    }
}