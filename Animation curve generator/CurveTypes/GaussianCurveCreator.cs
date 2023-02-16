using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimationCurveGenerator.CurveTypes
{
    internal sealed class GaussianCurveType : ICurveType
    {
        private float sigma = 0;

        public string DisplayName => "Gaussian";

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            sigma = EditorGUILayout.FloatField("Sigma", sigma);
            GUILayout.EndVertical();
        }

        public float Evaluate(float value) => Gaussian(value, sigma);
        public bool IsValidValue(float value) => true;

        private static float Gaussian(float value, float sigma = 1f)
        {
            return Mathf.Exp(-Mathf.Pow(sigma * value, 2) / 2);
        }
    }
}