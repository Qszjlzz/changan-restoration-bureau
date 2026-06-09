using System.Collections.Generic;
using System.IO;
using ChanganRestorationBureau;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ChanganPlayableProofSceneBuilder
{
    public const string ScenePath = "Assets/Scenes/PlayableProofScene.unity";
    private const string PlaceholderFolder = "Assets/Art/Generated/Placeholders";
    private const string ProductionFolder = "Assets/Art/Generated/Production";

    private static readonly ArtifactSeed[] ArtifactSeeds =
    {
        new ArtifactSeed("artifact_terracotta_fragment", "Terracotta Fragment", "Qin-Han", new Vector2(3.9f, -1.7f)),
        new ArtifactSeed("artifact_roof_tile", "Lotus Roof Tile", "Han-Tang", new Vector2(4.9f, -2.0f)),
        new ArtifactSeed("artifact_bronze_mirror", "Bronze Mirror", "Household Relic", new Vector2(-3.6f, -2.35f)),
        new ArtifactSeed("artifact_rubbing", "Stele Rubbing", "Stone Text", new Vector2(4.2f, 2.0f)),
        new ArtifactSeed("artifact_tangsancai", "Tang Sancai Shard", "Prosperous Tang", new Vector2(-1.0f, 2.2f)),
        new ArtifactSeed("artifact_bamboo_slip", "Old Bamboo Slip", "Archive Note", new Vector2(0.5f, -0.25f))
    };

    [MenuItem("Changan/Build Playable Proof Scene")]
    public static void BuildScene()
    {
        Directory.CreateDirectory(PlaceholderFolder);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Physics2D.gravity = Vector2.zero;

        var camera = CreateCamera();
        CreateBackground();
        CreateFurniture();
        CreateForegrounds();
        var player = CreatePlayer();
        CreateBoundaryWalls();

        var controller = new GameObject("PlayableProofController").AddComponent<AssetProofController>();
        controller.worldCamera = camera;
        controller.workbenchSlot = CreateSlot("WorkbenchSlot", ProofSlotType.WorkbenchSlot, new Vector2(-4.15f, 1.25f), new Vector2(3f, 1f));
        controller.displaySlots.Add(CreateSlot("DisplaySlot_01", ProofSlotType.DisplayCaseSlot, new Vector2(-2.9f, 1.35f), Vector2.one));
        controller.displaySlots.Add(CreateSlot("DisplaySlot_02", ProofSlotType.DisplayCaseSlot, new Vector2(-2.05f, 1.35f), Vector2.one));
        controller.displaySlots.Add(CreateSlot("DisplaySlot_03", ProofSlotType.DisplayCaseSlot, new Vector2(-1.2f, 1.35f), Vector2.one));

        foreach (var seed in ArtifactSeeds)
        {
            controller.artifacts.Add(CreateArtifact(seed));
        }

        var interaction = player.AddComponent<InteractionController>();
        interaction.proofController = controller;
        foreach (var artifact in controller.artifacts)
        {
            var target = artifact.GetComponent<InteractionTarget>();
            if (target != null)
            {
                target.artifact = artifact;
            }
        }

        var bootstrap = controller.gameObject.AddComponent<ProofRuntimeBootstrap>();
        bootstrap.player = player;
        bootstrap.uiPanelSprite = LoadOptionalProductionSprite("ui_detail_panel");
        bootstrap.repairButtonSprite = LoadOptionalProductionSprite("ui_repair_button");
        bootstrap.displayButtonSprite = LoadOptionalProductionSprite("ui_display_button");

        var follow = camera.gameObject.AddComponent<ProofCameraFollow>();
        follow.target = player.transform;
        follow.min = new Vector2(-4.15f, -2.25f);
        follow.max = new Vector2(4.15f, 2.25f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log($"Built {ScenePath} with {controller.artifacts.Count} artifacts.");
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

    private static void CreateBackground()
    {
        var obj = new GameObject("Background_OpenMap");
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadRequiredProductionSprite("background_open_map", SpritePivot.Center);
        renderer.sortingOrder = -100;
        obj.transform.localScale = new Vector3(0.78f, 0.78f, 1f);
    }

    private static void CreateFurniture()
    {
        CreateProp("Furniture_Workbench", LoadPlaceholder("furniture_workbench", 512, 512, new Color32(126, 78, 48, 255), PlaceholderPattern.Workbench), new Vector2(-4.15f, 1.05f), 0.82f, 18);
        CreateProp("Furniture_DisplayCase", LoadPlaceholder("furniture_display_case", 512, 512, new Color32(95, 70, 50, 255), PlaceholderPattern.DisplayCase), new Vector2(-2.05f, 1.0f), 0.82f, 16);
        CreateCleanupProp("Prop_GrassPatch_01", "prop_grass_patch", "prop_cleared_grass", new Vector2(1.7f, -2.15f), "Press E to clear grass");
        CreateCleanupProp("Prop_GrassPatch_02", "prop_grass_patch", "prop_cleared_grass", new Vector2(2.35f, -2.55f), "Press E to clear grass");
        CreateCleanupProp("Prop_RubbleStones", "prop_rubble_stones", null, new Vector2(3.15f, -2.2f), "Press E to clear rubble");
        CreateProp("Prop_RelicPile", LoadRequiredProductionSprite("prop_relic_pile", SpritePivot.BottomCenter), new Vector2(4.35f, -2.35f), 0.72f, 13);
    }

    private static void CreateForegrounds()
    {
        CreateProp("Foreground_BureauEave", LoadRequiredProductionSprite("foreground_bureau_eave", SpritePivot.BottomCenter), new Vector2(-4.2f, 2.05f), 0.64f, 72);
        CreateProp("Foreground_TreeCanopy", LoadRequiredProductionSprite("foreground_tree_canopy", SpritePivot.BottomCenter), new Vector2(1.15f, 1.75f), 0.58f, 78);
        CreateProp("Foreground_MarketAwning", LoadRequiredProductionSprite("foreground_market_awning", SpritePivot.BottomCenter), new Vector2(-3.75f, -1.55f), 0.58f, 74);
        CreateProp("Foreground_WallEdge", LoadRequiredProductionSprite("foreground_wall_edge", SpritePivot.BottomCenter), new Vector2(3.65f, 0.25f), 0.52f, 76);
    }

    private static GameObject CreatePlayer()
    {
        var player = new GameObject("Player_Keeper");
        player.transform.position = new Vector3(0f, -0.3f, 0f);
        player.transform.localScale = Vector3.one * 0.42f;
        var renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadRequiredProductionSprite("character_keeper_down", SpritePivot.BottomCenter);
        renderer.sortingOrder = 40;
        var body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        player.AddComponent<CapsuleCollider2D>().size = new Vector2(0.7f, 1.1f);
        var controller = player.AddComponent<ProofPlayer2D>();
        controller.downSprite = LoadRequiredProductionSprite("character_keeper_down", SpritePivot.BottomCenter);
        controller.upSprite = LoadRequiredProductionSprite("character_keeper_up", SpritePivot.BottomCenter);
        controller.leftSprite = LoadRequiredProductionSprite("character_keeper_left", SpritePivot.BottomCenter);
        controller.rightSprite = LoadRequiredProductionSprite("character_keeper_right", SpritePivot.BottomCenter);
        controller.minBounds = new Vector2(-9.6f, -5.1f);
        controller.maxBounds = new Vector2(9.6f, 5.1f);
        var sorter = player.AddComponent<YSortRenderer>();
        sorter.offset = 60;
        return player;
    }

    private static void CreateBoundaryWalls()
    {
        CreateBoundaryWall("Boundary_Left", new Vector2(-10.75f, 0f), new Vector2(0.5f, 12.4f));
        CreateBoundaryWall("Boundary_Right", new Vector2(10.75f, 0f), new Vector2(0.5f, 12.4f));
        CreateBoundaryWall("Boundary_Top", new Vector2(0f, 6.05f), new Vector2(21.6f, 0.5f));
        CreateBoundaryWall("Boundary_Bottom", new Vector2(0f, -6.05f), new Vector2(21.6f, 0.5f));
    }

    private static void CreateBoundaryWall(string name, Vector2 position, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.size = size;
    }

    private static RestorationArtifact CreateArtifact(ArtifactSeed seed)
    {
        var obj = new GameObject(seed.Id);
        obj.transform.position = seed.Position;
        obj.transform.localScale = Vector3.one * 0.48f;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadRequiredProductionSprite($"{seed.Id}_damaged", SpritePivot.Center);
        renderer.sortingOrder = 20;
        var collider = obj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.55f;
        var artifact = obj.AddComponent<RestorationArtifact>();
        artifact.artifactId = seed.Id;
        artifact.displayName = seed.DisplayName;
        artifact.eraTag = seed.EraTag;
        artifact.damagedSprite = LoadRequiredProductionSprite($"{seed.Id}_damaged", SpritePivot.Center);
        artifact.repairedSprite = LoadRequiredProductionSprite($"{seed.Id}_repaired", SpritePivot.Center);
        var target = obj.AddComponent<InteractionTarget>();
        target.kind = InteractionKind.Sample;
        target.prompt = "Press E to sample relic";
        target.artifact = artifact;
        var sorter = obj.AddComponent<YSortRenderer>();
        sorter.offset = 20;
        return artifact;
    }

    private static ProofSlot CreateSlot(string name, ProofSlotType type, Vector2 position, Vector2 sizeUnits)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        var slot = obj.AddComponent<ProofSlot>();
        slot.slotId = name;
        slot.slotType = type;
        slot.sizeUnits = sizeUnits;
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = sizeUnits;
        var target = obj.AddComponent<InteractionTarget>();
        target.kind = type == ProofSlotType.WorkbenchSlot ? InteractionKind.Repair : InteractionKind.Display;
        target.prompt = type == ProofSlotType.WorkbenchSlot ? "Press E to repair relic" : "Press E to display relic";
        target.slot = slot;
        return slot;
    }

    private static void CreateCleanupProp(string name, string spriteName, string clearedSpriteName, Vector2 position, string prompt)
    {
        var obj = CreateProp(name, LoadRequiredProductionSprite(spriteName, SpritePivot.BottomCenter), position, 0.38f, 12);
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1.1f, 0.7f);
        var cleanup = obj.AddComponent<MapCleanupInteractable>();
        cleanup.clearedSprite = string.IsNullOrEmpty(clearedSpriteName) ? null : LoadRequiredProductionSprite(clearedSpriteName, SpritePivot.BottomCenter);
        var target = obj.AddComponent<InteractionTarget>();
        target.kind = InteractionKind.Cleanup;
        target.prompt = prompt;
        target.cleanup = cleanup;
    }

    private static GameObject CreateProp(string name, Sprite sprite, Vector2 position, float scale, int order)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
        var sorter = obj.AddComponent<YSortRenderer>();
        sorter.offset = order;
        return obj;
    }

    private static Sprite LoadRequiredProductionSprite(string name, SpritePivot pivot)
    {
        var sprite = LoadProductionSprite(name, pivot);
        if (sprite == null)
        {
            throw new System.Exception($"Missing production sprite: {name}");
        }
        return sprite;
    }

    private static Sprite LoadOptionalProductionSprite(string name)
    {
        return LoadProductionSprite(name, SpritePivot.Center);
    }

    private static Sprite LoadProductionSprite(string name, SpritePivot pivot)
    {
        var path = $"{ProductionFolder}/{name}.png";
        if (!File.Exists(path))
        {
            return null;
        }

        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.filterMode = FilterMode.Bilinear;
        importer.spritePivot = pivot == SpritePivot.BottomCenter ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f);
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static Sprite LoadPlaceholder(string name, int width, int height, Color32 color, PlaceholderPattern pattern)
    {
        var path = $"{PlaceholderFolder}/{name}.png";
        if (!File.Exists(path))
        {
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 0);
            }

            if (pattern == PlaceholderPattern.Workbench)
            {
                FillRect(pixels, width, height, 40, 150, width - 80, 150, new Color32(50, 38, 32, 255));
                FillRect(pixels, width, height, 62, 175, width - 124, 95, color);
            }
            else
            {
                FillRect(pixels, width, height, 55, 145, width - 110, 185, new Color32(50, 38, 32, 255));
                FillRect(pixels, width, height, 80, 175, width - 160, 120, new Color32(177, 136, 78, 255));
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }

        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.filterMode = FilterMode.Point;
        importer.spritePivot = new Vector2(0.5f, 0f);
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void FillRect(Color32[] pixels, int width, int height, int x, int y, int rectWidth, int rectHeight, Color32 color)
    {
        for (var py = Mathf.Max(0, y); py < Mathf.Min(height, y + rectHeight); py++)
        {
            for (var px = Mathf.Max(0, x); px < Mathf.Min(width, x + rectWidth); px++)
            {
                pixels[py * width + px] = color;
            }
        }
    }

    private readonly struct ArtifactSeed
    {
        public readonly string Id;
        public readonly string DisplayName;
        public readonly string EraTag;
        public readonly Vector2 Position;

        public ArtifactSeed(string id, string displayName, string eraTag, Vector2 position)
        {
            Id = id;
            DisplayName = displayName;
            EraTag = eraTag;
            Position = position;
        }
    }

    private enum SpritePivot
    {
        Center,
        BottomCenter
    }

    private enum PlaceholderPattern
    {
        Workbench,
        DisplayCase
    }
}
