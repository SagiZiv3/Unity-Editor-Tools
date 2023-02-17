using System;
using UnityEngine;

namespace EditorTools.SerializedReferenceInitializer.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ShowInitializationMenuAttribute : PropertyAttribute
    {
    }
}