using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimatorParameter.Editor
{
    internal static class ErrorMessages
    {
        public static string UnsupportedPropertyType(SerializedPropertyType propertyType) =>
            $"Unsupported field type: {propertyType}.";

        public static string FieldNotFound(string fieldName) =>
            $"Unable to find field named \"{fieldName}\".";

        public static string AnimatorNotAssigned(string animatorName) =>
            $"Animator \"{animatorName}\" is not assigned.";

        public static string AnimatorDoesNotReferenceController(string animatorName) =>
            $"Animator \"{animatorName}\" doesn't reference any controller.";

        public static string UnableToFindParameterOfType(string animatorName,
            AnimatorControllerParameterType parameterType) =>
            $"Animator \"{animatorName}\" doesn't have {parameterType} parameters.";
    }
}