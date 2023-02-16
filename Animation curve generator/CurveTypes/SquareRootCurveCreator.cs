using UnityEngine;

namespace EditorTools.AnimationCurveGenerator.CurveTypes
{
    internal class SquareRootCurveType : ICurveType
    {
        public string DisplayName => "Square root";
        public void OnGUI()
        {
        }

        public float Evaluate(float value) => Mathf.Sqrt(value);
        public bool IsValidValue(float value) => value >= 0 || Mathf.Approximately(value, 0f);
    }
}