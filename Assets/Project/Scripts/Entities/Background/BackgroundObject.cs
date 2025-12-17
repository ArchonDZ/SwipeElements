using Cysharp.Threading.Tasks;
using Elements.Systems;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Elements.Entities.Background
{
    public class BackgroundObject : MonoBehaviour, IPoolable<float, float, IMemoryPool>
    {
        public class Pool : MonoPoolableMemoryPool<float, float, IMemoryPool, BackgroundObject> { }

        [SerializeField] private SpriteRenderer spriteRenderer;

        private IMemoryPool pool;
        private BackgroundSettings settings;

        private float halfSpriteX;
        private float spriteY;
        private float moveDirection;
        private float moveSpeed;
        private float startPositionY;

        private CancellationTokenSource cts;

        [Inject]
        public void Construct(BackgroundSettings backgroundSettings)
        {
            settings = backgroundSettings;
            halfSpriteX = spriteRenderer.bounds.extents.x;
            spriteY = spriteRenderer.bounds.size.y;
        }

        public void OnSpawned(float direction, float speed, IMemoryPool pool)
        {
            this.pool = pool;
            moveDirection = direction;
            moveSpeed = speed;
            gameObject.SetActive(true);

            RecalculateStartPosition();
            CheckCancellationTokenSource();
            UpdateAsync(cts.Token).Forget();
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            StopUniTask();
        }

        private void RecalculateStartPosition()
        {
            float positionX = moveDirection > 0 ? settings.LeftBorder - halfSpriteX : settings.RightBorder + halfSpriteX;
            float positionY = Random.Range(settings.BottomBorder + spriteY, settings.TopBorder - spriteY);

            startPositionY = positionY;
            transform.position = new Vector3(positionX, positionY, transform.position.z);
            gameObject.SetActive(true);
        }

        public async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                transform.position = new Vector3(
                    transform.position.x + (moveDirection * moveSpeed * Time.deltaTime),
                    startPositionY + Mathf.Sin(Time.time * settings.Frequency * moveSpeed) * settings.Amplitude,
                    transform.position.z
                );

                CheckOutOfBounds();

                await UniTask.Yield(token);
            }
        }

        private void CheckOutOfBounds()
        {
            if (moveDirection > 0 && transform.position.x > settings.RightBorder + halfSpriteX ||
                moveDirection < 0 && transform.position.x < settings.LeftBorder - halfSpriteX)
            {
                pool?.Despawn(this);
            }
        }

        private void CheckCancellationTokenSource()
        {
            StopUniTask();
            cts = new CancellationTokenSource();
        }

        private void StopUniTask()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        private void OnDestroy()
        {
            StopUniTask();
        }
    }
}