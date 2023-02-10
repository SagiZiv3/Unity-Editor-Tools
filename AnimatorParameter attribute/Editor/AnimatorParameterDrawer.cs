using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EditorTools.AnimatorParameter.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public sealed class AnimatorParameterDrawer : PropertyDrawer
    {
        private static readonly ISet<SerializedPropertyType> SupportedTypes =
            new HashSet<SerializedPropertyType> { SerializedPropertyType.String, SerializedPropertyType.Integer };

        private const BindingFlags AnimatorBindingFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!SupportedTypes.Contains(property.propertyType))
            {
                UpdatePropertyToInvalidValue(property);
                EditorGUI.LabelField(position, label.text,
                    ErrorMessages.UnsupportedPropertyType(property.propertyType));
                return;
            }

            var animatorParameterAttribute = (AnimatorParameterAttribute)attribute;

            var targetObject = property.serializedObject.targetObject;
            // Find the animator field.
            var field = targetObject.GetType().GetField(animatorParameterAttribute.AnimatorName, AnimatorBindingFlags);
            if (field == null)
            {
                UpdatePropertyToInvalidValue(property);
                EditorGUI.LabelField(position, label.text,
                    ErrorMessages.FieldNotFound(animatorParameterAttribute.AnimatorName));
                return;
            }

            var animator = field.GetValue(targetObject) as Animator;
            if (animator == null)
            {
                UpdatePropertyToInvalidValue(property);
                EditorGUI.LabelField(position, label.text,
                    ErrorMessages.AnimatorNotAssigned(animatorParameterAttribute.AnimatorName));
                return;
            }

            var animatorController = (AnimatorController)animator.runtimeAnimatorController;
            if (animatorController == null)
            {
                UpdatePropertyToInvalidValue(property);
                EditorGUI.LabelField(position, label.text,
                    ErrorMessages.AnimatorDoesNotReferenceController(animatorParameterAttribute.AnimatorName));
                return;
            }

            var parameters = animatorController.parameters;

            // Create a dictionary that maps from the parameter's hash-code to its name.
            IReadOnlyDictionary<int, string> parametersNamesByHashCode = parameters
                .Where(parameter => animatorParameterAttribute.ParameterType == parameter.type)
                .ToDictionary(p => Animator.StringToHash(p.name), p => p.name);

            string[] parametersNames = parametersNamesByHashCode.Values.ToArray();

            if (parametersNames.Length == 0)
            {
                UpdatePropertyToInvalidValue(property);
                EditorGUI.LabelField(position, label.text,
                    ErrorMessages.UnableToFindParameterOfType(animatorParameterAttribute.AnimatorName,
                        animatorParameterAttribute.ParameterType));
                return;
            }

            int selection = GetSelectionIndex(property, parametersNamesByHashCode, parametersNames);
            // If the variables are not initialized yet, assign them the first parameter.
            if (selection < 0)
            {
                UpdatePropertyToNewSelection(property, parametersNames[0], parametersNamesByHashCode);
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            selection = EditorGUI.Popup(position, selection, parametersNames);
            if (EditorGUI.EndChangeCheck())
            {
                UpdatePropertyToNewSelection(property, parametersNames[selection], parametersNamesByHashCode);
            }

            EditorGUI.EndProperty();
        }

        private static int GetSelectionIndex(SerializedProperty property,
            IReadOnlyDictionary<int, string> parametersNamesByHashCode, string[] parametersNames)
        {
            string selectionName = property.propertyType switch
            {
                SerializedPropertyType.Integer => GetParameterNameFromHashCode(property.intValue,
                    parametersNamesByHashCode),
                SerializedPropertyType.String => property.stringValue
            };
            int index = Array.IndexOf(parametersNames, selectionName);
            return index;
        }

        private static string GetParameterNameFromHashCode(int hashCode,
            IReadOnlyDictionary<int, string> parametersNamesByHashCode)
        {
            return parametersNamesByHashCode.TryGetValue(hashCode, out string parameterName)
                ? parameterName
                : string.Empty;
        }

        private static void UpdatePropertyToNewSelection(SerializedProperty property, string selectionName,
            IReadOnlyDictionary<int, string> parametersNamesByHashCode)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = selectionName;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = parametersNamesByHashCode.First(x => x.Value == selectionName).Key;
                    break;
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static void UpdatePropertyToInvalidValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = null;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}