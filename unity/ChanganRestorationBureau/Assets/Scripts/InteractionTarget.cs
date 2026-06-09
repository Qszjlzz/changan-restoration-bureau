using UnityEngine;

namespace ChanganRestorationBureau
{
    public enum InteractionKind
    {
        Cleanup,
        Sample,
        Repair,
        Display
    }

    public sealed class InteractionTarget : MonoBehaviour
    {
        public InteractionKind kind;
        public string prompt;
        public RestorationArtifact artifact;
        public MapCleanupInteractable cleanup;
        public ProofSlot slot;
    }
}
