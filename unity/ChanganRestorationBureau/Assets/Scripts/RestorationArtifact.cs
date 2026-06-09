using UnityEngine;

namespace ChanganRestorationBureau
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class RestorationArtifact : MonoBehaviour
    {
        public string artifactId;
        public string displayName;
        public string eraTag;
        public Sprite damagedSprite;
        public Sprite repairedSprite;
        public bool IsRepaired { get; private set; }
        public bool IsDisplayed { get; private set; }

        private SpriteRenderer spriteRenderer;
        private Vector3 originalScale;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
            ApplyState();
        }

        public void SetSelected(bool selected)
        {
            transform.localScale = selected ? originalScale * 1.12f : originalScale;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = selected ? new Color(1f, 0.92f, 0.58f, 1f) : Color.white;
            }
        }

        public void Repair()
        {
            IsRepaired = true;
            Debug.Log($"[Changan] Artifact repaired id={artifactId}");
            ApplyState();
        }

        public void MarkDisplayed(bool displayed)
        {
            IsDisplayed = displayed;
            Debug.Log($"[Changan] Artifact display state id={artifactId} displayed={displayed}");
            ApplyState();
        }

        private void ApplyState()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.sprite = IsRepaired && repairedSprite != null ? repairedSprite : damagedSprite;
            spriteRenderer.sortingOrder = IsDisplayed ? 32 : 20;
        }
    }
}
