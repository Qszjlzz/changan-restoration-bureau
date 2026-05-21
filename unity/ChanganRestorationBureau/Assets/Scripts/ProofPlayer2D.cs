using UnityEngine;

namespace ChanganRestorationBureau
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ProofPlayer2D : MonoBehaviour
    {
        public float moveSpeed = 4.2f;
        public Vector2 minBounds = new Vector2(-9.4f, -5.0f);
        public Vector2 maxBounds = new Vector2(9.4f, 5.0f);
        public Sprite downSprite;
        public Sprite upSprite;
        public Sprite leftSprite;
        public Sprite rightSprite;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private Vector2 lastFacing = Vector2.down;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            spriteRenderer = GetComponent<SpriteRenderer>();
            ApplyFacingSprite();
        }

        private void FixedUpdate()
        {
            var input = Vector2.zero;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                input.x -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                input.x += 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                input.y -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                input.y += 1f;
            }

            if (input.sqrMagnitude > 0.01f)
            {
                lastFacing = Mathf.Abs(input.x) > Mathf.Abs(input.y)
                    ? new Vector2(Mathf.Sign(input.x), 0f)
                    : new Vector2(0f, Mathf.Sign(input.y));
                ApplyFacingSprite();
            }

            body.velocity = input.normalized * moveSpeed;
            body.position = new Vector2(
                Mathf.Clamp(body.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(body.position.y, minBounds.y, maxBounds.y));
        }

        private void ApplyFacingSprite()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (lastFacing.y > 0.1f && upSprite != null)
            {
                spriteRenderer.sprite = upSprite;
            }
            else if (lastFacing.x < -0.1f && leftSprite != null)
            {
                spriteRenderer.sprite = leftSprite;
            }
            else if (lastFacing.x > 0.1f && rightSprite != null)
            {
                spriteRenderer.sprite = rightSprite;
            }
            else if (downSprite != null)
            {
                spriteRenderer.sprite = downSprite;
            }
        }
    }
}
