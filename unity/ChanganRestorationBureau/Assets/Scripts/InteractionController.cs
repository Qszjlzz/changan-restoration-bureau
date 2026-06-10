using UnityEngine;

namespace ChanganRestorationBureau
{
    public sealed class InteractionController : MonoBehaviour
    {
        public float radius = 1.15f;
        public ProofObjectiveState objective;
        public AssetProofController proofController;
        public ProofDialogueController dialogue;
        public ProofRestorationChoiceController restorationChoice;
        public ProofResourceHudController resourceHud;
        public ProofDayState dayState;

        private InteractionTarget current;

        private void Update()
        {
            if (dayState == null && proofController != null)
            {
                dayState = proofController.GetComponent<ProofDayState>();
            }

            if (dialogue != null && dialogue.IsOpen)
            {
                if (objective != null)
                {
                    objective.SetHint(dialogue.FooterHint);
                }

                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    dialogue.Advance();
                }

                return;
            }

            if (restorationChoice != null && restorationChoice.IsOpen)
            {
                if (objective != null)
                {
                    objective.SetHint(restorationChoice.FooterHint);
                }

                restorationChoice.HandleRuntimeInput();
                return;
            }

            current = FindNearest();
            if (objective != null)
            {
                if (current != null)
                {
                    if (current.kind == InteractionKind.Talk && current.npc != null)
                    {
                        current.prompt = current.npc.BuildPrompt(dayState, objective);
                    }
                    else if (current.kind == InteractionKind.Repair)
                    {
                        current.prompt = dayState != null && dayState.WorkHours > 0
                            ? "Press E to choose a restoration path (-1 hour and material)"
                            : "Press E to inspect restoration costs. No work hours remain.";
                    }
                    else if (current.kind == InteractionKind.Display)
                    {
                        current.prompt = "Press E to place the conserved tile on display";
                    }
                }

                objective.SetHint(current != null ? current.prompt : "");
            }

            if (current != null && Input.GetKeyDown(KeyCode.E))
            {
                Interact(current);
            }
        }

        public bool TryInteractNearest()
        {
            current = FindNearest();
            if (current == null)
            {
                Debug.LogWarning("[Changan] TryInteractNearest found no valid target.");
                return false;
            }

            Interact(current);
            return true;
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
            if (objective == null)
            {
                return false;
            }

            switch (target.kind)
            {
                case InteractionKind.Cleanup:
                    return !objective.Cleared;
                case InteractionKind.Sample:
                    return objective.Cleared && !objective.Sampled;
                case InteractionKind.Repair:
                    return objective.Sampled && !objective.Repaired;
                case InteractionKind.Display:
                    return objective.Repaired
                        && !objective.Displayed
                        && (dayState == null || dayState.SelectedRestorationBranch != RestorationBranch.QuickReuse);
                case InteractionKind.Talk:
                    return target.npc != null && target.npc.CanInteract(dayState, objective);
                default:
                    return false;
            }
        }

        private void Interact(InteractionTarget target)
        {
            Debug.Log($"[Changan] Interact kind={target.kind} target={target.name}");
            switch (target.kind)
            {
                case InteractionKind.Cleanup:
                    target.cleanup.Clear();
                    objective.MarkCleared();
                    resourceHud?.ShowActionFeedback("Overgrowth cleared. The buried tile is visible.");
                    break;
                case InteractionKind.Sample:
                    proofController.SelectArtifactFromInteraction(target.artifact);
                    target.artifact.transform.SetParent(transform, true);
                    target.artifact.transform.localPosition = new Vector3(0.32f, 0.25f, 0f);
                    objective.MarkSampled(target.artifact);
                    resourceHud?.ShowActionFeedback("Lotus tile sampled. Return to the bureau bench.");
                    break;
                case InteractionKind.Repair:
                    proofController.SelectArtifactFromInteraction(objective.targetArtifact);
                    proofController.RequestRepairChoice();
                    break;
                case InteractionKind.Display:
                    proofController.SelectArtifactFromInteraction(objective.targetArtifact);
                    proofController.DisplaySelected();
                    objective.MarkDisplayed();
                    resourceHud?.ShowActionFeedback("Conserved tile placed in the display case.");
                    break;
                case InteractionKind.Talk:
                    target.npc.BeginInteraction(dayState, objective, dialogue);
                    break;
            }
        }
    }
}
