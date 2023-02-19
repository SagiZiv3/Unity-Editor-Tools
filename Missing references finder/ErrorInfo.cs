using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorTools.MissingReferencesFinder
{
    internal sealed class ErrorInfo
    {
        public Object referencedObject;
        public Texture2D icon;
        public string path;
        public string message;
        public bool display;

        /// <summary>
        /// Create an <see cref="ErrorInfo"/> object for a component whose field has a deleted/unassigned reference.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ErrorInfo BuildErrorObject(Component script, FieldInfo fieldInfo, string errorMessage)
        {
            return new ErrorInfo
            {
                referencedObject = script,
                path = $"{script.name}/{script.GetType()}/{fieldInfo.Name}",
                message = errorMessage,
                icon = AssetPreview.GetMiniThumbnail(script.gameObject)
            };
        }

        /// <summary>
        /// Create an <see cref="ErrorInfo"/> object for a game object that references a deleted script.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static ErrorInfo BuildDeletedScriptErrorObject(GameObject gameObject)
        {
            var pathBuilder = new StringBuilder(gameObject.name);
            var transform = gameObject.transform.parent;
            while (transform)
            {
                pathBuilder.Insert(0, $"{transform.name}/");
                transform = transform.parent;
            }

            return new ErrorInfo
            {
                referencedObject = gameObject,
                path = pathBuilder.ToString(),
                message = "Script was deleted!",
                icon = AssetPreview.GetMiniThumbnail(gameObject)
            };
        }
    }
}