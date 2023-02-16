using System;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimationCurveGenerator.CurveTypes
{
    internal class TrigonometricCurveType : ICurveType
    {
        public string DisplayName => "Trigonometric";
        private TrigonometricFunction trigonometricFunction;

        public void OnGUI()
        {
            trigonometricFunction = (TrigonometricFunction)EditorGUILayout.EnumPopup("Trigonometric function", trigonometricFunction);
        }

        public float Evaluate(float value)
        {
            return trigonometricFunction switch
            {
                TrigonometricFunction.Sin => Mathf.Sin(value),
                TrigonometricFunction.Cos => Mathf.Cos(value),
                TrigonometricFunction.Tan => Mathf.Tan(value),
                TrigonometricFunction.Asin => Mathf.Asin(value),
                TrigonometricFunction.Acos => Mathf.Acos(value),
                TrigonometricFunction.Atan => Mathf.Atan(value),
                _ => throw new ArgumentException($"Invalid type: {trigonometricFunction}")
            };
        }

        public bool IsValidValue(float value) => true;

        private enum TrigonometricFunction
        {
            Sin,
            Cos,
            Tan,
            Asin,
            Acos,
            Atan,
        }
    }
}