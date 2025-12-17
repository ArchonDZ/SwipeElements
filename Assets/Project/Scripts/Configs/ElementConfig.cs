using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(ElementConfig), menuName = "Configs/" + nameof(ElementConfig))]
    public class ElementConfig : ScriptableObject
    {
        [SerializeField] private float swapDuration = 0.3f;

        public float SwapDuration => swapDuration;
    }
}