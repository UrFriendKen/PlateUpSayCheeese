using KitchenLib.DevUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KitchenSayCheeese
{
    internal class CameraControlUI : BaseUI
    {
        protected GUIStyle labelStyle;
        protected GUIStyle labelCenterStyle;
        protected GUIStyle buttonStyle;

        private ScreenshotCamera CustomCamera => Main.ScreenshotCamera;
        private float verticalOffset = 0.5f;

        private string activePlayer = null;

        private bool showSettings = false;
        private Vector2 scrollPosition = default;

        public CameraControlUI()
        {
            ButtonName = "Camera";
        }

        public sealed override void Setup()
        {
            GUILayout.BeginArea(new Rect(10f, 0f, 770f, 900f));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, GUIStyle.none);

            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding.left = 10;
            labelStyle.stretchWidth = true;

            labelCenterStyle = new GUIStyle(GUI.skin.label);
            labelCenterStyle.alignment = TextAnchor.MiddleCenter;
            labelCenterStyle.padding.left = 10;
            labelCenterStyle.stretchWidth = true;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.stretchWidth = true;

            GUILayout.Label("Camera Mod Menu", labelStyle);

            GUILayout.Space(10);


            if (GUILayout.Button(showSettings ? "Hide Settings" : "Show Settings", buttonStyle))
            {
                showSettings = !showSettings;
            }

            if (showSettings)
            {
                GUILayout.Label($"Focal Length - {CustomCamera.FocalLength}mm");
                CustomCamera.FocalLength = HorizontalSlider(CustomCamera.FocalLength, 15f, 300f);

                GUILayout.Label($"Image Size = {CustomCamera.ImageSizeString}");
                CustomCamera.ImageSizeIndex = (int)HorizontalSlider(CustomCamera.ImageSizeIndex, 0, 24, 0);

                GUILayout.Label($"Vertical Offset - {verticalOffset}mm");
                verticalOffset = HorizontalSlider(verticalOffset, 0f, 1.5f);

                GUILayout.Label($"Distance - {CustomCamera.Distance}m");
                CustomCamera.Distance = HorizontalSlider(CustomCamera.Distance, 0.1f, 10f);

                GUILayout.Label($"Theta - {CustomCamera.Theta} degrees");
                CustomCamera.Theta = HorizontalSlider(CustomCamera.Theta, 0f, 359.9f);

                GUILayout.Label($"Phi - {CustomCamera.Phi} degrees");
                CustomCamera.Phi = HorizontalSlider(CustomCamera.Phi, -90f, 90f);

                GUILayout.Label($"Move Speed - {CustomCamera.Speed} m/s");
                CustomCamera.Speed = HorizontalSlider(CustomCamera.Speed, 0.1f, 30f);

                GUILayout.Label($"Rotation Speed - {CustomCamera.RotationSpeed} rad/s");
                CustomCamera.RotationSpeed = HorizontalSlider(CustomCamera.RotationSpeed, 0.1f, 10f);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Snap To Interaction Target = {PlayerInteractionController.SnapToTarget}", buttonStyle))
                {
                    PlayerInteractionController.SnapToTarget = !PlayerInteractionController.SnapToTarget;
                }
                if (GUILayout.Button($"Highlight Interaction = {PlayerInteractionController.HighlightInteraction}", buttonStyle))
                {
                    PlayerInteractionController.HighlightInteraction = !PlayerInteractionController.HighlightInteraction;
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button($"Render Appliances Only = {CustomCamera.RenderAppliancesOnly}", buttonStyle))
                {
                    CustomCamera.RenderAppliancesOnly = !CustomCamera.RenderAppliancesOnly;
                }
                if (GUILayout.Button($"Reset Settings", buttonStyle))
                {
                    OnResetButtonClicked();
                }
            }

            GUILayout.Space(10);
            GUILayout.Label(activePlayer == null? "Select Player" : activePlayer, labelCenterStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Player", buttonStyle))
            {
                OnPreviousButtonClicked();
            }
            if (GUILayout.Button("Next Player", buttonStyle))
            {
                OnNextButtonClicked();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Capture", buttonStyle))
            {
                OnCaptureButtonClicked();
            }

            Vector3 target = default;
            if (activePlayer != null)
            {
                target = PlayerInteractionController.PlayerTargetPositions[activePlayer];
            }

            if (target != default)
            {
                CustomCamera.Target = target + new Vector3(0f, verticalOffset, 0f);
            }

            // add image preview for render texture
            var rt = CustomCamera.screenshotTexture;
            if (rt != null)
            {
                var textureRect = GUILayoutUtility.GetRect(770f, 770f/CustomCamera.Aspect);
                GUI.DrawTexture(textureRect, rt);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private float HorizontalSlider(float value, float min, float max, int decimals = 1)
        {
            value = GUILayout.HorizontalSlider(value, min, max);
            return Round(value, decimals);
        }

        private float Round(float value, int decimals)
        {
            int muliplier = 1;

            for (int i = 0; i < decimals; i++)
            {
                muliplier *= 10;
            }

            return Mathf.Round(value * muliplier) / muliplier;
        }

        protected void OnPreviousButtonClicked()
        {
            List<string> keys = GetPlayersList();

            if (keys.Count == 0)
            {
                activePlayer = null;
                return;
            }

            if (activePlayer == null)
            {
                activePlayer = keys[0];
                return;
            }
            else
            {
                int index = keys.IndexOf(activePlayer);
                if (index == -1)
                {
                    activePlayer = keys[0];
                    return;
                }

                index = index - 1 < 0 ? keys.Count : index;
                activePlayer = keys[index - 1];
            }
        }

        protected void OnNextButtonClicked()
        {
            List<string> keys = GetPlayersList();

            if (keys.Count == 0)
            {
                activePlayer = null;
                return;
            }

            if (activePlayer == null)
            {
                activePlayer = keys[0];
                return;
            }
            else
            {
                int index = keys.IndexOf(activePlayer);
                if (index == -1)
                {
                    activePlayer = keys[0];
                    return;
                }

                index = index + 1 < keys.Count ? index + 1 : 0;
                activePlayer = keys[index];
            }
        }

        protected List<string> GetPlayersList()
        {
            List<string> keys = PlayerInteractionController.PlayerTargetPositions.Keys.ToList();
            keys.Sort();
            return keys;
        }

        protected void OnCaptureButtonClicked()
        {
            CustomCamera.CaptureScreenshot();
        }

        protected void OnResetButtonClicked()
        {
            CustomCamera.FocalLength = 50f;
            CustomCamera.ImageSizeIndex = 18;
            verticalOffset = 0.5f;
            CustomCamera.Distance = 3f;
            CustomCamera.Theta = 45f;
            CustomCamera.Phi = 45f;
            CustomCamera.Speed = 2f;
            CustomCamera.RotationSpeed = 4.5f;
            PlayerInteractionController.SnapToTarget = false;
            PlayerInteractionController.HighlightInteraction = true;
            CustomCamera.RenderAppliancesOnly = false;
        }
    }
}