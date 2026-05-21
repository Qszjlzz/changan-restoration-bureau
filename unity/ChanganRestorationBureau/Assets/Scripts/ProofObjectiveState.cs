using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofObjectiveState : MonoBehaviour
    {
        public Text objectiveText;
        public Text hintText;
        public RestorationArtifact targetArtifact;

        public bool Cleared { get; private set; }
        public bool Sampled { get; private set; }
        public bool Repaired { get; private set; }
        public bool Displayed { get; private set; }

        private void Start()
        {
            if (targetArtifact != null)
            {
                targetArtifact.gameObject.SetActive(false);
            }
            Refresh();
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
            Refresh();
        }

        public void MarkRepaired()
        {
            Repaired = true;
            Refresh();
        }

        public void MarkDisplayed()
        {
            Displayed = true;
            Refresh();
        }

        private void Refresh()
        {
            if (objectiveText == null)
            {
                return;
            }

            var step = "清理荒草，寻找待修物";
            if (Displayed)
            {
                step = "已完成：文物修复并陈列";
            }
            else if (Repaired)
            {
                step = "带到展柜陈列";
            }
            else if (Sampled)
            {
                step = "回修物局修复";
            }
            else if (Cleared)
            {
                step = "采样露出的文物";
            }

            objectiveText.text = $"目标：{step}\n发现：{(Sampled ? 1 : 0)}/1　陈列：{(Displayed ? 1 : 0)}/1";
        }
    }
}
