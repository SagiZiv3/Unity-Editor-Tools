using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTools.SerializedReferenceInitializer.Editor
{
    // Icons names: https://github.com/halak/unity-editor-icons
    internal static class EditorIcons
    {
        public static Texture2D Script { get; } = GetIconByName("cs Script Icon");

        public static Texture2D AddNew { get; } = GetIconByName("CreateAddNew@2x");
        
        public static Texture2D Delete { get; } = GetIconByName("CollabDeleted Icon");

        public static Texture2D GameObject { get; } = GetIconByName("d_GameObject Icon");

        public static Texture2D ScriptableObject { get; } = GetIconByName("ScriptableObject On Icon");

        /// <summary>
        /// Retrieves the thumbnail icon for a given Unity Object.
        /// </summary>
        /// <param name="obj">The Unity Object to retrieve the icon for.</param>
        /// <returns>A Texture2D object containing the icon for the Unity Object.</returns>
        public static Texture2D GetIconForObject(Object obj)
        {
            return AssetPreview.GetMiniThumbnail(obj);
        }

        /// <summary>
        /// Gets the texture icon with the specified name from the editor's Unity builtin resources.
        /// </summary>
        /// <param name="name">The name of the desired icon.</param>
        /// <returns>The texture icon as a Texture2D object.</returns>
        private static Texture2D GetIconByName(string name)
        {
            return (Texture2D)EditorGUIUtility.IconContent(name).image;
        }
    }
}