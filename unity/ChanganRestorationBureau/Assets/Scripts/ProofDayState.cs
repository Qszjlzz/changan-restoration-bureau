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

        public ProofNarrativeCatalog Catalog { get; private set; }

        private void Awake()
        {
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
        }
    }
}
