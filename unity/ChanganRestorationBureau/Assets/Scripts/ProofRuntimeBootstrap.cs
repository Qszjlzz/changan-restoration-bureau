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

            var objective = controller.gameObject.AddComponent<ProofObjectiveState>();
            objective.objectiveText = CreateText("ObjectiveText", ui.transform, new Vector2(-420f, 295f), new Vector2(420f, 70f), 18, TextAnchor.MiddleLeft);
            objective.objectiveText.text = "Goal: clear the grass and uncover the relic.";
            objective.hintText = CreateText("InteractionHint", ui.transform, new Vector2(0f, -270f), new Vector2(360f, 48f), 22, TextAnchor.MiddleCenter);
            objective.hintText.color = new Color32(78, 47, 28, 255);
            objective.hintText.enabled = false;
            objective.targetArtifact = controller.artifacts.Count > 0 ? controller.artifacts[0] : null;

            var guide = CreateText("HintText", ui.transform, new Vector2(-250f, 320f), new Vector2(650f, 40f), 20, TextAnchor.MiddleLeft);
            guide.text = "WASD move | E interact | click or 1-6 select relics";

            var interaction = player != null ? player.GetComponent<InteractionController>() : null;
            if (interaction != null)
            {
                interaction.proofController = controller;
                interaction.objective = objective;
            }

            foreach (var artifact in controller.artifacts)
            {
                var target = artifact.GetComponent<InteractionTarget>();
                if (target != null)
                {
                    target.artifact = artifact;
                }
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
