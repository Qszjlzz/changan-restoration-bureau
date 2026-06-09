using System.Collections;
using System.Linq;
using UnityEngine;

namespace ChanganRestorationBureau
{
    public sealed class ProofSmokeAutoplay : MonoBehaviour
    {
        public AssetProofController controller;
        public InteractionController interaction;
        public Transform player;
        public float startDelay = 0.75f;
        public float stepDelay = 0.3f;

        private IEnumerator Start()
        {
            if (!ShouldRun())
            {
                yield break;
            }

            ResolveReferences();
            if (controller == null || interaction == null || player == null)
            {
                Debug.LogError("[Changan] Smoke autoplay missing required references.");
                yield break;
            }

            Debug.Log("[Changan] Smoke autoplay started");
            yield return new WaitForSeconds(startDelay);

            yield return RunStep("cleanup", new Vector2(1.7f, -2.15f));
            yield return RunStep("sample", controller.artifacts[0].transform.position);
            yield return RunStep("repair", controller.workbenchSlot.transform.position);
            yield return RunStep("display", controller.displaySlots[0].transform.position);

            Debug.Log("[Changan] Smoke autoplay completed");
            yield return new WaitForSeconds(0.5f);
            Application.Quit();
        }

        private void ResolveReferences()
        {
            if (controller == null)
            {
                controller = GetComponent<AssetProofController>();
            }

            if (interaction == null)
            {
                interaction = FindObjectOfType<InteractionController>();
            }

            if (player == null && interaction != null)
            {
                player = interaction.transform;
            }
        }

        private IEnumerator RunStep(string label, Vector2 position)
        {
            MovePlayer(position);
            yield return null;

            var success = interaction.TryInteractNearest();
            Debug.Log($"[Changan] Smoke step={label} success={success}");
            if (!success)
            {
                Debug.LogError($"[Changan] Smoke autoplay failed at step={label}");
                Application.Quit();
                yield break;
            }

            yield return new WaitForSeconds(stepDelay);
        }

        private void MovePlayer(Vector2 position)
        {
            var current = player.position;
            player.position = new Vector3(position.x, position.y, current.z);

            var body = player.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.position = position;
                body.velocity = Vector2.zero;
            }
        }

        private static bool ShouldRun()
        {
            return System.Environment.GetCommandLineArgs().Any(arg => arg == "-smoke-play");
        }
    }
}
