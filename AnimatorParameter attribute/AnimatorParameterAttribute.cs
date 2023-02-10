using System;
using UnityEngine;

namespace EditorTools.AnimatorParameter
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AnimatorParameterAttribute : PropertyAttribute
    {
        public string AnimatorName { get; }
        public AnimatorControllerParameterType ParameterType { get; }

        public AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType parameterType)
        {
            AnimatorName = animatorName;
            ParameterType = parameterType;
        }
    }
}