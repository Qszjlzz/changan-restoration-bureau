using UnityEngine;

namespace ChanganRestorationBureau
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class MapCleanupInteractable : MonoBehaviour
    {
        public string displayName = "可清理荒草";
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
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.65f, 0.65f, 0.65f, 0.35f);
            }
        }
    }
}
