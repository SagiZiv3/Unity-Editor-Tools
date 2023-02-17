using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorTools.SerializedReferenceInitializer.Editor
{
    internal sealed class MenuItemsGenerator
    {
        private readonly Dictionary<string, AdvancedDropdownItem> menuItemsByCategory;

        public MenuItemsGenerator()
        {
            menuItemsByCategory = new Dictionary<string, AdvancedDropdownItem>();
        }

        /// <summary>
        /// Creates and returns a menu-item that represents the parent item for the specified path.
        /// If an item for the path already exists, returns the existing item.
        /// If any of the parent items along the path do not exist, they will be created as well.
        /// This function uses the given `root` item as the starting point for creating the parent menu-items.
        /// </summary>
        /// <param name="path">The path of the item to create, where each component of the path is separated by a forward slash ('/').</param>
        /// <param name="root">The root item for the dropdown menu.</param>
        /// <returns>The parent item for the specified path.</returns>
        public AdvancedDropdownItem CreateParents(string path, AdvancedDropdownItem root)
        {
            // Check if we already have an item for this path.
            if (menuItemsByCategory.TryGetValue(path, out AdvancedDropdownItem parent))
                return parent;

            AdvancedDropdownItem currentParent = root;
            string[] pathComponents = path.Split('/');
            for (int componentIndex = 0; componentIndex < pathComponents.Length; componentIndex++)
            {
                string parentPath = string.Join("/", pathComponents.Take(componentIndex + 1));
                if (!menuItemsByCategory.TryGetValue(parentPath, out AdvancedDropdownItem item))
                {
                    item = new AdvancedDropdownItem(pathComponents[componentIndex]);
                    currentParent.AddChild(item);
                    menuItemsByCategory[parentPath] = item;
                }

                currentParent = item;
            }

            return currentParent;
        }

        /// <summary>
        /// Creates a menu-item that will instantiate a plain-old class instance of the given type when selected.
        /// </summary>
        /// <param name="displayName">The text to show on the menu-item.</param>
        /// <param name="classType">The type to instantiate when this menu-item is selected.</param>
        /// <returns></returns>
        public static AdvancedDropdownItem CreateMenuItemForClass(string displayName, Type classType)
        {
            var item = new ObjectCreationMenuItem(displayName, new SimpleClassCreator(classType))
            {
                icon = EditorIcons.Script
            };
            return item;
        }

        /// <summary>
        /// Creates a dropdown menu item for a given unity-object type.
        /// </summary>
        /// <param name="displayName">The name to display for this object type in the dropdown menu.</param>
        /// <param name="wrapperType">The type of object to wrap the object instances in.</param>
        /// <param name="wrappedType">The type of object to find instances of.</param>
        /// <param name="referenceObject">A reference object to search for instances of the given type.</param>
        /// <returns>A dropdown menu item with options to create new instances and select existing instances of the given type.</returns>
        public static AdvancedDropdownItem CreateMenuItemForObject(string displayName, Type wrapperType,
            Type wrappedType, Component referenceObject)
        {
            var root = new AdvancedDropdownItem(displayName);
            IUnityObjectRetriever createNewInstanceObjectRetriever;
            IEnumerable<ObjectInfo> objectInfos;
            if (wrappedType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                root.icon = EditorIcons.GameObject;
                createNewInstanceObjectRetriever = new MonoBehaviourCreator(wrappedType, referenceObject);
                // If we directly modify the prefab asset, show only instances on this prefab's hierarchy.
                // Otherwise, if the object is in a scene/stage, find all the instance in this scene/stage.
                objectInfos = EditorUtility.IsPersistent(referenceObject)
                    ? ObjectUtils.FindAllInstancesOnGameObject(referenceObject.gameObject, wrappedType)
                    : ObjectUtils.FindAllInstancesInCurrentStage(wrappedType);
            }
            else
            {
                root.icon = EditorIcons.ScriptableObject;
                createNewInstanceObjectRetriever = new ScriptableObjectCreator(wrappedType, referenceObject);
                objectInfos = ObjectUtils.FindAllInstancesInProjectFolder(wrappedType);
            }

            root.AddChild(new ObjectCreationMenuItem($"New {displayName}",
                new UnityObjectCreator(wrapperType, createNewInstanceObjectRetriever)
            ) { icon = EditorIcons.AddNew });
            root.AddSeparator();
            foreach (ObjectInfo objectInfo in objectInfos.OrderBy(info => info.path))
            {
                var creationMenuItem = new ObjectCreationMenuItem(
                    objectInfo.path,
                    new UnityObjectCreator(wrapperType, new ObjectGetter(objectInfo.obj))
                )
                {
                    icon = EditorIcons.GetIconForObject(objectInfo.obj),
                };
                root.AddChild(creationMenuItem);
            }

            return root;
        }
    }

    internal sealed class ObjectCreationMenuItem : AdvancedDropdownItem
    {
        private readonly ObjectsCreator objectsCreator;

        public ObjectCreationMenuItem(string name, ObjectsCreator objectsCreator) : base(name)
        {
            this.objectsCreator = objectsCreator;
        }

        public object CreateInstance() => objectsCreator.Create();
    }
}