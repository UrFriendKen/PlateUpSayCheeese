using System.IO;
using UnityEngine;

namespace KitchenSayCheeese
{
    public class ScreenshotCamera : MonoBehaviour
    {
        public Camera movableCamera { get; private set; }
        public RenderTexture screenshotTexture { get; private set; }


        private float _focalLength = 50f;
        public float FocalLength
        {
            get
            {
                return _focalLength;
            }
            set
            {
                _focalLength = Mathf.Clamp(value, 5f, 300f);
            }
        }

        public float Aspect => movableCamera.aspect;

        public float Theta { get; set; } = 45f;
        public float Phi { get; set; } = 45f;
        public float Distance { get; set; } = 3f;
        public Vector3 Target { get; set; } = Vector3.zero;
        public float Speed { get; set; } = 2f;
        public float RotationSpeed { get; set; } = 4.5f;
        public bool RenderAppliancesOnly { get; set; } = false;
        public bool RenderInteractedApplianceOnly { get; set; } = false;
        public int ImageSizeIndex { get; set; } = 18;
        private int prevImageSizeIndex = 0;
        public string ImageSizeString { get; private set; } = string.Empty;
        private int[] widths = { 16, 32, 60, 64, 96, 128, 160, 240, 320, 640, 1024, 1024, 1024, 1152, 1280, 1280, 1600, 1600, 1920, 1920, 2160, 2560, 3200, 5120, 3840 };
        private int[] heights= { 16, 32, 40, 64, 96, 128, 160, 240, 320, 480,  576,  768, 1024,  720,  720,  960,  768, 1200, 1080, 1200, 1200, 1440, 1800, 1440, 2160 };

        private void Start()
        {
            if (movableCamera == null)
            {
                // Create the auxiliary camera
                movableCamera = new GameObject("Movable Camera").AddComponent<Camera>();
                movableCamera.aspect = 1.5f;
                movableCamera.gameObject.SetActive(true); // Disable the camera initially
                movableCamera.clearFlags = CameraClearFlags.SolidColor; // Set clear flags to solid color
                movableCamera.backgroundColor = Color.black; // Set background color to black
                movableCamera.transform.SetParent(transform, false);

                // Create the render texture
                screenshotTexture = new RenderTexture(16, 16, 16, RenderTextureFormat.ARGB32);
                movableCamera.targetTexture = screenshotTexture;
            }
        }

        private void Update()
        {
            SetFocalLength(_focalLength);
            UpdateCameraPosition();
            UpdateCullingMask();
            UpdatePixelCount();
            ImageSizeString = $"{widths[ImageSizeIndex]}x{heights[ImageSizeIndex]}";
        }

        private void UpdateCameraPosition()
        {
            float dt = Time.deltaTime;

            Vector3 offset = SphericalToRectangular(Distance, Theta, Phi);
            // Update the position of the camera based on the input fields

            transform.localPosition = InterpolatePosition(transform.localPosition, Target, Speed, dt);
            movableCamera.transform.localPosition = offset;

            movableCamera.transform.localRotation = InterpolateRotation(movableCamera.transform.localRotation, Quaternion.LookRotation(Target - transform.localPosition - movableCamera.transform.localPosition), RotationSpeed, dt);
            //movableCamera.transform.LookAt(Target);

            if (Phi == 90 || Phi == -90)
            {
                movableCamera.transform.Rotate(Vector3.forward, Theta);
            }
        }

        private Vector3 InterpolatePosition(Vector3 from, Vector3 to, float speed, float deltaTime)
        {
            Vector3 displacement = to - from;
            float totalDistance = displacement.magnitude;
            float maxDistanceMove = deltaTime * speed;
            return from + displacement.normalized * Mathf.Clamp(maxDistanceMove, 0, totalDistance);
        }

        private Quaternion InterpolateRotation(Quaternion from, Quaternion to, float rotationSpeed, float deltaTime)
        {
            return Quaternion.Lerp(from, to, rotationSpeed * deltaTime);
        }

        private void UpdateCullingMask()
        {
            //if (PlayerInteractionController.RenderInteractedAppliancesOnly)
            //{
            //    movableCamera.cullingMask = 1 << Main.INTERACTION_LAYER;
            //    return;
            //}
            if (RenderAppliancesOnly)
            {
                movableCamera.cullingMask = 1 << LayerMask.NameToLayer("Default");
                return;
            }
            movableCamera.cullingMask = -1;
        }

        private void UpdatePixelCount()
        {
            if (prevImageSizeIndex != ImageSizeIndex)
            {
                ImageSizeIndex = Mathf.Clamp(ImageSizeIndex, 0, widths.Length - 1);
                prevImageSizeIndex = ImageSizeIndex;
                int width = widths[ImageSizeIndex];
                int height = heights[ImageSizeIndex];
                screenshotTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
                movableCamera.targetTexture = screenshotTexture;
                movableCamera.aspect = ((float)width)/((float)height);
            }
        }

        public void SetFocalLength(float focalLength)
        {
            if (movableCamera != null)
            {
                movableCamera.fieldOfView = FocalLengthToFOV(focalLength);
            }
        }

        private static float FocalLengthToFOV(float focalLength)
        {
            float fovInRadians = 2f * Mathf.Atan(24f / (2f * focalLength));
            float fovInDegrees = fovInRadians * Mathf.Rad2Deg;

            float horizontalFOVInDegrees = fovInDegrees * 1.5f;

            return horizontalFOVInDegrees;
        }

        public Vector3 SphericalToRectangular(float r, float theta, float phi)
        {
            float t = Mathf.Deg2Rad * (theta + 90);
            float p = Mathf.Deg2Rad * (phi - 90);
            float x = r * Mathf.Sin(p) * Mathf.Cos(t);
            float y = r * Mathf.Cos(p);
            float z = r * Mathf.Sin(p) * Mathf.Sin(t);
            return new Vector3(x, y, z);
        }

        public void CaptureScreenshot()
        {   
            // Save the texture to a PNG file
            Texture2D screenshot = new Texture2D(screenshotTexture.width, screenshotTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = screenshotTexture;
            screenshot.ReadPixels(new Rect(0, 0, screenshotTexture.width, screenshotTexture.height), 0, 0);
            screenshot.Apply();
            RenderTexture.active = null;
            byte[] bytes = screenshot.EncodeToPNG();
            string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            SavePNG(filename, bytes);
        }


        protected void SavePNG(string name, byte[] bytes)
        {
            string folder = Path.Combine(Application.persistentDataPath, "SayCheeese");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), bytes);
        }

        public void TogglePreview()
        {
            // Toggle the main camera's target texture between the render texture and null
            Camera.main.targetTexture = (Camera.main.targetTexture == screenshotTexture) ? null : screenshotTexture;
        }
    }
}