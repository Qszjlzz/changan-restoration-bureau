using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChanganRestorationBureau
{
    public sealed class ProofRuntimeBootstrap : MonoBehaviour
    {
        public GameObject player;
        public Sprite uiPanelSprite;
        public Sprite repairButtonSprite;
        public Sprite displayButtonSprite;

        private void Awake()
        {
            Debug.Log("[Changan] ProofRuntimeBootstrap.Awake begin");
            var controller = GetComponent<AssetProofController>();
            if (controller == null)
            {
                Debug.LogError("ProofRuntimeBootstrap requires AssetProofController.");
                enabled = false;
                return;
            }

            EnsureEventSystem();
            var ui = BuildUi();
            controller.ui = ui;
            Debug.Log("[Changan] ProofRuntimeBootstrap built UI");

            var dayState = controller.GetComponent<ProofDayState>();
            var dialogue = BuildDialogueUi(ui.transform.parent);

            var objective = controller.gameObject.AddComponent<ProofObjectiveState>();
            objective.objectiveText = CreateText("ObjectiveText", ui.transform, new Vector2(-420f, 295f), new Vector2(420f, 70f), 18, TextAnchor.MiddleLeft);
            objective.hintText = CreateText("InteractionHint", ui.transform, new Vector2(0f, -270f), new Vector2(360f, 48f), 22, TextAnchor.MiddleCenter);
            objective.hintText.color = new Color32(78, 47, 28, 255);
            objective.hintText.enabled = false;
            objective.targetArtifact = controller.artifacts.Find(artifact => artifact.artifactId == "artifact_roof_tile");
            if (objective.targetArtifact == null && controller.artifacts.Count > 0)
            {
                objective.targetArtifact = controller.artifacts[0];
            }
            objective.dayState = dayState;
            var restorationChoice = BuildRestorationChoiceUi(ui.transform.parent);
            var resourceHud = BuildResourceHud(ui.transform.parent);
            resourceHud.Bind(dayState);
            restorationChoice.resourceHud = resourceHud;
            restorationChoice.Bind(controller, dayState, objective);

            var guide = CreateText("HintText", ui.transform, new Vector2(-250f, 320f), new Vector2(650f, 40f), 20, TextAnchor.MiddleLeft);
            guide.text = "WASD move | E interact";

            var interaction = player != null ? player.GetComponent<InteractionController>() : null;
            if (interaction != null)
            {
                interaction.proofController = controller;
                interaction.objective = objective;
                interaction.dayState = dayState;
                interaction.dialogue = dialogue;
                interaction.restorationChoice = restorationChoice;
                interaction.resourceHud = resourceHud;
            }

            foreach (var artifact in controller.artifacts)
            {
                var target = artifact.GetComponent<InteractionTarget>();
                if (target != null)
                {
                    target.artifact = artifact;
                }
            }

            var smokeAutoplay = controller.GetComponent<ProofSmokeAutoplay>();
            if (smokeAutoplay != null)
            {
                smokeAutoplay.dayState = dayState;
                smokeAutoplay.restorationChoice = restorationChoice;
            }

            Debug.Log("[Changan] ProofRuntimeBootstrap.Awake complete");
        }

        private ProofUIController BuildUi()
        {
            var canvasObject = new GameObject("ProofCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            canvasObject.AddComponent<GraphicRaycaster>();

            var panel = CreateUiRect("DetailPanel", canvasObject.transform, new Vector2(480f, -25f), new Vector2(270f, 405f), new Color32(238, 216, 170, 226));
            var panelImage = panel.GetComponent<Image>();
            if (uiPanelSprite != null)
            {
                panelImage.sprite = uiPanelSprite;
                panelImage.type = Image.Type.Simple;
                panelImage.color = Color.white;
            }

            var ui = canvasObject.AddComponent<ProofUIController>();
            ui.iconImage = CreateImage("ArtifactIcon", panel.transform, new Vector2(0f, 105f), new Vector2(78f, 78f), new Color32(255, 255, 255, 210));
            ui.titleText = CreateText("TitleText", panel.transform, new Vector2(0f, 42f), new Vector2(220f, 42f), 22, TextAnchor.MiddleCenter);
            ui.eraText = CreateText("EraText", panel.transform, new Vector2(0f, -4f), new Vector2(220f, 52f), 16, TextAnchor.MiddleCenter);
            ui.stateText = CreateText("StateText", panel.transform, new Vector2(0f, -58f), new Vector2(220f, 34f), 17, TextAnchor.MiddleCenter);
            ui.repairButton = CreateButton("RepairButton", panel.transform, new Vector2(-58f, -135f), new Vector2(98f, 42f), "Repair", repairButtonSprite);
            ui.displayButton = CreateButton("DisplayButton", panel.transform, new Vector2(58f, -135f), new Vector2(98f, 42f), "Display", displayButtonSprite);
            return ui;
        }

        private static ProofDialogueController BuildDialogueUi(Transform parent)
        {
            var panel = CreateUiRect("DialoguePanel", parent, new Vector2(0f, -235f), new Vector2(1120f, 170f), new Color32(44, 31, 26, 232));
            panel.SetActive(false);
            var controller = panel.AddComponent<ProofDialogueController>();
            controller.panelRoot = panel;
            controller.speakerText = CreateText("DialogueSpeaker", panel.transform, new Vector2(-430f, 48f), new Vector2(260f, 34f), 24, TextAnchor.MiddleLeft);
            controller.speakerText.color = new Color32(244, 222, 170, 255);
            controller.bodyText = CreateText("DialogueBody", panel.transform, new Vector2(0f, -2f), new Vector2(760f, 88f), 22, TextAnchor.MiddleLeft);
            controller.footerText = CreateText("DialogueFooter", panel.transform, new Vector2(380f, 56f), new Vector2(300f, 30f), 16, TextAnchor.MiddleRight);
            controller.footerText.color = new Color32(214, 197, 168, 255);
            controller.portraitImage = CreateImage("DialoguePortrait", panel.transform, new Vector2(-470f, -4f), new Vector2(112f, 112f), new Color32(255, 255, 255, 210));
            controller.portraitImage.enabled = false;
            return controller;
        }

        private static ProofResourceHudController BuildResourceHud(Transform parent)
        {
            var panel = CreateUiRect("ResourceHudPanel", parent, new Vector2(385f, 260f), new Vector2(310f, 132f), new Color32(243, 232, 209, 220));
            var controller = panel.AddComponent<ProofResourceHudController>();
            controller.budgetText = CreateText("BudgetText", panel.transform, new Vector2(0f, 18f), new Vector2(270f, 88f), 15, TextAnchor.MiddleLeft);
            controller.feedbackText = CreateText("BudgetFeedback", panel.transform, new Vector2(0f, -44f), new Vector2(270f, 30f), 14, TextAnchor.MiddleLeft);
            controller.feedbackText.enabled = false;
            return controller;
        }

        private static ProofRestorationChoiceController BuildRestorationChoiceUi(Transform parent)
        {
            var panel = CreateUiRect("RestorationChoicePanel", parent, new Vector2(0f, 35f), new Vector2(560f, 270f), new Color32(53, 40, 32, 238));
            panel.SetActive(false);
            var controller = panel.AddComponent<ProofRestorationChoiceController>();
            controller.panelRoot = panel;
            controller.titleText = CreateText("ChoiceTitle", panel.transform, new Vector2(0f, 95f), new Vector2(460f, 34f), 24, TextAnchor.MiddleCenter);
            controller.titleText.color = new Color32(244, 222, 170, 255);
            controller.bodyText = CreateText("ChoiceBody", panel.transform, new Vector2(0f, 48f), new Vector2(470f, 56f), 18, TextAnchor.MiddleCenter);
            controller.bodyText.color = new Color32(235, 223, 201, 255);
            controller.resourceText = CreateText("ChoiceResources", panel.transform, new Vector2(0f, 8f), new Vector2(430f, 28f), 16, TextAnchor.MiddleCenter);
            controller.resourceText.color = new Color32(214, 197, 168, 255);
            controller.quickReuseButton = CreateButton("QuickReuseButton", panel.transform, new Vector2(-128f, -70f), new Vector2(196f, 88f), "", null);
            controller.carefulExhibitButton = CreateButton("CarefulExhibitButton", panel.transform, new Vector2(128f, -70f), new Vector2(196f, 88f), "", null);
            controller.quickLabelText = controller.quickReuseButton.GetComponentInChildren<Text>();
            controller.carefulLabelText = controller.carefulExhibitButton.GetComponentInChildren<Text>();
            if (controller.quickLabelText != null)
            {
                controller.quickLabelText.fontSize = 16;
            }

            if (controller.carefulLabelText != null)
            {
                controller.carefulLabelText.fontSize = 16;
            }

            controller.footerText = CreateText("ChoiceFooter", panel.transform, new Vector2(0f, -120f), new Vector2(420f, 24f), 14, TextAnchor.MiddleCenter);
            controller.footerText.color = new Color32(214, 197, 168, 255);
            return controller;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
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
            return CreateUiRect(name, parent, anchoredPosition, size, color).GetComponent<Image>();
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

        private static Button CreateButton(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, string label, Sprite sprite)
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
    }
}
