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
        public int startingNeighborhoodTrust;
        public int startingScholarlyReputation;

        public ProofNarrativeCatalog Catalog { get; private set; }
        public bool CommissionAccepted { get; private set; }
        public bool HasConsultedSteleDu { get; private set; }
        public bool OutcomeResolved { get; private set; }
        public bool DaySummaryShown { get; private set; }
        public string ResolvedOutcomeId { get; private set; }
        public int Coins { get; private set; }
        public int WorkHours { get; private set; }
        public int NeighborhoodTrust { get; private set; }
        public int ScholarlyReputation { get; private set; }

        public event Action StateChanged;

        private void Awake()
        {
            Coins = startingCoins;
            WorkHours = startingWorkHours;
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
            return $"Day Summary\nOutcome: {outcomeLabel}\nCoins: {Coins}\nTrust: {NeighborhoodTrust}\nReputation: {ScholarlyReputation}\nWork Hours: {WorkHours}";
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
