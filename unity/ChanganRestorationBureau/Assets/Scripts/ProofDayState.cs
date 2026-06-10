using System;
using System.Linq;
using UnityEngine;

namespace ChanganRestorationBureau
{
    public enum ProofDayPhase
    {
        FreeRoamStart,
        CommissionAccepted,
        InvestigationOpen,
        ArtifactCollected,
        ConsultationOpen,
        RestorationReady,
        OutcomeResolved,
        DaySummary
    }

    public enum RestorationBranch
    {
        None,
        QuickReuse,
        CarefulExhibit
    }

    [Serializable]
    public sealed class ProofNarrativeCatalog
    {
        public ProofNpcDefinition[] npcs = Array.Empty<ProofNpcDefinition>();
        public ProofCommissionDefinition[] commissions = Array.Empty<ProofCommissionDefinition>();
    }

    [Serializable]
    public sealed class ProofNpcDefinition
    {
        public string npcId;
        public string displayName;
        public string role;
        public string homeLandmarkId;
        public string idleAssetId;
        public string portraitAssetId;
    }

    [Serializable]
    public sealed class ProofCommissionDefinition
    {
        public string commissionId;
        public string title;
        public string artifactId;
        public string commissionerNpcId;
        public string guideNpcId;
        public string consultantNpcId;
        public string[] storyBeats = Array.Empty<string>();
        public ProofReward quickReuseReward;
        public ProofReward carefulExhibitReward;
    }

    [Serializable]
    public sealed class ProofReward
    {
        public int coins;
        public int neighborhoodTrust;
        public int scholarlyReputation;
        public int workHours;
    }

    public sealed class ProofDayState : MonoBehaviour
    {
        public string catalogResourcePath = "Data/ProofNarrativeCatalog";
        public string activeCommissionId = "lotus_roof_tile_night_market";
        public ProofDayPhase currentPhase = ProofDayPhase.FreeRoamStart;
        public int startingCoins = 12;
        public int startingWorkHours = 5;
        public int startingPaste = 2;
        public int startingStonePowder = 1;
        public int startingNeighborhoodTrust;
        public int startingScholarlyReputation;

        public ProofNarrativeCatalog Catalog { get; private set; }
        public bool CommissionAccepted { get; private set; }
        public bool HasConsultedSteleDu { get; private set; }
        public bool OutcomeResolved { get; private set; }
        public bool DaySummaryShown { get; private set; }
        public string ResolvedOutcomeId { get; private set; }
        public RestorationBranch SelectedRestorationBranch { get; private set; }
        public int Coins { get; private set; }
        public int WorkHours { get; private set; }
        public int Paste { get; private set; }
        public int StonePowder { get; private set; }
        public int NeighborhoodTrust { get; private set; }
        public int ScholarlyReputation { get; private set; }

        public event Action StateChanged;

        private void Awake()
        {
            Coins = startingCoins;
            WorkHours = startingWorkHours;
            Paste = startingPaste;
            StonePowder = startingStonePowder;
            NeighborhoodTrust = startingNeighborhoodTrust;
            ScholarlyReputation = startingScholarlyReputation;
            LoadCatalog();
            LogCurrentState();
        }

        public ProofCommissionDefinition GetActiveCommission()
        {
            return Catalog?.commissions?.FirstOrDefault(commission => commission.commissionId == activeCommissionId);
        }

        public void SetPhase(ProofDayPhase phase)
        {
            currentPhase = phase;
            Debug.Log($"[Changan] Day phase changed phase={phase}");
            NotifyStateChanged();
        }

        public void AcceptCommission()
        {
            if (CommissionAccepted)
            {
                return;
            }

            CommissionAccepted = true;
            Debug.Log($"[Changan] Commission accepted id={activeCommissionId}");
            SetPhase(ProofDayPhase.InvestigationOpen);
        }

        public void MarkArtifactCollected()
        {
            SetPhase(ProofDayPhase.ArtifactCollected);
        }

        public void MarkConsultedSteleDu()
        {
            if (HasConsultedSteleDu)
            {
                return;
            }

            HasConsultedSteleDu = true;
            SpendWorkHours(1);
            Debug.Log("[Changan] Consultation completed npc=stele_du");
            SetPhase(ProofDayPhase.ConsultationOpen);
        }

        public void MarkRestorationReady()
        {
            SetPhase(ProofDayPhase.RestorationReady);
        }

        public bool CanChooseBranch(RestorationBranch branch)
        {
            switch (branch)
            {
                case RestorationBranch.QuickReuse:
                    return WorkHours > 0 && Paste > 0;
                case RestorationBranch.CarefulExhibit:
                    return WorkHours > 0 && StonePowder > 0;
                default:
                    return false;
            }
        }

        public bool CanConsultSteleDu()
        {
            return WorkHours > 0;
        }

        public string GetBranchBlockedReason(RestorationBranch branch)
        {
            switch (branch)
            {
                case RestorationBranch.QuickReuse:
                    if (WorkHours <= 0 && Paste <= 0)
                    {
                        return "Quick reuse needs 1 work hour and 1 paste.";
                    }

                    if (WorkHours <= 0)
                    {
                        return "Quick reuse needs 1 more work hour.";
                    }

                    if (Paste <= 0)
                    {
                        return "Quick reuse needs 1 paste.";
                    }

                    return "Quick reuse is ready.";
                case RestorationBranch.CarefulExhibit:
                    if (WorkHours <= 0 && StonePowder <= 0)
                    {
                        return "Careful exhibit needs 1 work hour and 1 stone powder.";
                    }

                    if (WorkHours <= 0)
                    {
                        return "Careful exhibit needs 1 more work hour.";
                    }

                    if (StonePowder <= 0)
                    {
                        return "Careful exhibit needs 1 stone powder.";
                    }

                    return "Careful exhibit is ready.";
                default:
                    return "Choose a restoration route.";
            }
        }

        public string BuildBudgetHudText()
        {
            var routeLabel = SelectedRestorationBranch == RestorationBranch.None
                ? "Unchosen"
                : SelectedRestorationBranch == RestorationBranch.QuickReuse
                    ? "Quick Reuse"
                    : "Careful Exhibit";

            return $"Day Budget\nCoins {Coins}   Hours {WorkHours}\nPaste {Paste}   Stone {StonePowder}\nTrust {NeighborhoodTrust}   Reputation {ScholarlyReputation}\nRoute {routeLabel}\n{BuildNextPressureText()}";
        }

        public void ChooseRestorationBranch(RestorationBranch branch)
        {
            if (branch == RestorationBranch.None || SelectedRestorationBranch != RestorationBranch.None)
            {
                return;
            }

            if (!CanChooseBranch(branch))
            {
                Debug.LogWarning($"[Changan] Cannot choose restoration branch branch={branch} hours={WorkHours} paste={Paste} stone={StonePowder}");
                return;
            }

            SelectedRestorationBranch = branch;
            SpendWorkHours(1);
            switch (branch)
            {
                case RestorationBranch.QuickReuse:
                    Paste = Mathf.Max(0, Paste - 1);
                    break;
                case RestorationBranch.CarefulExhibit:
                    StonePowder = Mathf.Max(0, StonePowder - 1);
                    break;
            }

            Debug.Log($"[Changan] Restoration branch chosen branch={branch} hours={WorkHours} paste={Paste} stone={StonePowder}");
            NotifyStateChanged();
        }

        public void ResolveOutcome(string outcomeId)
        {
            if (OutcomeResolved)
            {
                return;
            }

            var commission = GetActiveCommission();
            ResolvedOutcomeId = outcomeId;
            OutcomeResolved = true;
            if (commission != null)
            {
                var reward = string.Equals(outcomeId, "careful_exhibit", StringComparison.OrdinalIgnoreCase)
                    ? commission.carefulExhibitReward
                    : commission.quickReuseReward;
                ApplyReward(reward);
            }

            Debug.Log($"[Changan] Outcome resolved id={outcomeId} coins={Coins} trust={NeighborhoodTrust} reputation={ScholarlyReputation}");
            SetPhase(ProofDayPhase.OutcomeResolved);
        }

        public void ShowDaySummary()
        {
            if (DaySummaryShown)
            {
                return;
            }

            DaySummaryShown = true;
            Debug.Log("[Changan] Day summary shown");
            SetPhase(ProofDayPhase.DaySummary);
        }

        public string BuildSummaryText()
        {
            var outcomeLabel = string.IsNullOrEmpty(ResolvedOutcomeId) ? "Unresolved" : ResolvedOutcomeId;
            var branchLabel = SelectedRestorationBranch == RestorationBranch.None ? "Not chosen" : SelectedRestorationBranch.ToString();
            return $"Day Summary\nBranch: {branchLabel}\nOutcome: {outcomeLabel}\nCoins: {Coins}\nTrust: {NeighborhoodTrust}\nReputation: {ScholarlyReputation}\nWork Hours: {WorkHours}\nPaste: {Paste}\nStone Powder: {StonePowder}";
        }

        private string BuildNextPressureText()
        {
            if (!CommissionAccepted)
            {
                return "Next: Meet Han at the night market";
            }

            if (OutcomeResolved && !DaySummaryShown)
            {
                return "Next: Talk to Dou for the day ledger";
            }

            if ((currentPhase == ProofDayPhase.ArtifactCollected
                || currentPhase == ProofDayPhase.ConsultationOpen
                || currentPhase == ProofDayPhase.RestorationReady)
                && SelectedRestorationBranch == RestorationBranch.None)
            {
                return "Bench: Quick -1h/-1 paste | Careful -1h/-1 stone";
            }

            if (SelectedRestorationBranch == RestorationBranch.QuickReuse && !OutcomeResolved)
            {
                return "Next: Return the restored tile to Han";
            }

            if (SelectedRestorationBranch == RestorationBranch.CarefulExhibit && !OutcomeResolved)
            {
                return "Next: Display the conserved tile";
            }

            return $"Stage: {currentPhase}";
        }

        private void SpendWorkHours(int amount)
        {
            WorkHours = Mathf.Max(0, WorkHours - Mathf.Max(0, amount));
            NotifyStateChanged();
        }

        private void ApplyReward(ProofReward reward)
        {
            if (reward == null)
            {
                return;
            }

            Coins += reward.coins;
            NeighborhoodTrust += reward.neighborhoodTrust;
            ScholarlyReputation += reward.scholarlyReputation;
            WorkHours += reward.workHours;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }

        private void LoadCatalog()
        {
            var textAsset = Resources.Load<TextAsset>(catalogResourcePath);
            if (textAsset == null)
            {
                Debug.LogWarning($"[Changan] Missing narrative catalog resource={catalogResourcePath}");
                return;
            }

            Catalog = JsonUtility.FromJson<ProofNarrativeCatalog>(textAsset.text);
            if (Catalog == null)
            {
                Debug.LogWarning("[Changan] Failed to parse narrative catalog.");
            }
        }

        private void LogCurrentState()
        {
            var commission = GetActiveCommission();
            if (commission == null)
            {
                Debug.LogWarning($"[Changan] Active commission not found id={activeCommissionId}");
                return;
            }

            Debug.Log($"[Changan] Day state ready phase={currentPhase} commission={commission.commissionId} title={commission.title}");
            NotifyStateChanged();
        }
    }
}
