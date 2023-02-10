using UnityEngine;

namespace EditorTools.AnimatorParameter.Example
{
    internal sealed class Demo : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)]
        private string animatorTrigger;
        [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Bool)]
        private int animatorBool;
        [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Int)]
        private int animatorInt;

        [ContextMenu("Print values")]
        private void PrintValues()
        {
            Debug.Log($"Trigger: {animatorTrigger}");
            Debug.Log($"Bool: {animatorBool}");
        }
    }
}