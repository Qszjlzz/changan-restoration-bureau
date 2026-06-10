using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofObjectiveState : MonoBehaviour
    {
        public Text objectiveText;
        public Text hintText;
        public RestorationArtifact targetArtifact;
        public ProofDayState dayState;

        public bool Cleared { get; private set; }
        public bool Sampled { get; private set; }
        public bool Repaired { get; private set; }
        public bool Displayed { get; private set; }

        private void Start()
        {
            Debug.Log($"[Changan] ProofObjectiveState.Start target={(targetArtifact != null ? targetArtifact.artifactId : "none")}");
            if (targetArtifact != null)
            {
                targetArtifact.gameObject.SetActive(false);
            }

            if (dayState != null)
            {
                dayState.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            if (dayState != null)
            {
                dayState.StateChanged -= Refresh;
            }
        }

        public void SetHint(string hint)
        {
            if (hintText != null)
            {
                hintText.text = hint;
                hintText.enabled = !string.IsNullOrEmpty(hint);
            }
        }

        public void MarkCleared()
        {
            Cleared = true;
            Debug.Log("[Changan] Objective marked cleared");
            if (targetArtifact != null)
            {
                targetArtifact.gameObject.SetActive(true);
            }

            Refresh();
        }

        public void MarkSampled(RestorationArtifact artifact)
        {
            Sampled = true;
            targetArtifact = artifact;
            Debug.Log($"[Changan] Objective marked sampled artifact={(artifact != null ? artifact.artifactId : "none")}");
            if (dayState != null)
            {
                dayState.MarkArtifactCollected();
            }
            Refresh();
        }

        public void MarkRepaired()
        {
            if (Repaired)
            {
                return;
            }

            Repaired = true;
            Debug.Log("[Changan] Objective marked repaired");
            if (dayState != null)
            {
                dayState.MarkRestorationReady();
            }
            Refresh();
        }

        public void MarkDisplayed()
        {
            if (Displayed)
            {
                return;
            }

            Displayed = true;
            Debug.Log("[Changan] Objective marked displayed");
            Refresh();
        }

        private void Refresh()
        {
            if (objectiveText == null)
            {
                return;
            }

            var step = BuildStepText();
            var phase = dayState != null ? dayState.currentPhase.ToString() : "Proof";
            var branch = dayState != null && dayState.SelectedRestorationBranch != RestorationBranch.None
                ? dayState.SelectedRestorationBranch.ToString()
                : "Pending";
            objectiveText.text = $"Goal: {step}\nFound: {(Sampled ? 1 : 0)}/1  Displayed: {(Displayed ? 1 : 0)}/1  Branch: {branch}  Phase: {phase}";
        }

        private string BuildStepText()
        {
            if (dayState == null || !dayState.CommissionAccepted)
            {
                return "Talk to Han Niangzi at the night market";
            }

            if (!Cleared)
            {
                return "Travel to the relic yard and clear the overgrowth";
            }

            if (!Sampled)
            {
                return "Sample the uncovered lotus roof tile";
            }

            if (!Repaired)
            {
                return dayState.HasConsultedSteleDu
                    ? "Return to the bureau and choose a restoration path"
                    : "Optional: consult Stele Rubbing Du, then return to the bureau";
            }

            if (dayState != null && dayState.SelectedRestorationBranch == RestorationBranch.QuickReuse && !dayState.OutcomeResolved)
            {
                return "Return the restored tile to Han Niangzi";
            }

            if (dayState != null && dayState.SelectedRestorationBranch == RestorationBranch.CarefulExhibit && !Displayed)
            {
                return "Place the conserved tile on the display stand";
            }

            if (dayState != null && !dayState.OutcomeResolved)
            {
                return "Return to Han Niangzi to resolve the commission";
            }

            if (dayState != null && !dayState.DaySummaryShown)
            {
                return "Talk to Apprentice Dou to write the day ledger";
            }

            return "Day complete. Prepare the next commission";
        }
    }
}
