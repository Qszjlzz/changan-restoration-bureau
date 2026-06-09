using UnityEngine;

namespace ChanganRestorationBureau
{
    public enum ProofSlotType
    {
        ShelfSlot,
        WorkbenchSlot,
        DisplayCaseSlot,
        InventorySlot,
        DetailPanelSlot
    }

    public sealed class ProofSlot : MonoBehaviour
    {
        public ProofSlotType slotType;
        public string slotId;
        public Vector2 sizeUnits = Vector2.one;
        public RestorationArtifact Occupant { get; private set; }

        public bool CanAccept(RestorationArtifact artifact)
        {
            if (artifact == null || Occupant != null)
            {
                return false;
            }

            return slotType == ProofSlotType.DisplayCaseSlot || slotType == ProofSlotType.WorkbenchSlot;
        }

        public bool TryPlace(RestorationArtifact artifact)
        {
            if (!CanAccept(artifact))
            {
                return false;
            }

            Occupant = artifact;
            artifact.transform.position = transform.position;
            artifact.transform.SetParent(transform, true);
            artifact.MarkDisplayed(slotType == ProofSlotType.DisplayCaseSlot);
            Debug.Log($"[Changan] Slot placed artifact={artifact.artifactId} slot={slotId} type={slotType}");
            return true;
        }
    }
}
