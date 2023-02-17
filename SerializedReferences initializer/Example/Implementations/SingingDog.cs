using EditorTools.SerializedReferenceInitializer.Attributes;
using UnityEngine;

namespace EditorTools.SerializedReferenceInitializer.Example.Implementations
{
    [CreateAssetMenu(menuName = "SerializedReference initializer/Demo/Singing Dog")]
    [GenerateWrapperFor(typeof(ISing))]
    internal sealed class SingingDog : ScriptableObject, ISing
    {
        [SerializeField] private int age;

        public void Sing()
        {
            Debug.Log("Barkkk");
        }

        public void StopSinging()
        {
            Debug.Log("...");
        }
    }
}