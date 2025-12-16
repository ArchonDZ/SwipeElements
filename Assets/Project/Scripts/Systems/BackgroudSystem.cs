using Cysharp.Threading.Tasks;
using Elements.Configs;
using Elements.Entities.Background;
using System.Collections.Generic;
using System.Linq;
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

        [Inject] private DiContainer container;
        [Inject] private CameraSystem cameraSystem;

        private float leftBorderX;
        private float rightBorderX;
        private float topBorderX;
        private float bottomBorderX;
        private List<BackgroundObject> objectPool;

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
            leftBorderX = cameraSystem.MainCamera.transform.position.x - halfWidth;
            rightBorderX = cameraSystem.MainCamera.transform.position.x + halfWidth;
            topBorderX = (cameraSystem.MainCamera.orthographicSize * 2) - cameraSystem.CameraSettingsConfig.VerticalPadding;
            bottomBorderX = 0;

            CreateObjectPool();
            SpawnLoopAsync(destroyCancellationToken).Forget();
        }

        private void CreateObjectPool()
        {
            if (backgroundObjectPrefabs.Count <= 0) return;

            objectPool = new List<BackgroundObject>(backgroundConfig.MaxObjectCountOnScreen);
            for (int i = 0; i < backgroundConfig.MaxObjectCountOnScreen; i++)
            {
                CreateObject();
            }
        }

        private BackgroundObject CreateObject()
        {
            BackgroundObject randomPrefab = backgroundObjectPrefabs[Random.Range(0, backgroundObjectPrefabs.Count)];
            BackgroundObject backObject = container.InstantiatePrefabForComponent<BackgroundObject>(randomPrefab);
            backObject.Initialize(leftBorderX, rightBorderX, topBorderX, bottomBorderX, backgroundConfig.ObjectSinFrequency, backgroundConfig.ObjectSinAmplitude);
            backObject.gameObject.SetActive(false);
            objectPool.Add(backObject);
            return backObject;
        }

        private async UniTaskVoid SpawnLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (objectPool.Where(x => x.gameObject.activeSelf).Count() < backgroundConfig.MaxObjectCountOnScreen)
                {
                    BackgroundObject backgroundObject = objectPool.Find(x => !x.gameObject.activeSelf);
                    if (backgroundObject == null)
                        backgroundObject = CreateObject();

                    float direction = Random.Range(0, 2) == 1 ? 1 : -1;
                    float speed = Random.Range(backgroundConfig.MinObjectSpeed, backgroundConfig.MaxObjectSpeed);
                    backgroundObject.Activate(direction, speed);
                }

                await UniTask.WaitForSeconds(backgroundConfig.SpawnInterval, cancellationToken: cancellationToken);
            }
        }
    }
}