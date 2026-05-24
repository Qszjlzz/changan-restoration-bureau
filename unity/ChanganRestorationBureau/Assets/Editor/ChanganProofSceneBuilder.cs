using System.Collections.Generic;
using System.IO;
using ChanganRestorationBureau;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ChanganProofSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/AssetProofScene.unity";
    private const string PlaceholderFolder = "Assets/Art/Generated/Placeholders";
    private const string ProductionFolder = "Assets/Art/Generated/Production";

    private static readonly ArtifactSeed[] ArtifactSeeds =
    {
        new ArtifactSeed("artifact_terracotta_fragment", "陶俑残片", "秦汉风物", new Color32(172, 93, 66, 255), new Color32(206, 128, 84, 255), new Vector2(3.9f, -1.7f)),
        new ArtifactSeed("artifact_roof_tile", "莲纹瓦当", "汉唐建筑", new Color32(118, 96, 78, 255), new Color32(156, 121, 86, 255), new Vector2(4.9f, -2.0f)),
        new ArtifactSeed("artifact_bronze_mirror", "旧铜镜", "闺阁器用", new Color32(73, 112, 94, 255), new Color32(109, 146, 116, 255), new Vector2(-3.6f, -2.35f)),
        new ArtifactSeed("artifact_rubbing", "残碑拓片", "碑林文字", new Color32(214, 191, 148, 255), new Color32(239, 217, 174, 255), new Vector2(4.2f, 2.0f)),
        new ArtifactSeed("artifact_tangsancai", "唐三彩碎片", "盛唐釉色", new Color32(167, 103, 61, 255), new Color32(218, 148, 72, 255), new Vector2(-1.0f, 2.2f)),
        new ArtifactSeed("artifact_bamboo_slip", "旧木简", "关中旧档", new Color32(114, 75, 52, 255), new Color32(156, 102, 62, 255), new Vector2(0.5f, -0.25f))
    };

    [MenuItem("Changan/Build Asset Proof Scene")]
    public static void BuildAssetProofScene()
    {
        Directory.CreateDirectory(PlaceholderFolder);
        var sprites = GeneratePlaceholderSprites();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Physics2D.gravity = Vector2.zero;

        var camera = CreateCamera();
        CreateRoom(sprites);
        if (!HasProductionSprite("background_open_map"))
        {
            CreateLandmarks(sprites);
        }
        CreateFurniture(sprites);
        var player = CreatePlayer(sprites);
        var follow = camera.gameObject.AddComponent<ProofCameraFollow>();
        follow.target = player.transform;
        follow.min = new Vector2(-4.15f, -2.25f);
        follow.max = new Vector2(4.15f, 2.25f);
        CreateBoundaryWalls();

        var controller = new GameObject("AssetProofController").AddComponent<AssetProofController>();
        controller.worldCamera = camera;
        controller.workbenchSlot = CreateSlot("WorkbenchSlot", ProofSlotType.WorkbenchSlot, new Vector2(-4.15f, 1.25f), new Vector2(3f, 1f));

        controller.displaySlots.Add(CreateSlot("DisplaySlot_01", ProofSlotType.DisplayCaseSlot, new Vector2(-2.9f, 1.35f), Vector2.one));
        controller.displaySlots.Add(CreateSlot("DisplaySlot_02", ProofSlotType.DisplayCaseSlot, new Vector2(-2.05f, 1.35f), Vector2.one));
        controller.displaySlots.Add(CreateSlot("DisplaySlot_03", ProofSlotType.DisplayCaseSlot, new Vector2(-1.2f, 1.35f), Vector2.one));

        foreach (var seed in ArtifactSeeds)
        {
            controller.artifacts.Add(CreateArtifact(seed, sprites));
        }

        controller.ui = CreateUi(controller);
        var objective = CreateObjectiveUi(controller);
        objective.targetArtifact = controller.artifacts[0];
        ConfigureInteractions(player, controller, objective);
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log($"Built {ScenePath} with {controller.artifacts.Count} artifacts and {controller.displaySlots.Count} display slots.");
    }

    private static Dictionary<string, Sprite> GeneratePlaceholderSprites()
    {
        var output = new Dictionary<string, Sprite>();
        output["background_open_map"] = LoadProductionOrGenerate("background_open_map", 3072, 1792, new Color32(214, 182, 130, 255), SpritePivot.Center, TexturePattern.OpenMap);
        output["landmark_bureau"] = LoadProductionOrGenerate("landmark_bureau", 1024, 1024, new Color32(156, 91, 64, 255), SpritePivot.BottomCenter, TexturePattern.Bureau);
        output["landmark_stele_yard"] = LoadProductionOrGenerate("landmark_stele_yard", 1024, 1024, new Color32(93, 78, 61, 255), SpritePivot.BottomCenter, TexturePattern.SteleYard);
        output["landmark_market"] = LoadProductionOrGenerate("landmark_market", 1024, 1024, new Color32(139, 86, 62, 255), SpritePivot.BottomCenter, TexturePattern.Market);
        output["landmark_relic_yard"] = LoadProductionOrGenerate("landmark_relic_yard", 1024, 1024, new Color32(126, 105, 75, 255), SpritePivot.BottomCenter, TexturePattern.RelicYard);
        output["furniture_workbench"] = GenerateSprite("furniture_workbench", 512, 512, new Color32(126, 78, 48, 255), SpritePivot.BottomCenter, TexturePattern.Workbench);
        output["furniture_display_case"] = GenerateSprite("furniture_display_case", 512, 512, new Color32(95, 70, 50, 255), SpritePivot.BottomCenter, TexturePattern.DisplayCase);
        output["prop_grass_patch"] = LoadProductionOrGenerate("prop_grass_patch", 512, 512, new Color32(87, 126, 73, 255), SpritePivot.BottomCenter, TexturePattern.GrassPatch);
        output["prop_cleared_grass"] = LoadProductionOrGenerate("prop_cleared_grass", 512, 512, new Color32(169, 128, 74, 255), SpritePivot.BottomCenter, TexturePattern.GrassPatch);
        output["prop_rubble_stones"] = LoadProductionOrGenerate("prop_rubble_stones", 512, 512, new Color32(142, 120, 94, 255), SpritePivot.BottomCenter, TexturePattern.RelicPile);
        output["prop_relic_pile"] = LoadProductionOrGenerate("prop_relic_pile", 512, 512, new Color32(145, 101, 73, 255), SpritePivot.BottomCenter, TexturePattern.RelicPile);
        output["character_keeper_idle"] = LoadProductionOrGenerate("character_keeper_idle", 512, 512, new Color32(82, 60, 50, 255), SpritePivot.BottomCenter, TexturePattern.Character);
        output["character_keeper_down"] = LoadProductionOrGenerate("character_keeper_down", 512, 512, new Color32(82, 60, 50, 255), SpritePivot.BottomCenter, TexturePattern.Character);
        output["character_keeper_up"] = LoadProductionOrGenerate("character_keeper_up", 512, 512, new Color32(82, 60, 50, 255), SpritePivot.BottomCenter, TexturePattern.Character);
        output["character_keeper_left"] = LoadProductionOrGenerate("character_keeper_left", 512, 512, new Color32(82, 60, 50, 255), SpritePivot.BottomCenter, TexturePattern.Character);
        output["character_keeper_right"] = LoadProductionOrGenerate("character_keeper_right", 512, 512, new Color32(82, 60, 50, 255), SpritePivot.BottomCenter, TexturePattern.Character);

        foreach (var seed in ArtifactSeeds)
        {
            output[$"{seed.Id}_damaged"] = LoadProductionOrGenerate($"{seed.Id}_damaged", 256, 256, seed.DamagedColor, SpritePivot.Center, TexturePattern.ArtifactDamaged);
            output[$"{seed.Id}_repaired"] = LoadProductionOrGenerate($"{seed.Id}_repaired", 256, 256, seed.RepairedColor, SpritePivot.Center, TexturePattern.ArtifactRepaired);
        }

        return output;
    }

    private static bool HasProductionSprite(string name)
    {
        return File.Exists($"{ProductionFolder}/{name}.png");
    }

    private static Sprite LoadProductionOrGenerate(string name, int width, int height, Color32 color, SpritePivot pivot, TexturePattern pattern)
    {
        var productionPath = $"{ProductionFolder}/{name}.png";
        if (File.Exists(productionPath))
        {
            AssetDatabase.ImportAsset(productionPath);
            ConfigureSpriteImporter(productionPath, pivot, FilterMode.Bilinear);
            return AssetDatabase.LoadAssetAtPath<Sprite>(productionPath);
        }

        return GenerateSprite(name, width, height, color, pivot, pattern);
    }

    private static Sprite LoadProductionSprite(string name, SpritePivot pivot)
    {
        var productionPath = $"{ProductionFolder}/{name}.png";
        if (!File.Exists(productionPath))
        {
            return null;
        }

        AssetDatabase.ImportAsset(productionPath);
        ConfigureSpriteImporter(productionPath, pivot, FilterMode.Bilinear);
        return AssetDatabase.LoadAssetAtPath<Sprite>(productionPath);
    }

    private static Sprite GenerateSprite(string name, int width, int height, Color32 color, SpritePivot pivot, TexturePattern pattern)
    {
        var pixels = new Color32[width * height];
        var clear = new Color32(0, 0, 0, 0);
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = pattern == TexturePattern.OpenMap ? color : clear;
        }

        DrawPattern(pixels, width, height, color, pattern);
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply();

        var path = $"{PlaceholderFolder}/{name}.png";
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        ConfigureSpriteImporter(path, pivot, FilterMode.Point);

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void ConfigureSpriteImporter(string path, SpritePivot pivot, FilterMode filterMode)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.filterMode = filterMode;
        importer.spritePivot = PivotToVector(pivot);
        importer.SaveAndReimport();
    }

    private static void DrawPattern(Color32[] pixels, int width, int height, Color32 color, TexturePattern pattern)
    {
        var outline = new Color32(50, 38, 32, 255);
        switch (pattern)
        {
            case TexturePattern.OpenMap:
                FillRect(pixels, width, height, 0, 0, width, height, new Color32(210, 178, 124, 255));
                FillRect(pixels, width, height, 0, height / 2 - 80, width, 160, new Color32(232, 211, 169, 255));
                FillRect(pixels, width, height, width / 2 - 70, 0, 140, height, new Color32(226, 202, 158, 255));
                FillRect(pixels, width, height, width / 2 - 240, height / 2 - 210, 480, 420, new Color32(235, 214, 173, 255));
                break;
            case TexturePattern.Bureau:
                FillRect(pixels, width, height, 170, 250, 680, 350, outline);
                FillRect(pixels, width, height, 210, 290, 600, 250, color);
                FillRect(pixels, width, height, 360, 180, 300, 120, new Color32(228, 203, 154, 255));
                break;
            case TexturePattern.SteleYard:
                FillRect(pixels, width, height, 180, 210, 660, 320, outline);
                for (var i = 0; i < 5; i++)
                {
                    FillRect(pixels, width, height, 235 + i * 110, 285, 58, 180, new Color32(68, 61, 55, 255));
                }
                break;
            case TexturePattern.Market:
                FillRect(pixels, width, height, 185, 250, 650, 260, outline);
                FillRect(pixels, width, height, 225, 300, 570, 150, color);
                FillRect(pixels, width, height, 230, 450, 560, 70, new Color32(187, 71, 58, 255));
                break;
            case TexturePattern.RelicYard:
                FillRect(pixels, width, height, 180, 250, 660, 250, outline);
                FillRect(pixels, width, height, 220, 290, 580, 150, color);
                FillEllipse(pixels, width, height, 360, 370, 55, 35, new Color32(172, 112, 76, 255));
                FillEllipse(pixels, width, height, 560, 360, 65, 38, new Color32(112, 94, 78, 255));
                break;
            case TexturePattern.GrassPatch:
                for (var i = 0; i < 12; i++)
                {
                    var x = 120 + i * 24;
                    FillRect(pixels, width, height, x, 120, 14, 150 + (i % 3) * 24, outline);
                    FillRect(pixels, width, height, x + 4, 128, 8, 130 + (i % 3) * 22, color);
                }
                break;
            case TexturePattern.RelicPile:
                FillEllipse(pixels, width, height, 190, 170, 70, 38, outline);
                FillEllipse(pixels, width, height, 190, 170, 56, 28, color);
                FillEllipse(pixels, width, height, 310, 190, 82, 44, outline);
                FillEllipse(pixels, width, height, 310, 190, 64, 32, new Color32(120, 92, 78, 255));
                break;
            case TexturePattern.Workbench:
                FillRect(pixels, width, height, 40, 150, width - 80, 150, outline);
                FillRect(pixels, width, height, 62, 175, width - 124, 95, color);
                break;
            case TexturePattern.DisplayCase:
                FillRect(pixels, width, height, 55, 145, width - 110, 185, outline);
                FillRect(pixels, width, height, 80, 175, width - 160, 120, new Color32(177, 136, 78, 255));
                break;
            case TexturePattern.Character:
                FillEllipse(pixels, width, height, width / 2, 245, 58, 58, outline);
                FillEllipse(pixels, width, height, width / 2, 245, 42, 42, new Color32(216, 171, 123, 255));
                FillRect(pixels, width, height, 205, 70, 102, 145, outline);
                FillRect(pixels, width, height, 220, 90, 72, 110, color);
                break;
            case TexturePattern.ArtifactDamaged:
                FillEllipse(pixels, width, height, width / 2, height / 2, 70, 50, outline);
                FillEllipse(pixels, width, height, width / 2, height / 2, 56, 38, color);
                FillRect(pixels, width, height, width / 2 + 10, height / 2 - 10, 46, 16, new Color32(0, 0, 0, 0), clear: true);
                break;
            case TexturePattern.ArtifactRepaired:
                FillEllipse(pixels, width, height, width / 2, height / 2, 72, 52, outline);
                FillEllipse(pixels, width, height, width / 2, height / 2, 58, 40, color);
                FillEllipse(pixels, width, height, width / 2, height / 2, 20, 14, new Color32(236, 205, 122, 255));
                break;
        }
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

    private static void CreateRoom(Dictionary<string, Sprite> sprites)
    {
        var room = new GameObject("Background_OpenMap");
        var renderer = room.AddComponent<SpriteRenderer>();
        renderer.sprite = sprites["background_open_map"];
        renderer.sortingOrder = -100;
        room.transform.localScale = new Vector3(0.78f, 0.78f, 1f);
    }

    private static void CreateLandmarks(Dictionary<string, Sprite> sprites)
    {
        CreateLandmarkObject("Landmark_ChanganRestorationBureau", sprites["landmark_bureau"], new Vector2(-3.25f, 1.6f), 4, 0.55f);
        CreateLandmarkObject("Landmark_SteleYard", sprites["landmark_stele_yard"], new Vector2(4.35f, 2.35f), 3, 0.48f);
        CreateLandmarkObject("Landmark_NightMarket", sprites["landmark_market"], new Vector2(-3.5f, -2.35f), 3, 0.48f);
        CreateLandmarkObject("Landmark_RelicYard", sprites["landmark_relic_yard"], new Vector2(4.35f, -1.8f), 3, 0.48f);
    }

    private static void CreateLandmarkObject(string name, Sprite sprite, Vector2 position, int order, float scale)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
    }

    private static void CreateFurniture(Dictionary<string, Sprite> sprites)
    {
        CreateFurnitureObject("Furniture_Workbench", sprites["furniture_workbench"], new Vector2(-4.15f, 1.05f), 18);
        CreateFurnitureObject("Furniture_DisplayCase", sprites["furniture_display_case"], new Vector2(-2.05f, 1.0f), 16);
        CreateCleanupProp("Prop_GrassPatch_01", sprites["prop_grass_patch"], sprites["prop_cleared_grass"], new Vector2(1.7f, -2.15f), 12, "按 E 清理荒草");
        CreateCleanupProp("Prop_GrassPatch_02", sprites["prop_grass_patch"], sprites["prop_cleared_grass"], new Vector2(2.35f, -2.55f), 12, "按 E 清理荒草");
        CreateCleanupProp("Prop_RubbleStones", sprites["prop_rubble_stones"], null, new Vector2(3.15f, -2.2f), 12, "按 E 清理碎石");
        CreateFurnitureObject("Prop_RelicPile", sprites["prop_relic_pile"], new Vector2(4.35f, -2.35f), 13);
    }

    private static void CreateFurnitureObject(string name, Sprite sprite, Vector2 position, int order)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * 0.82f;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
        var sorter = obj.AddComponent<YSortRenderer>();
        sorter.offset = order;
    }

    private static void CreateCleanupProp(string name, Sprite sprite, Sprite clearedSprite, Vector2 position, int order, string prompt)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * 0.38f;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
        var sorter = obj.AddComponent<YSortRenderer>();
        sorter.offset = order;
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1.1f, 0.7f);
        var cleanup = obj.AddComponent<MapCleanupInteractable>();
        cleanup.clearedSprite = clearedSprite;
        var target = obj.AddComponent<InteractionTarget>();
        target.kind = InteractionKind.Cleanup;
        target.prompt = prompt;
        target.cleanup = cleanup;
    }

    private static GameObject CreatePlayer(Dictionary<string, Sprite> sprites)
    {
        var player = new GameObject("Player_Keeper");
        player.transform.position = new Vector3(0f, -0.3f, 0f);
        player.transform.localScale = Vector3.one * 0.42f;
        var renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = sprites["character_keeper_down"];
        renderer.sortingOrder = 40;
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<CapsuleCollider2D>().size = new Vector2(0.7f, 1.1f);
        var playerController = player.AddComponent<ProofPlayer2D>();
        playerController.downSprite = sprites["character_keeper_down"];
        playerController.upSprite = sprites["character_keeper_up"];
        playerController.leftSprite = sprites["character_keeper_left"];
        playerController.rightSprite = sprites["character_keeper_right"];
        playerController.minBounds = new Vector2(-9.6f, -5.1f);
        playerController.maxBounds = new Vector2(9.6f, 5.1f);
        var sorter = player.AddComponent<YSortRenderer>();
        sorter.offset = 60;
        return player;
    }

    private static RestorationArtifact CreateArtifact(ArtifactSeed seed, Dictionary<string, Sprite> sprites)
    {
        var obj = new GameObject(seed.Id);
        obj.transform.position = seed.Position;
        obj.transform.localScale = Vector3.one * 0.48f;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprites[$"{seed.Id}_damaged"];
        renderer.sortingOrder = 20;
        var collider = obj.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.55f;
        var target = obj.AddComponent<InteractionTarget>();
        target.kind = InteractionKind.Sample;
        target.prompt = "按 E 采样文物";

        var artifact = obj.AddComponent<RestorationArtifact>();
        artifact.artifactId = seed.Id;
        artifact.displayName = seed.DisplayName;
        artifact.eraTag = seed.EraTag;
        artifact.damagedSprite = sprites[$"{seed.Id}_damaged"];
        artifact.repairedSprite = sprites[$"{seed.Id}_repaired"];
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
        target.prompt = type == ProofSlotType.WorkbenchSlot ? "按 E 修复文物" : "按 E 陈列文物";
        target.slot = slot;
        return slot;
    }

    private static void ConfigureInteractions(GameObject player, AssetProofController controller, ProofObjectiveState objective)
    {
        var interaction = player.AddComponent<InteractionController>();
        interaction.objective = objective;
        interaction.proofController = controller;

        foreach (var artifact in controller.artifacts)
        {
            var target = artifact.GetComponent<InteractionTarget>();
            if (target != null)
            {
                target.artifact = artifact;
            }
        }
    }

    private static void CreateBoundaryWalls()
    {
        const float minX = -10.5f;
        const float maxX = 10.5f;
        const float minY = -5.8f;
        const float maxY = 5.8f;
        CreateBoundaryWall("Boundary_Left", new Vector2(minX - 0.25f, 0f), new Vector2(0.5f, 12.4f));
        CreateBoundaryWall("Boundary_Right", new Vector2(maxX + 0.25f, 0f), new Vector2(0.5f, 12.4f));
        CreateBoundaryWall("Boundary_Top", new Vector2(0f, maxY + 0.25f), new Vector2(21.6f, 0.5f));
        CreateBoundaryWall("Boundary_Bottom", new Vector2(0f, minY - 0.25f), new Vector2(21.6f, 0.5f));
    }

    private static void CreateBoundaryWall(string name, Vector2 position, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.size = size;
    }

    private static ProofObjectiveState CreateObjectiveUi(AssetProofController controller)
    {
        var canvas = controller.ui.GetComponent<Canvas>().transform;
        var objectiveText = CreateText("ObjectiveText", canvas, new Vector2(-420, 295), new Vector2(420, 70), 18, TextAnchor.MiddleLeft);
        objectiveText.text = "目标：清理荒草，寻找待修物";
        var hintText = CreateText("InteractionHint", canvas, new Vector2(0, -270), new Vector2(360, 48), 22, TextAnchor.MiddleCenter);
        hintText.color = new Color32(78, 47, 28, 255);
        hintText.enabled = false;

        var objective = controller.gameObject.AddComponent<ProofObjectiveState>();
        objective.objectiveText = objectiveText;
        objective.hintText = hintText;
        return objective;
    }

    private static ProofUIController CreateUi(AssetProofController controller)
    {
        var canvasObject = new GameObject("ProofCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);
        canvasObject.AddComponent<GraphicRaycaster>();

        var panelSprite = LoadProductionSprite("ui_detail_panel", SpritePivot.Center);
        var repairSprite = LoadProductionSprite("ui_repair_button", SpritePivot.Center);
        var displaySprite = LoadProductionSprite("ui_display_button", SpritePivot.Center);

        var panel = CreateUiRect("DetailPanel", canvasObject.transform, new Vector2(480, -25), new Vector2(270, 405), new Color32(238, 216, 170, 226));
        var panelImage = panel.GetComponent<Image>();
        if (panelSprite != null)
        {
            panelImage.sprite = panelSprite;
            panelImage.type = Image.Type.Simple;
            panelImage.color = Color.white;
        }

        var icon = CreateImage("ArtifactIcon", panel.transform, new Vector2(0, 105), new Vector2(78, 78), new Color32(255, 255, 255, 210));
        var title = CreateText("TitleText", panel.transform, new Vector2(0, 42), new Vector2(220, 42), 22, TextAnchor.MiddleCenter);
        var era = CreateText("EraText", panel.transform, new Vector2(0, -4), new Vector2(220, 52), 16, TextAnchor.MiddleCenter);
        var state = CreateText("StateText", panel.transform, new Vector2(0, -58), new Vector2(220, 34), 17, TextAnchor.MiddleCenter);
        var repairButton = CreateButton("RepairButton", panel.transform, new Vector2(-58, -135), new Vector2(98, 42), "修复", repairSprite);
        var displayButton = CreateButton("DisplayButton", panel.transform, new Vector2(58, -135), new Vector2(98, 42), "陈列", displaySprite);

        var hint = CreateText("HintText", canvasObject.transform, new Vector2(-250, 320), new Vector2(650, 40), 20, TextAnchor.MiddleLeft);
        hint.text = "WASD 移动｜靠近后按 E 交互｜点击或按 1-6 可调试选择";

        var ui = canvasObject.AddComponent<ProofUIController>();
        ui.titleText = title;
        ui.eraText = era;
        ui.stateText = state;
        ui.iconImage = icon;
        ui.repairButton = repairButton;
        ui.displayButton = displayButton;
        return ui;
    }

    private static GameObject CreateUiRect(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, Color32 color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        var image = obj.AddComponent<Image>();
        image.color = color;
        return obj;
    }

    private static Image CreateImage(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, Color32 color)
    {
        var obj = CreateUiRect(name, parent, anchoredPosition, size, color);
        return obj.GetComponent<Image>();
    }

    private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        var text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color32(61, 42, 32, 255);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static Button CreateButton(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, string label, Sprite sprite = null)
    {
        var obj = CreateUiRect(name, parent, anchoredPosition, size, new Color32(125, 75, 51, 255));
        var button = obj.AddComponent<Button>();
        var image = obj.GetComponent<Image>();
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
        }

        button.targetGraphic = image;
        var text = CreateText($"{name}_Text", obj.transform, Vector2.zero, size, 18, TextAnchor.MiddleCenter);
        text.text = label;
        text.color = new Color32(249, 229, 181, 255);
        return button;
    }

    private static void CreateEventSystem()
    {
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static void FillRect(Color32[] pixels, int width, int height, int x, int y, int rectWidth, int rectHeight, Color32 color, bool clear = false)
    {
        var fill = clear ? new Color32(0, 0, 0, 0) : color;
        for (var py = Mathf.Max(0, y); py < Mathf.Min(height, y + rectHeight); py++)
        {
            for (var px = Mathf.Max(0, x); px < Mathf.Min(width, x + rectWidth); px++)
            {
                pixels[py * width + px] = fill;
            }
        }
    }

    private static void FillEllipse(Color32[] pixels, int width, int height, int cx, int cy, int rx, int ry, Color32 color)
    {
        for (var y = -ry; y <= ry; y++)
        {
            for (var x = -rx; x <= rx; x++)
            {
                if (x * x * ry * ry + y * y * rx * rx > rx * rx * ry * ry)
                {
                    continue;
                }

                var px = cx + x;
                var py = cy + y;
                if (px >= 0 && px < width && py >= 0 && py < height)
                {
                    pixels[py * width + px] = color;
                }
            }
        }
    }

    private static Vector2 PivotToVector(SpritePivot pivot)
    {
        return pivot == SpritePivot.BottomCenter ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f);
    }

    private readonly struct ArtifactSeed
    {
        public readonly string Id;
        public readonly string DisplayName;
        public readonly string EraTag;
        public readonly Color32 DamagedColor;
        public readonly Color32 RepairedColor;
        public readonly Vector2 Position;

        public ArtifactSeed(string id, string displayName, string eraTag, Color32 damagedColor, Color32 repairedColor, Vector2 position)
        {
            Id = id;
            DisplayName = displayName;
            EraTag = eraTag;
            DamagedColor = damagedColor;
            RepairedColor = repairedColor;
            Position = position;
        }
    }

    private enum SpritePivot
    {
        Center,
        BottomCenter
    }

    private enum TexturePattern
    {
        OpenMap,
        Bureau,
        SteleYard,
        Market,
        RelicYard,
        Workbench,
        DisplayCase,
        GrassPatch,
        RelicPile,
        Character,
        ArtifactDamaged,
        ArtifactRepaired
    }
}
