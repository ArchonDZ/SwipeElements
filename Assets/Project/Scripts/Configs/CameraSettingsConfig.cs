using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(CameraSettingsConfig), menuName = "Configs/" + nameof(CameraSettingsConfig))]
    public class CameraSettingsConfig : ScriptableObject
    {
        [SerializeField][Range(0, 1f)] private float paddingLeft = 0.1f;
        [SerializeField][Range(0, 1f)] private float paddingRight = 0.25f;
        [SerializeField][Range(0, 3f)] private float paddingTop = 2.5f;
        [SerializeField][Range(0, 3f)] private float paddingBottom = 2.75f;

        public float PaddingLeft => paddingLeft;
        public float PaddingRight => paddingRight;
        public float HorizontalPadding => paddingLeft + paddingRight;
        public float PaddingTop => paddingTop;
        public float PaddingBottom => paddingBottom;
        public float VerticalPadding => paddingTop + paddingBottom;
    }
}