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

    public sealed class InteractionController : MonoBehaviour
    {
        public float radius = 1.15f;
        public ProofObjectiveState objective;
        public AssetProofController proofController;

        private InteractionTarget current;

        private void Update()
        {
            current = FindNearest();
            objective.SetHint(current != null ? current.prompt : "");

            if (current != null && Input.GetKeyDown(KeyCode.E))
            {
                Interact(current);
            }
        }

        private InteractionTarget FindNearest()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, radius);
            InteractionTarget best = null;
            var bestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                var target = hit.GetComponent<InteractionTarget>();
                if (target == null || !IsAvailable(target))
                {
                    continue;
                }

                var distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < bestDistance)
                {
                    best = target;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private bool IsAvailable(InteractionTarget target)
        {
            switch (target.kind)
            {
                case InteractionKind.Cleanup:
                    return !objective.Cleared;
                case InteractionKind.Sample:
                    return objective.Cleared && !objective.Sampled;
                case InteractionKind.Repair:
                    return objective.Sampled && !objective.Repaired;
                case InteractionKind.Display:
                    return objective.Repaired && !objective.Displayed;
                default:
                    return false;
            }
        }

        private void Interact(InteractionTarget target)
        {
            switch (target.kind)
            {
                case InteractionKind.Cleanup:
                    target.cleanup.Clear();
                    objective.MarkCleared();
                    break;
                case InteractionKind.Sample:
                    proofController.SelectArtifactFromInteraction(target.artifact);
                    target.artifact.transform.SetParent(transform, true);
                    target.artifact.transform.localPosition = new Vector3(0.32f, 0.25f, 0f);
                    objective.MarkSampled(target.artifact);
                    break;
                case InteractionKind.Repair:
                    proofController.SelectArtifactFromInteraction(objective.targetArtifact);
                    proofController.RepairSelected();
                    objective.MarkRepaired();
                    break;
                case InteractionKind.Display:
                    proofController.SelectArtifactFromInteraction(objective.targetArtifact);
                    proofController.DisplaySelected();
                    objective.MarkDisplayed();
                    break;
            }
        }
    }
}
