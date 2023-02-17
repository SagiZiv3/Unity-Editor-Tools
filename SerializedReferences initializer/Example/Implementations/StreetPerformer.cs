using EditorTools.SerializedReferenceInitializer.Attributes;
using UnityEngine;

namespace EditorTools.SerializedReferenceInitializer.Example.Implementations
{
    [GenerateWrapperFor(typeof(ISing))]
    [AddComponentMenu("Singing related/")]
    internal sealed class StreetPerformer : MonoBehaviour, ISing
    {
        [SerializeField] private GameObject street;
        [SerializeField] private AnimationCurve type;

        public void Sing()
        {
            Debug.Log($"Singing on {street.name}");
        }

        public void StopSinging()
        {
            Debug.Log($"Stop singing on {street.name}");
        }
    }
}