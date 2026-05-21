using UnityEngine;

namespace ChanganRestorationBureau
{
    public sealed class ProofCameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector2 min = new Vector2(-6.8f, -3.25f);
        public Vector2 max = new Vector2(6.8f, 3.25f);
        public float smooth = 10f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desired = new Vector3(
                Mathf.Clamp(target.position.x, min.x, max.x),
                Mathf.Clamp(target.position.y, min.y, max.y),
                transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
        }
    }
}
