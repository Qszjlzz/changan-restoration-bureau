using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofResourceHudController : MonoBehaviour
    {
        public Text budgetText;
        public Text feedbackText;

        private ProofDayState dayState;
        private Coroutine hideFeedbackRoutine;

        public void Bind(ProofDayState proofDayState)
        {
            if (dayState != null)
            {
                dayState.StateChanged -= Refresh;
            }

            dayState = proofDayState;
            if (dayState != null)
            {
                dayState.StateChanged += Refresh;
            }

            if (feedbackText != null)
            {
                feedbackText.enabled = false;
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

        public void ShowActionFeedback(string message, bool warning = false, float duration = 2.2f)
        {
            if (feedbackText == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            feedbackText.text = message;
            feedbackText.color = warning
                ? new Color32(205, 112, 78, 255)
                : new Color32(96, 64, 44, 255);
            feedbackText.enabled = true;

            if (hideFeedbackRoutine != null)
            {
                StopCoroutine(hideFeedbackRoutine);
            }

            hideFeedbackRoutine = StartCoroutine(HideFeedbackAfter(duration));
        }

        private void Refresh()
        {
            if (budgetText == null || dayState == null)
            {
                return;
            }

            budgetText.text = dayState.BuildBudgetHudText();
        }

        private IEnumerator HideFeedbackAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (feedbackText != null)
            {
                feedbackText.enabled = false;
            }

            hideFeedbackRoutine = null;
        }
    }
}
