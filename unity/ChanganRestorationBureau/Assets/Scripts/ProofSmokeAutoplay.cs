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
        public ProofDayState dayState;
        public ProofRestorationChoiceController restorationChoice;
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

            var smokeBranch = GetRequestedBranch();

            Debug.Log($"[Changan] Smoke autoplay started branch={smokeBranch}");
            yield return new WaitForSeconds(startDelay);

            yield return RunNpcStep("take_commission", "han_niangzi");
            yield return RunStep("cleanup", new Vector2(1.7f, -2.15f));
            yield return RunStep("sample", GetPrimaryArtifact().transform.position);
            yield return RunNpcStep("consult_du", "stele_du");
            yield return RunRepairStep(controller.workbenchSlot.transform.position, smokeBranch);
            if (smokeBranch == RestorationBranch.CarefulExhibit)
            {
                yield return RunStep("display", controller.displaySlots[0].transform.position);
            }
            yield return RunNpcStep("resolve_han", "han_niangzi");
            yield return RunNpcStep("day_summary", "apprentice_dou");

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

            if (dayState == null && controller != null)
            {
                dayState = controller.GetComponent<ProofDayState>();
            }

            if (player == null && interaction != null)
            {
                player = interaction.transform;
            }

            if (restorationChoice == null && interaction != null)
            {
                restorationChoice = interaction.restorationChoice;
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

        private IEnumerator RunRepairStep(Vector2 position, RestorationBranch branch)
        {
            MovePlayer(position);
            yield return null;

            var success = interaction.TryInteractNearest();
            Debug.Log($"[Changan] Smoke step=repair_open success={success}");
            if (!success)
            {
                Debug.LogError("[Changan] Smoke autoplay failed at step=repair_open");
                Application.Quit();
                yield break;
            }

            if (restorationChoice == null || !restorationChoice.IsOpen)
            {
                Debug.LogError("[Changan] Smoke autoplay failed because restoration choice did not open.");
                Application.Quit();
                yield break;
            }

            var branchSuccess = restorationChoice.TryChooseBranch(branch);
            Debug.Log($"[Changan] Smoke step=repair_branch branch={branch} success={branchSuccess}");
            if (!branchSuccess)
            {
                Debug.LogError($"[Changan] Smoke autoplay failed at repair branch={branch}");
                Application.Quit();
                yield break;
            }

            yield return new WaitForSeconds(stepDelay);
        }

        private IEnumerator RunNpcStep(string label, string npcId)
        {
            var npc = FindNpc(npcId);
            if (npc == null)
            {
                Debug.LogError($"[Changan] Smoke autoplay missing npc={npcId}");
                Application.Quit();
                yield break;
            }

            MovePlayer(npc.transform.position);
            yield return null;

            var success = interaction.TryInteractNearest();
            Debug.Log($"[Changan] Smoke step={label} success={success}");
            if (!success)
            {
                Debug.LogError($"[Changan] Smoke autoplay failed at npc step={label}");
                Application.Quit();
                yield break;
            }

            while (interaction.dialogue != null && interaction.dialogue.IsOpen)
            {
                interaction.dialogue.Advance();
                yield return null;
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

        private static RestorationBranch GetRequestedBranch()
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index] == "-smoke-branch" && index + 1 < args.Length)
                {
                    return ParseBranch(args[index + 1]);
                }

                if (args[index].StartsWith("-smoke-branch=", System.StringComparison.OrdinalIgnoreCase))
                {
                    var value = args[index].Substring("-smoke-branch=".Length);
                    return ParseBranch(value);
                }
            }

            return RestorationBranch.CarefulExhibit;
        }

        private static RestorationBranch ParseBranch(string raw)
        {
            return string.Equals(raw, "quick_reuse", System.StringComparison.OrdinalIgnoreCase)
                ? RestorationBranch.QuickReuse
                : RestorationBranch.CarefulExhibit;
        }

        private RestorationArtifact GetPrimaryArtifact()
        {
            return controller.artifacts.FirstOrDefault(artifact => artifact.artifactId == "artifact_roof_tile")
                ?? controller.artifacts[0];
        }

        private static ProofNpcInteractable FindNpc(string npcId)
        {
            return FindObjectsOfType<ProofNpcInteractable>().FirstOrDefault(npc => npc.npcId == npcId);
        }
    }
}
