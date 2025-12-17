using Elements.Configs;
using UnityEngine;

namespace Elements.Systems
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CameraSettingsConfig cameraSettingsConfig;
        [SerializeField] private Camera mainCamera;

        public CameraSettingsConfig CameraSettingsConfig => cameraSettingsConfig;
        public Camera MainCamera => mainCamera;

        public void UpdateCameraFocus(Vector2 gridSize)
        {
            float targetWidth = gridSize.x + cameraSettingsConfig.HorizontalPadding;
            float targetHeight = gridSize.y + cameraSettingsConfig.VerticalPadding;
            float targetAspect = targetWidth / targetHeight;
            float screenAspect = (float)Screen.width / Screen.height;

            if (screenAspect >= targetAspect)
            {
                mainCamera.orthographicSize = targetHeight / 2f;
            }
            else
            {
                float differenceInAspect = targetAspect / screenAspect;
                mainCamera.orthographicSize = (targetHeight / 2f) * differenceInAspect;
            }

            float horizontalGridCenter = gridSize.x / 2f;
            float horizontalOffset = cameraSettingsConfig.HorizontalPadding / 2f - cameraSettingsConfig.PaddingLeft;

            float cameraHeight = mainCamera.orthographicSize * 2;
            float verticalOffsetCenter = mainCamera.orthographicSize - (cameraHeight * cameraSettingsConfig.BottomOffsetPercent);

            mainCamera.transform.position = new Vector3(
                horizontalGridCenter + horizontalOffset,
                verticalOffsetCenter,
                mainCamera.transform.position.z
                );
        }
    }
}