using System.IO;
using ChanganRestorationBureau;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ChanganVisualPreviewBuild
{
    private const string ScenePath = "Assets/Scenes/VisualPreviewScene.unity";
    private const string ProductionFolder = "Assets/Art/Generated/Production";

    public static void BuildWindowsVisualPreview()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Physics2D.gravity = Vector2.zero;

        var camera = CreateCamera();
        CreateSprite("Background_OpenMap", "background_open_map", Vector2.zero, 0.78f, -100, false);

        CreateSprite("Prop_GrassPatch_01", "prop_grass_patch", new Vector2(1.7f, -2.15f), 0.38f, 12, true);
        CreateSprite("Prop_RubbleStones", "prop_rubble_stones", new Vector2(3.15f, -2.2f), 0.38f, 12, true);
        CreateSprite("Prop_RelicPile", "prop_relic_pile", new Vector2(4.35f, -2.35f), 0.72f, 13, true);
        CreateSprite("Artifact_Terracotta", "artifact_terracotta_fragment_damaged", new Vector2(3.9f, -1.7f), 0.48f, 20, true);
        CreateSprite("Artifact_BronzeMirror", "artifact_bronze_mirror_damaged", new Vector2(-3.6f, -2.35f), 0.48f, 20, true);

        CreateSprite("Foreground_BureauEave", "foreground_bureau_eave", new Vector2(-4.2f, 2.05f), 0.64f, 72, true);
        CreateSprite("Foreground_TreeCanopy", "foreground_tree_canopy", new Vector2(1.15f, 1.75f), 0.58f, 78, true);
        CreateSprite("Foreground_MarketAwning", "foreground_market_awning", new Vector2(-3.75f, -1.55f), 0.58f, 74, true);
        CreateSprite("Foreground_WallEdge", "foreground_wall_edge", new Vector2(3.65f, 0.25f), 0.52f, 76, true);

        var player = CreatePlayer();
        var follow = camera.gameObject.AddComponent<ProofCameraFollow>();
        follow.target = player.transform;
        follow.min = new Vector2(-4.15f, -2.25f);
        follow.max = new Vector2(4.15f, 2.25f);

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);

        var absoluteBuildFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../Build/ChanganVisualPreview"));
        Directory.CreateDirectory(absoluteBuildFolder);
        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = Path.Combine(absoluteBuildFolder, "ChanganVisualPreview.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CleanBuildCache
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.Exception($"Visual preview build failed: {report.summary.result}");
        }

        Debug.Log($"Built visual preview: {options.locationPathName}");
    }

    private static Camera CreateCamera()
    {
        var cameraObject = new GameObject("Main Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4.4f;
        camera.backgroundColor = new Color32(33, 29, 28, 255);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.transform.position = new Vector3(0f, -0.2f, -10f);
        cameraObject.tag = "MainCamera";
        return camera;
    }

    private static GameObject CreateSprite(string name, string spriteName, Vector2 position, float scale, int order, bool ySort)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(spriteName);
        renderer.sortingOrder = order;
        if (ySort)
        {
            var sorter = obj.AddComponent<YSortRenderer>();
            sorter.offset = order;
        }
        return obj;
    }

    private static GameObject CreatePlayer()
    {
        var player = CreateSprite("Player_Keeper", "character_keeper_down", new Vector2(0f, -0.3f), 0.42f, 60, true);
        var body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        player.AddComponent<CapsuleCollider2D>().size = new Vector2(0.7f, 1.1f);
        var controller = player.AddComponent<ProofPlayer2D>();
        controller.downSprite = LoadSprite("character_keeper_down");
        controller.upSprite = LoadSprite("character_keeper_up");
        controller.leftSprite = LoadSprite("character_keeper_left");
        controller.rightSprite = LoadSprite("character_keeper_right");
        controller.minBounds = new Vector2(-9.6f, -5.1f);
        controller.maxBounds = new Vector2(9.6f, 5.1f);
        return player;
    }

    private static Sprite LoadSprite(string name)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{ProductionFolder}/{name}.png");
        if (sprite == null)
        {
            throw new System.Exception($"Missing preview sprite: {name}");
        }
        return sprite;
    }
}
