using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorTools.AnimationCurveGenerator
{
    internal sealed class AnimationCurvesBuilder : EditorWindow
    {
        private const BindingFlags FieldBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private MonoBehaviour component;
        private SerializedObject serializedObject;
        private ICurveType curveType;
        private IReadOnlyDictionary<string, ICurveType> availableCurveCreators;
        private string variableName;
        private string[] curveCreatorNames;
        private float stepSize = 0.05f, minValue = -1f, maxValue = 1f;
        private int precision = 5;

        [MenuItem("Utilities/Animation Curve Builder")]
        private static void ShowWindow()
        {
            var window = GetWindow<AnimationCurvesBuilder>("Animation Curve Builder");
            window.Show();
            if (Selection.activeTransform != null)
                window.component = Selection.activeTransform.GetComponent<MonoBehaviour>();
        }

        private void OnEnable()
        {
            // Find all the curve creators implementations.
            availableCurveCreators = GetAllCurveBuilders();
            curveType = availableCurveCreators.Values.First();
            curveCreatorNames = availableCurveCreators.Keys.ToArray();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            component = (MonoBehaviour)EditorGUILayout.ObjectField("Script instance:", component, typeof(MonoBehaviour),
                true);
            if (EditorGUI.EndChangeCheck())
            {
                variableName = string.Empty;
            }
            if (component == null)
            {
                EditorGUILayout.HelpBox("Please select a component!", MessageType.Info);
                variableName = string.Empty;
                return;
            }

            string[] fields = GetAnimationCurveFieldsNames(component);
            if (fields.Length < 1)
            {
                EditorGUILayout.HelpBox("Component doesn't have any serialized AnimationCurves!", MessageType.Error);
                variableName = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(variableName))
                variableName = fields[0];
            int selectedIndex = EditorGUILayout.Popup("Field name", Array.IndexOf(fields, variableName), fields);
            variableName = fields[selectedIndex];

            // Show all the existing implementations.
            int curveCreatorIndex = EditorGUILayout.Popup(
                "Curve type:",
                Array.IndexOf(curveCreatorNames, curveType.DisplayName),
                curveCreatorNames
            );
            curveType = availableCurveCreators[curveCreatorNames[curveCreatorIndex]];

            stepSize = EditorGUILayout.FloatField("Step size", stepSize);
            GUILayout.BeginHorizontal();
            minValue = Mathf.Min(EditorGUILayout.FloatField("Min value", minValue), maxValue);
            maxValue = Mathf.Max(EditorGUILayout.FloatField("Max value", maxValue), minValue);
            GUILayout.EndHorizontal();
            precision = EditorGUILayout.IntSlider(
                new GUIContent("Precision", "How many digits after the decimal point"), precision, 3, 10);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.BeginHorizontal("BOX");
            curveType.OnGUI();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate curve"))
            {
                var curve = BuildCurve();
                serializedObject = new SerializedObject(component);
                serializedObject.FindProperty(variableName).animationCurveValue = curve;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(component);
            }
        }

        private AnimationCurve BuildCurve()
        {
            var animationCurve = new AnimationCurve();
            for (float x = minValue; x < maxValue || Mathf.Approximately(x, maxValue); x += stepSize)
            {
                // We must round the x value, due to precisions mistakes.
                // Without rounding, the loop might stop on 0.9 instead of 1 because it exceeded 1.
                x = (float)Math.Round(x, precision);
                if (!curveType.IsValidValue(x)) continue;
                float value = curveType.Evaluate(x);
                animationCurve.AddKey(x, value);
            }

            return animationCurve;
        }

        private static IReadOnlyDictionary<string, ICurveType> GetAllCurveBuilders()
        {
            return TypeCache.GetTypesDerivedFrom<ICurveType>()
                .Where(type => !type.IsInterface && !type.IsAbstract)
                .Select(type => (ICurveType)Activator.CreateInstance(type))
                .ToDictionary(curveCreator => curveCreator.DisplayName);
        }

        private static string[] GetAnimationCurveFieldsNames(object o)
        {
            var fields = o.GetType().GetFields(FieldBindingFlags);
            return fields
                // Make sure it is an animation curve
                .Where(field => field.FieldType.IsAssignableFrom(typeof(AnimationCurve)))
                // And that it is serializable
                .Where(field => !field.IsDefined(typeof(NonSerializedAttribute)))
                .Where(field => field.IsPublic || field.IsDefined(typeof(SerializeField)))
                .Select(field => field.Name)
                .OrderBy(s => s.ToLowerInvariant())
                .ToArray();
        }
    }
}