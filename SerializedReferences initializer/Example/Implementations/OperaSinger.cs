using EditorTools.SerializedReferenceInitializer.Attributes;

namespace EditorTools.SerializedReferenceInitializer.Example.Implementations
{
    [CustomMenuPath("Singing related/Opera")]
    internal sealed class OperaSinger : ISing
    {
        public void Sing()
        {
            throw new System.NotImplementedException();
        }

        public void StopSinging()
        {
            throw new System.NotImplementedException();
        }
    }
}