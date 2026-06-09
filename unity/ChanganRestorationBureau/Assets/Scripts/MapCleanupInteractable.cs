using UnityEngine;

namespace ChanganRestorationBureau
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class MapCleanupInteractable : MonoBehaviour
    {
        public string displayName = "Clearable obstacle";
        public Sprite clearedSprite;
        public bool IsCleared { get; private set; }

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnMouseDown()
        {
            Clear();
        }

        public void Clear()
        {
            if (IsCleared)
            {
                return;
            }

            IsCleared = true;
            Debug.Log($"[Changan] Cleanup cleared obstacle={name}");
            if (spriteRenderer != null)
            {
                if (clearedSprite != null)
                {
                    spriteRenderer.sprite = clearedSprite;
                    spriteRenderer.color = Color.white;
                }
                else
                {
                    spriteRenderer.color = new Color(0.65f, 0.65f, 0.65f, 0.35f);
                }
            }
        }
    }
}
