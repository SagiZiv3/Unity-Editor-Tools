using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimationCurveGenerator.CurveTypes
{
    internal class PolynomialCurveType : ICurveType
    {
        public string DisplayName => "Polynomial";
        private float coefficient, power, constant;

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Equation: ");
            coefficient = EditorGUILayout.FloatField(coefficient, GUILayout.MaxWidth(25f));
            EditorGUILayout.LabelField(" * x ^ ", GUILayout.MaxWidth(30f));
            power = EditorGUILayout.FloatField(power, GUILayout.MaxWidth(25f));
            EditorGUILayout.LabelField(" + ", GUILayout.MaxWidth(15f));
            constant = EditorGUILayout.FloatField(constant, GUILayout.MaxWidth(25f));
            GUILayout.EndHorizontal();
        }

        public float Evaluate(float value) => coefficient * Mathf.Pow(value, power) + constant;
        public bool IsValidValue(float value) => true;
    }
}