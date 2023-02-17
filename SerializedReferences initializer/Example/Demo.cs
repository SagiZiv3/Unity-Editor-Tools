using EditorTools.SerializedReferenceInitializer.Attributes;
using UnityEngine;

namespace EditorTools.SerializedReferenceInitializer.Example
{
    internal sealed class Demo : MonoBehaviour
    {
        [SerializeReference, ShowInitializationMenu]
        private ISing type;

        [ContextMenu("Sing")]
        private void Sing() => type?.Sing();
        [ContextMenu("Stop singing")]
        private void StopSinging() => type?.StopSinging();
    }
}