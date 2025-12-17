using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(GridSettingConfig), menuName = "Configs/" + nameof(GridSettingConfig))]
    public class GridSettingConfig : ScriptableObject
    {
        [SerializeField] private int startElementPoolCount = 10;
        [SerializeField] private int maxElementPoolCount = 200;

        public int StartElementPoolCount => startElementPoolCount;
        public int MaxElementPoolCount => maxElementPoolCount;
    }
}