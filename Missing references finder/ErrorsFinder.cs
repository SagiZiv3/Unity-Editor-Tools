using System.Reflection;
using UnityEngine;

namespace EditorTools.MissingReferencesFinder
{
    internal abstract class ErrorsFinder
    {
        /// <summary>
        /// Can this <see cref="ErrorsFinder"/> instance find errors for the given field.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns>True if this <see cref="ErrorsFinder"/> can find errors, false otherwise.</returns>
        public abstract bool CanValidate(FieldInfo fieldInfo);
        /// <summary>
        /// Finds all the errors related to this field info on the given script as a string.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public abstract string Find(Object script, FieldInfo fieldInfo);
    }
}