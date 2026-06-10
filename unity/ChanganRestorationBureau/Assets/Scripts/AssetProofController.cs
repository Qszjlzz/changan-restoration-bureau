using System;
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
        public ProofDayState dayState;

        public event Action<RestorationArtifact> RepairChoiceRequested;

        private RestorationArtifact selected;

        private void Start()
        {
            if (dayState == null)
            {
                dayState = GetComponent<ProofDayState>();
            }

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

            if (selected.IsRepaired)
            {
                ui.ShowSelection(selected);
                return;
            }

            if (dayState != null && dayState.SelectedRestorationBranch == RestorationBranch.None)
            {
                RequestRepairChoice();
                return;
            }

            var branch = dayState != null ? dayState.SelectedRestorationBranch : RestorationBranch.QuickReuse;
            TryApplyRestorationChoice(branch);
        }

        public void DisplaySelected()
        {
            if (selected == null || !selected.IsRepaired)
            {
                return;
            }

            if (dayState != null && dayState.SelectedRestorationBranch == RestorationBranch.QuickReuse)
            {
                Debug.Log("[Changan] DisplaySelected blocked because the quick reuse route returns directly to Han.");
                ui.ShowSelection(selected);
                return;
            }

            Debug.Log($"[Changan] DisplaySelected artifact={selected.artifactId}");
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

        public void RequestRepairChoice()
        {
            if (selected == null || selected.IsRepaired)
            {
                return;
            }

            if (dayState == null)
            {
                dayState = GetComponent<ProofDayState>();
            }

            if (dayState != null && dayState.SelectedRestorationBranch != RestorationBranch.None)
            {
                TryApplyRestorationChoice(dayState.SelectedRestorationBranch);
                return;
            }

            Debug.Log($"[Changan] Repair choice requested artifact={selected.artifactId}");
            RepairChoiceRequested?.Invoke(selected);
        }

        public bool TryApplyRestorationChoice(RestorationBranch branch)
        {
            if (selected == null)
            {
                return false;
            }

            if (selected.IsRepaired)
            {
                ui.ShowSelection(selected);
                return true;
            }

            if (dayState == null)
            {
                dayState = GetComponent<ProofDayState>();
            }

            if (dayState != null)
            {
                if (dayState.SelectedRestorationBranch == RestorationBranch.None)
                {
                    if (!dayState.CanChooseBranch(branch))
                    {
                        Debug.LogWarning($"[Changan] Repair choice blocked branch={branch} artifact={selected.artifactId}");
                        return false;
                    }

                    dayState.ChooseRestorationBranch(branch);
                }
                else if (dayState.SelectedRestorationBranch != branch)
                {
                    Debug.LogWarning($"[Changan] Repair choice mismatch requested={branch} selected={dayState.SelectedRestorationBranch}");
                    return false;
                }
            }

            Debug.Log($"[Changan] RepairSelected artifact={selected.artifactId} branch={branch}");
            selected.Repair();
            if (workbenchSlot != null && workbenchSlot.Occupant != selected && !selected.IsDisplayed)
            {
                workbenchSlot.TryPlace(selected);
            }

            ui.ShowSelection(selected);
            return true;
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

            Debug.Log($"[Changan] SelectArtifact artifact={(selected != null ? selected.artifactId : "none")}");
            ui.ShowSelection(selected);
        }
    }
}
