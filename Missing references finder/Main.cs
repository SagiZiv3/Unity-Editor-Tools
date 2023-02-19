using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTools.MissingReferencesFinder
{
    internal static class Main
    {
        private static readonly IList<string> DefaultNamespaceToIgnore = new List<string>
        {
            "TMPro",
            "UnityEngine",
            "UnityEngine.UI",
            "UnityEngine.EventSystems"
        };

        private static Window _openWindow;
        private static IList<string> _namespacesToIgnore;

        private static readonly IList<ErrorsFinder> ErrorsFinders = new List<ErrorsFinder>
        {
            new UnityEventErrorsFinder(),
            new MissingOrNullObjectErrorFinder()
        };

        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                                  System.Reflection.BindingFlags.DeclaredOnly |
                                                  System.Reflection.BindingFlags.Public |
                                                  System.Reflection.BindingFlags.NonPublic;

        #region Menu items

        [MenuItem("Utilities/Open missing references window &#o")]
        private static void ShowWindow()
        {
            if (!_openWindow)
            {
                string nameSpacesToIgnore =
                    EditorPrefs.GetString("EditorTools.MissingReferencesFinder.NamespaceToIgnore");
                if (string.IsNullOrEmpty(nameSpacesToIgnore))
                    _namespacesToIgnore = new List<string>(DefaultNamespaceToIgnore);
                else
                    _namespacesToIgnore = new List<string>(nameSpacesToIgnore.Split(";"));
                _openWindow = EditorWindow.GetWindow<Window>("Find missing references");
                _openWindow.findProblems = FindProblems;
                _openWindow.Initialize(_namespacesToIgnore);
            }

            _openWindow.Focus();
        }

        [MenuItem("Utilities/Find missing references &#f")]
        private static void ShowWindowAndFind()
        {
            ShowWindow();
            _openWindow.Initialize(FindProblems(false));
        }

        #endregion

        private static IList<ErrorInfo> FindProblems(bool includePrefabs)
        {
            IList<ErrorInfo> errors = new List<ErrorInfo>();
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var gameObject in gameObjects)
            {
                if (!includePrefabs && PrefabUtility.IsPartOfPrefabAsset(gameObject)) continue;
                // FindMissingReferencesInGameObject(errors, gameObject);
                var components = gameObject.GetComponents<Component>();
                foreach (var component in components)
                {
                    // If the component doesn't exist, the script for this component was deleted.
                    if (!component)
                    {
                        errors.Add(ErrorInfo.BuildDeletedScriptErrorObject(gameObject));
                        continue;
                    }

                    if (_namespacesToIgnore.Contains(component.GetType().Namespace)) continue;
                    var fieldValues = GetAllFields(component, BindingFlags);
                    var errorBuilder = new StringBuilder();
                    foreach (var fieldInfo in fieldValues)
                    {
                        errorBuilder.Clear();
                        foreach (var errorsFinder in ErrorsFinders)
                        {
                            if (!errorsFinder.CanValidate(fieldInfo)) continue;
                            string error = errorsFinder.Find(component, fieldInfo);
                            if (string.IsNullOrEmpty(error)) continue;
                            errorBuilder.AppendLine(error);
                        }

                        if (errorBuilder.Length > 0)
                        {
                            errors.Add(ErrorInfo.BuildErrorObject(component, fieldInfo, errorBuilder.ToString()));
                        }
                    }
                }
            }

            return errors;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Component behaviour, BindingFlags bindingFlags)
        {
            var fieldValues = new List<FieldInfo>();
            var type = behaviour.GetType();
            while (type != null)
            {
                if (_namespacesToIgnore.Contains(type.Namespace))
                {
                    break;
                }

                fieldValues.AddRange(
                    type.GetFields(bindingFlags)
                        .Where(fieldInfo =>
                            fieldInfo.IsPublic || Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                );
                type = type.BaseType;
            }

            return fieldValues;
        }
    }
}