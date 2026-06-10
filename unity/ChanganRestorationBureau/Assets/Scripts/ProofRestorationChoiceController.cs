using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofRestorationChoiceController : MonoBehaviour
    {
        public GameObject panelRoot;
        public Text titleText;
        public Text bodyText;
        public Text resourceText;
        public Text footerText;
        public Text quickLabelText;
        public Text carefulLabelText;
        public Button quickReuseButton;
        public Button carefulExhibitButton;
        public ProofResourceHudController resourceHud;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
        public string FooterHint => IsOpen ? "Press 1 for quick reuse, 2 for careful exhibit" : string.Empty;

        private AssetProofController controller;
        private ProofDayState dayState;
        private ProofObjectiveState objective;
        private RestorationArtifact pendingArtifact;

        public void Bind(AssetProofController proofController, ProofDayState proofDayState, ProofObjectiveState objectiveState)
        {
            controller = proofController;
            dayState = proofDayState;
            objective = objectiveState;

            if (controller != null)
            {
                controller.RepairChoiceRequested -= OpenForArtifact;
                controller.RepairChoiceRequested += OpenForArtifact;
            }

            if (quickReuseButton != null)
            {
                quickReuseButton.onClick.RemoveAllListeners();
                quickReuseButton.onClick.AddListener(() => TryChooseBranch(RestorationBranch.QuickReuse));
            }

            if (carefulExhibitButton != null)
            {
                carefulExhibitButton.onClick.RemoveAllListeners();
                carefulExhibitButton.onClick.AddListener(() => TryChooseBranch(RestorationBranch.CarefulExhibit));
            }

            Close();
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.RepairChoiceRequested -= OpenForArtifact;
            }
        }

        public void HandleRuntimeInput()
        {
            if (!IsOpen)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryChooseBranch(RestorationBranch.QuickReuse);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryChooseBranch(RestorationBranch.CarefulExhibit);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public bool TryChooseBranch(RestorationBranch branch)
        {
            if (!IsOpen || controller == null || pendingArtifact == null)
            {
                return false;
            }

            controller.SelectArtifactFromInteraction(pendingArtifact);
            var success = controller.TryApplyRestorationChoice(branch);
            Debug.Log($"[Changan] Restoration choice attempted branch={branch} success={success}");
            if (!success)
            {
                if (dayState != null)
                {
                    resourceHud?.ShowActionFeedback(dayState.GetBranchBlockedReason(branch), true);
                }

                Refresh();
                return false;
            }

            resourceHud?.ShowActionFeedback(
                branch == RestorationBranch.QuickReuse
                    ? "Quick reuse chosen: -1 hour, -1 paste."
                    : "Careful exhibit chosen: -1 hour, -1 stone powder.");

            if (objective != null)
            {
                objective.MarkRepaired();
            }

            Close();
            return true;
        }

        private void OpenForArtifact(RestorationArtifact artifact)
        {
            pendingArtifact = artifact;
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            Debug.Log($"[Changan] Restoration choice opened artifact={(artifact != null ? artifact.artifactId : "none")}");
            Refresh();
        }

        private void Close()
        {
            pendingArtifact = null;
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void Refresh()
        {
            if (dayState == null || !IsOpen)
            {
                return;
            }

            var artifactName = pendingArtifact != null ? pendingArtifact.displayName : "Selected relic";
            if (titleText != null)
            {
                titleText.text = "Choose Restoration Intention";
            }

            if (bodyText != null)
            {
                bodyText.text = $"{artifactName}\nDecide whether it should return to daily use or be conserved for display.";
            }

            if (resourceText != null)
            {
                resourceText.text = $"Hours {dayState.WorkHours}   Paste {dayState.Paste}   Stone {dayState.StonePowder}";
            }

            var canQuickReuse = dayState.CanChooseBranch(RestorationBranch.QuickReuse);
            var canCarefulExhibit = dayState.CanChooseBranch(RestorationBranch.CarefulExhibit);

            if (quickLabelText != null)
            {
                quickLabelText.text = canQuickReuse
                    ? "Quick Reuse\n-1 Paste  ->  +12 Coins, +2 Trust"
                    : "Quick Reuse\nNeed 1 hour and 1 paste";
            }

            if (carefulLabelText != null)
            {
                carefulLabelText.text = canCarefulExhibit
                    ? "Careful Exhibit\n-1 Stone  ->  +8 Coins, +2 Reputation"
                    : "Careful Exhibit\nNeed 1 hour and 1 stone powder";
            }

            if (quickReuseButton != null)
            {
                quickReuseButton.interactable = canQuickReuse;
            }

            if (carefulExhibitButton != null)
            {
                carefulExhibitButton.interactable = canCarefulExhibit;
            }

            if (footerText != null)
            {
                footerText.text = FooterHint;
            }
        }
    }
}
