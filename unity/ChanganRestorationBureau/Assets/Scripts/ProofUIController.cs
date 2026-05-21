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

            titleText.text = hasSelection ? selected.displayName : "选择一件待修物";
            eraText.text = hasSelection ? selected.eraTag : "点击铺内 6 件文物，检查比例、pivot 与 UI 插槽。";
            stateText.text = hasSelection ? BuildState(selected) : "未选择";
            iconImage.enabled = hasSelection;
            iconImage.sprite = hasSelection ? selected.GetComponent<SpriteRenderer>().sprite : null;
            repairButton.interactable = hasSelection && !selected.IsRepaired;
            displayButton.interactable = hasSelection && selected.IsRepaired && !selected.IsDisplayed;
        }

        private static string BuildState(RestorationArtifact artifact)
        {
            if (artifact.IsDisplayed)
            {
                return "已陈列";
            }

            return artifact.IsRepaired ? "已修复，可陈列" : "残缺，等待修复";
        }
    }
}
