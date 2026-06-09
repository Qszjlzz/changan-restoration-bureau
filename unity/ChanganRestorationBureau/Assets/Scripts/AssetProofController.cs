using System.Collections.Generic;
using UnityEngine;

namespace ChanganRestorationBureau
{
    public sealed class AssetProofController : MonoBehaviour
    {
        public Camera worldCamera;
        public ProofUIController ui;
        public List<RestorationArtifact> artifacts = new List<RestorationArtifact>();
        public List<ProofSlot> displaySlots = new List<ProofSlot>();
        public ProofSlot workbenchSlot;

        private RestorationArtifact selected;

        private void Start()
        {
            Debug.Log($"[Changan] AssetProofController.Start artifacts={artifacts.Count} displays={displaySlots.Count} workbench={(workbenchSlot != null)} ui={(ui != null)}");
            if (ui == null)
            {
                Debug.LogError("[Changan] Proof UI is missing on AssetProofController.Start.");
                return;
            }

            ui.Bind(this);
            SelectArtifact(artifacts.Count > 0 ? artifacts[0] : null);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectFromPointer();
            }

            for (var i = 0; i < artifacts.Count && i < 9; i++)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    SelectArtifact(artifacts[i]);
                }
            }
        }

        public void RepairSelected()
        {
            if (selected == null)
            {
                return;
            }

            selected.Repair();
            if (workbenchSlot != null && !selected.IsDisplayed)
            {
                workbenchSlot.TryPlace(selected);
            }

            ui.ShowSelection(selected);
        }

        public void DisplaySelected()
        {
            if (selected == null || !selected.IsRepaired)
            {
                return;
            }

            foreach (var slot in displaySlots)
            {
                if (slot.TryPlace(selected))
                {
                    ui.ShowSelection(selected);
                    return;
                }
            }
        }

        private void TrySelectFromPointer()
        {
            var cameraToUse = worldCamera != null ? worldCamera : Camera.main;
            var world = cameraToUse.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapPoint(world);
            if (hit == null)
            {
                return;
            }

            var artifact = hit.GetComponent<RestorationArtifact>();
            if (artifact != null)
            {
                SelectArtifact(artifact);
            }
        }

        public void SelectArtifactFromInteraction(RestorationArtifact artifact)
        {
            SelectArtifact(artifact);
        }

        private void SelectArtifact(RestorationArtifact artifact)
        {
            if (selected != null)
            {
                selected.SetSelected(false);
            }

            selected = artifact;
            if (selected != null)
            {
                selected.SetSelected(true);
            }

            ui.ShowSelection(selected);
        }
    }
}
