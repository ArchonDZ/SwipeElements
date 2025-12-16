using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(BackgroundConfig), menuName = "Configs/" + nameof(BackgroundConfig))]
    public class BackgroundConfig : ScriptableObject
    {
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int maxObjectCountOnScreen = 3;
        [SerializeField][Range(0.1f, 3f)] private float minObjectSpeed = 0.1f;
        [SerializeField][Range(0.1f, 3f)] private float maxObjectSpeed = 2f;
        [SerializeField] private float objectSinFrequency = 1f;
        [SerializeField] private float objectSinAmplitude = 0.5f;

        public float SpawnInterval => spawnInterval;
        public int MaxObjectCountOnScreen => maxObjectCountOnScreen;
        public float MinObjectSpeed => minObjectSpeed;
        public float MaxObjectSpeed => maxObjectSpeed;
        public float ObjectSinFrequency => objectSinFrequency;
        public float ObjectSinAmplitude => objectSinAmplitude;
    }
}
