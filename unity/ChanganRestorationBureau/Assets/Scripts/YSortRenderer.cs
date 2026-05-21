using UnityEngine;

namespace ChanganRestorationBureau
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class YSortRenderer : MonoBehaviour
    {
        public int baseOrder = 1000;
        public int offset;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = baseOrder - Mathf.RoundToInt(transform.position.y * 100f) + offset;
        }
    }
}
