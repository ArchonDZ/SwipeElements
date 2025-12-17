using Cysharp.Threading.Tasks;
using Elements.Configs;
using Elements.Entities.Background;
using Elements.MemoryPools;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Elements.Systems
{
    public class BackgroudSystem : MonoBehaviour
    {
        [SerializeField] private BackgroundConfig backgroundConfig;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private List<BackgroundObject> backgroundObjectPrefabs = new();

        [Inject] private CameraSystem cameraSystem;
        [Inject] private BackgroundObjectPool backgroundObjectPool;

        private BackgroundSettings backgroundSettings;

        public void SetupBackground()
        {
            float cameraHeight = cameraSystem.MainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cameraSystem.MainCamera.aspect;

            float spriteWidth = backgroundSpriteRenderer.sprite.bounds.size.x;
            float spriteHeight = backgroundSpriteRenderer.sprite.bounds.size.y;

            float scaleX = cameraWidth / spriteWidth;
            float scaleY = cameraHeight / spriteHeight;
            float envelopeScale = Mathf.Max(scaleX, scaleY);

            backgroundSpriteRenderer.transform.localScale = new Vector3(envelopeScale, envelopeScale, 1);
            backgroundSpriteRenderer.transform.position = new Vector3(
                cameraSystem.MainCamera.transform.position.x,
                cameraSystem.MainCamera.transform.position.y - cameraSystem.MainCamera.orthographicSize,
                backgroundSpriteRenderer.transform.position.z
                );

            float halfWidth = cameraWidth / 2f;
            backgroundSettings = new BackgroundSettings()
            {
                LeftBorder = cameraSystem.MainCamera.transform.position.x - halfWidth,
                RightBorder = cameraSystem.MainCamera.transform.position.x + halfWidth,
                TopBorder = (cameraSystem.MainCamera.orthographicSize * 2) - cameraSystem.CameraSettingsConfig.VerticalPadding,
                BottomBorder = 0,
                Amplitude = backgroundConfig.ObjectSinAmplitude,
                Frequency = backgroundConfig.ObjectSinFrequency
            };

            if (backgroundObjectPrefabs.Count > 0)
            {
                backgroundObjectPool.Initialize(backgroundConfig.MaxObjectCountOnScreen, backgroundConfig.MaxObjectCountOnScreen, backgroundSettings);
                SpawnLoopAsync(destroyCancellationToken).Forget();
            }
        }

        private async UniTaskVoid SpawnLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (backgroundObjectPool.GetActiveCount() < backgroundConfig.MaxObjectCountOnScreen)
                {
                    BackgroundObject randomPrefab = backgroundObjectPrefabs[Random.Range(0, backgroundObjectPrefabs.Count)];
                    float direction = Random.Range(0, 2) == 1 ? 1 : -1;
                    float speed = Random.Range(backgroundConfig.MinObjectSpeed, backgroundConfig.MaxObjectSpeed);
                    backgroundObjectPool.Spawn(randomPrefab, direction, speed);
                }

                await UniTask.WaitForSeconds(backgroundConfig.SpawnInterval, cancellationToken: cancellationToken);
            }
        }
    }

    public struct BackgroundSettings
    {
        public float LeftBorder, RightBorder, TopBorder, BottomBorder, Frequency, Amplitude;
    }
}