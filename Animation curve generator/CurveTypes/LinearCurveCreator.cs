using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimationCurveGenerator.CurveTypes
{
    internal class LinearCurveType : ICurveType
    {
        private float m, b;
        public string DisplayName => "Linear";

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            m = EditorGUILayout.FloatField("Slope", m);
            b = EditorGUILayout.FloatField("Constant", b);
            GUILayout.EndHorizontal();
        }

        public float Evaluate(float value) => m * value + b;
        public bool IsValidValue(float value) => true;
    }
}