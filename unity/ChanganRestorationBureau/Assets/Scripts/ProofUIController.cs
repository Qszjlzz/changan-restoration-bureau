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
            repairButton.onClick.AddListener(() => controller.RepairSelected());
            displayButton.onClick.AddListener(() => controller.DisplaySelected());
            ShowSelection(null);
        }

        public void ShowSelection(RestorationArtifact artifact)
        {
            selected = artifact;
            var hasSelection = selected != null;

            titleText.text = hasSelection ? selected.displayName : "Select a relic to restore";
            eraText.text = hasSelection ? selected.eraTag : "Inspect relic scale, pivot, and UI fit.";
            stateText.text = hasSelection ? BuildState(selected) : "Nothing selected";
            iconImage.enabled = hasSelection;
            iconImage.sprite = hasSelection ? selected.GetComponent<SpriteRenderer>().sprite : null;
            repairButton.interactable = hasSelection && !selected.IsRepaired;
            displayButton.interactable = hasSelection && selected.IsRepaired && !selected.IsDisplayed;
        }

        private static string BuildState(RestorationArtifact artifact)
        {
            if (artifact.IsDisplayed)
            {
                return "Displayed";
            }

            return artifact.IsRepaired ? "Repaired and ready to display" : "Damaged and waiting for repair";
        }
    }
}
