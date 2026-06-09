using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofConversation
    {
        public string speakerName;
        public string[] lines = Array.Empty<string>();
        public Sprite portrait;
        public string footerHint = "Press E to continue";
        public Action onComplete;
    }

    public sealed class ProofDialogueController : MonoBehaviour
    {
        public GameObject panelRoot;
        public Text speakerText;
        public Text bodyText;
        public Text footerText;
        public Image portraitImage;

        public bool IsOpen => activeConversation != null;
        public string FooterHint => IsOpen ? activeConversation.footerHint : string.Empty;

        private ProofConversation activeConversation;
        private int lineIndex;

        public void BeginConversation(ProofConversation conversation)
        {
            if (conversation == null)
            {
                return;
            }

            if (conversation.lines == null || conversation.lines.Length == 0)
            {
                conversation.lines = new[] { "..." };
            }

            activeConversation = conversation;
            lineIndex = 0;
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            Debug.Log($"[Changan] Dialogue opened speaker={conversation.speakerName}");
            Refresh();
        }

        public void Advance()
        {
            if (!IsOpen)
            {
                return;
            }

            if (lineIndex < activeConversation.lines.Length - 1)
            {
                lineIndex++;
                Refresh();
                return;
            }

            CloseConversation();
        }

        private void CloseConversation()
        {
            var completed = activeConversation;
            activeConversation = null;
            lineIndex = 0;
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }

            Debug.Log($"[Changan] Dialogue closed speaker={completed.speakerName}");
            completed.onComplete?.Invoke();
        }

        private void Refresh()
        {
            if (!IsOpen)
            {
                return;
            }

            if (speakerText != null)
            {
                speakerText.text = activeConversation.speakerName;
            }

            if (bodyText != null)
            {
                bodyText.text = activeConversation.lines[Mathf.Clamp(lineIndex, 0, activeConversation.lines.Length - 1)];
            }

            if (footerText != null)
            {
                footerText.text = activeConversation.footerHint;
            }

            if (portraitImage != null)
            {
                portraitImage.enabled = activeConversation.portrait != null;
                portraitImage.sprite = activeConversation.portrait;
            }
        }
    }
}
