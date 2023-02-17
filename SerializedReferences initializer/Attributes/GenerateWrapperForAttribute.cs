using System;

namespace EditorTools.SerializedReferenceInitializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class GenerateWrapperForAttribute : Attribute
    {
        /// <summary>
        /// The interface type that you want to create a wrapper for.
        /// </summary>
        public Type BaseType { get; }

        public GenerateWrapperForAttribute(Type baseType)
        {
            if (!baseType.IsInterface)
                throw new ArgumentException("Base type must be an interface", nameof(baseType));
            BaseType = baseType;
        }
    }
}