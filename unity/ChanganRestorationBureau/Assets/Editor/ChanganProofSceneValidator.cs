using System.Linq;
using ChanganRestorationBureau;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ChanganProofSceneValidator
{
    public static void ValidateProofScene()
    {
        if (!System.IO.File.Exists("Assets/Scenes/AssetProofScene.unity"))
        {
            ChanganProofSceneBuilder.BuildAssetProofScene();
        }

        EditorSceneManager.OpenScene("Assets/Scenes/AssetProofScene.unity");

        var artifacts = Object.FindObjectsOfType<RestorationArtifact>();
        var slots = Object.FindObjectsOfType<ProofSlot>();
        var controller = Object.FindObjectOfType<AssetProofController>();
        var ui = Object.FindObjectOfType<ProofUIController>();

        if (artifacts.Length != 6)
        {
            throw new System.Exception($"Expected 6 artifacts, found {artifacts.Length}.");
        }

        if (slots.Count(s => s.slotType == ProofSlotType.DisplayCaseSlot) != 3)
        {
            throw new System.Exception("Expected exactly 3 display slots.");
        }

        if (controller == null || ui == null)
        {
            throw new System.Exception("Proof controller or UI controller is missing.");
        }

        foreach (var artifact in artifacts)
        {
            if (artifact.damagedSprite == null || artifact.repairedSprite == null)
            {
                throw new System.Exception($"{artifact.name} is missing damaged or repaired sprites.");
            }

            var collider = artifact.GetComponent<Collider2D>();
            if (collider == null || !collider.isTrigger)
            {
                throw new System.Exception($"{artifact.name} must have a trigger Collider2D.");
            }
        }

        Debug.Log("Changan proof scene validation passed.");
    }
}
