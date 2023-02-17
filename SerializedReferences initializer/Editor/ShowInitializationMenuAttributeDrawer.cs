﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EditorTools.SerializedReferenceInitializer.Attributes;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.SerializedReferenceInitializer.Editor
{
    [CustomPropertyDrawer(typeof(ShowInitializationMenuAttribute))]
    internal sealed class ShowInitializationMenuAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type propertyType = GetPropertyType();
            if (!IsSupportedType(propertyType) || propertyType.IsSubclassOf(typeof(Object)))
            {
                EditorGUI.LabelField(position, label.text,
                    $"Invalid type: {propertyType.Name}");
                return;
            }

            if (!fieldInfo.IsDefined(typeof(SerializeReference)))
            {
                EditorGUI.LabelField(position, label.text,
                    $"{property.displayName} is not marked as SerializeReference");
                return;
            }

            Type implementationType = GetImplementationType(property);

            string dropdownTitle = GetImplementationTypeName(implementationType);
            Rect dropdownPosition = new Rect(position);
            dropdownPosition.width -= EditorGUIUtility.labelWidth;
            dropdownPosition.x += EditorGUIUtility.labelWidth;
            dropdownPosition.height = EditorGUIUtility.singleLineHeight;
            if (EditorGUI.DropdownButton(dropdownPosition,
                    new GUIContent(ObjectNames.NicifyVariableName(dropdownTitle)), FocusType.Keyboard))
            {
                var menu = ShowMenu(property, propertyType,
                    $"Select {propertyType.Name} implementation");
                menu.Show(dropdownPosition);
            }

            DropAreaGUI(position, propertyType, property);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        private static bool IsSupportedType(Type propertyType)
        {
            return propertyType.IsClass || propertyType.IsInterface;
        }

        private Type GetPropertyType()
        {
            Type fieldType = fieldInfo.FieldType;
            if (fieldType.IsArray)
                return fieldType.GetElementType();
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];
            return fieldType;
        }

        private static Type GetImplementationType(SerializedProperty property)
        {
            string typePath = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(typePath))
                return null;
            int splitIndex = typePath.IndexOf(' ');
            var assembly = Assembly.Load(typePath.Substring(0, splitIndex));
            return assembly.GetType(typePath.Substring(splitIndex + 1));
        }

        private static string GetImplementationTypeName(Type implementationType)
        {
            if (implementationType == null)
                return "uninitialized";
            var autoGenerated = implementationType.GetCustomAttribute<AutoGeneratedWrapperAttribute>();

            return autoGenerated == null
                ? implementationType.Name
                : autoGenerated.WrappedType.Name;
        }

        private static void DropAreaGUI(Rect dropArea, Type propertyType, SerializedProperty property)
        {
            // Source: https://gist.github.com/bzgeb/3800350#file-triggercontainereditor-cs-L24
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    // Make sure the dragged object if of the correct type.
                    Object draggedObject = DragAndDrop.objectReferences[0];
                    Component component = null;

                    // Make sure that the dragged object is a sub-type of the property type,
                    // or a GameObject that has a component of this type.
                    if (!propertyType.IsInstanceOfType(draggedObject) &&
                        !(draggedObject is GameObject go && go.TryGetComponent(propertyType, out component)))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    Object scriptInstance = component == null ? draggedObject : component;
                    // Find the wrapper class for this object
                    Type wrapperClassType = TypeCache.GetTypesWithAttribute<AutoGeneratedWrapperAttribute>()
                        .FirstOrDefault(wrappedType => wrappedType
                            .GetCustomAttribute<AutoGeneratedWrapperAttribute>()
                            .WrappedType == scriptInstance.GetType());
                    if (wrapperClassType == null)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        UpdateProperty(property,
                            UnityObjectCreator.CreateWrapperClassInstance(scriptInstance, wrapperClassType)
                        );
                    }

                    evt.Use();
                    break;
            }
        }

        private static AdvancedDropdown ShowMenu(SerializedProperty serializedProperty, Type propertyType,
            string title)
        {
            // Find all the non-abstract implementations of `propertyType`
            // and exclude the classes derived from UnityEngine.Object (they will have a wrapper-class that references them).
            IEnumerable<Type> types = TypeCache.GetTypesDerivedFrom(propertyType)
                .Where(type => !type.IsAbstract && !typeof(Object).IsAssignableFrom(type))
                // Make sure to only show types that we can instantiate.
                .Where(type =>
                    type.GetConstructor(Type.EmptyTypes) != null ||
                    type.IsDefined(typeof(AutoGeneratedWrapperAttribute))
                );

            // If the property's type can also be instantiated, append it to the types enumerable.
            if (!propertyType.IsAbstract && !propertyType.IsInterface &&
                propertyType.GetConstructor(Type.EmptyTypes) != null)
            {
                types = types.Append(propertyType);
            }

            var menu = new DropDownMenu(new AdvancedDropdownState(), types, title,
                serializedProperty.serializedObject.targetObject as Component,
                (o) => UpdateProperty(serializedProperty, o));
            return menu;
        }

        private static void UpdateProperty(SerializedProperty serializedProperty, object instance)
        {
            serializedProperty.managedReferenceValue = instance;
            serializedProperty.serializedObject.ApplyModifiedProperties();
            serializedProperty.serializedObject.Update();
        }
    }
}