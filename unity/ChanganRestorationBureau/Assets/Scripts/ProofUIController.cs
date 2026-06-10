using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofUIController : MonoBehaviour
    {
        public Text titleText;
        public Text eraText;
        public Text stateText;
        public Image iconImage;
        public Button repairButton;
        public Button displayButton;

        private AssetProofController controller;
        private RestorationArtifact selected;

        public void Bind(AssetProofController proofController)
        {
            controller = proofController;
            repairButton.onClick.AddListener(controller.RequestRepairChoice);
            displayButton.onClick.AddListener(controller.DisplaySelected);
            ShowSelection(null);
        }

        public void ShowSelection(RestorationArtifact artifact)
        {
            selected = artifact;
            var hasSelection = selected != null;
            var selectedBranch = controller != null && controller.dayState != null
                ? controller.dayState.SelectedRestorationBranch
                : RestorationBranch.None;

            titleText.text = hasSelection ? selected.displayName : "Select a relic to restore";
            eraText.text = hasSelection ? selected.eraTag : "Inspect relic scale, pivot, and UI fit.";
            stateText.text = hasSelection ? BuildState(selected, selectedBranch) : "Nothing selected";
            iconImage.enabled = hasSelection;
            iconImage.sprite = hasSelection ? selected.GetComponent<SpriteRenderer>().sprite : null;
            repairButton.interactable = hasSelection && !selected.IsRepaired;
            displayButton.interactable = hasSelection
                && selected.IsRepaired
                && !selected.IsDisplayed
                && selectedBranch != RestorationBranch.QuickReuse;
        }

        private static string BuildState(RestorationArtifact artifact, RestorationBranch branch)
        {
            if (artifact.IsDisplayed)
            {
                return "Displayed";
            }

            if (artifact.IsRepaired)
            {
                return branch == RestorationBranch.QuickReuse
                    ? "Repaired and ready to return"
                    : "Repaired and ready to display";
            }

            return "Damaged and waiting for repair";
        }
    }
}
