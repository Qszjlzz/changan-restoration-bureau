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
            Debug.Log($"[Changan] ProofObjectiveState.Start target={(targetArtifact != null ? targetArtifact.artifactId : "none")}");
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
            Refresh();
        }

        public void MarkRepaired()
        {
            Repaired = true;
            Debug.Log("[Changan] Objective marked repaired");
            Refresh();
        }

        public void MarkDisplayed()
        {
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

            var step = "Clear the grass and find the relic";
            if (Displayed)
            {
                step = "Completed: relic repaired and displayed";
            }
            else if (Repaired)
            {
                step = "Bring the relic to the display case";
            }
            else if (Sampled)
            {
                step = "Return to the bureau and repair the relic";
            }
            else if (Cleared)
            {
                step = "Sample the uncovered relic";
            }

            objectiveText.text = $"Goal: {step}\nFound: {(Sampled ? 1 : 0)}/1  Displayed: {(Displayed ? 1 : 0)}/1";
        }
    }
}
