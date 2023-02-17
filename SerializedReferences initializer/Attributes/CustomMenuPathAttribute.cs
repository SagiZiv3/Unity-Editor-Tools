using System;

namespace EditorTools.SerializedReferenceInitializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomMenuPathAttribute : Attribute
    {
        public readonly string path;
        public readonly int order;

        public CustomMenuPathAttribute(string path, int order = 0)
        {
            this.path = path;
            this.order = order;
        }
    }
}