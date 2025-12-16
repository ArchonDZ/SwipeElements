using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Elements.Entities.Background
{
    public class BackgroundObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float leftBorder;
        private float rightBorder;
        private float topBorder;
        private float bottomBorder;

        private float frequency;
        private float amplitude;

        private float halfSpriteX;
        private float spriteY;

        private float moveDirection;
        private float moveSpeed;

        private float startPositionY;

        private CancellationTokenSource cts;

        public void Initialize(float leftBorderX, float rightBorderX, float topBorderY, float bottomBorderY, float frequency, float amplitude)
        {
            leftBorder = leftBorderX;
            rightBorder = rightBorderX;
            topBorder = topBorderY;
            bottomBorder = bottomBorderY;

            this.frequency = frequency;
            this.amplitude = amplitude;

            halfSpriteX = spriteRenderer.bounds.extents.x;
            spriteY = spriteRenderer.bounds.size.y;
        }

        public void Activate(float direction, float speed)
        {
            moveDirection = direction;
            moveSpeed = speed;

            RecalculateStartPosition();
            CheckCancellationTokenSource();
            UpdateAsync(cts.Token).Forget();
        }

        private void RecalculateStartPosition()
        {
            float positionX = moveDirection > 0 ? leftBorder - halfSpriteX : rightBorder + halfSpriteX;
            float positionY = Random.Range(bottomBorder + spriteY, topBorder - spriteY);

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
                    startPositionY + Mathf.Sin(Time.time * frequency * moveSpeed) * amplitude,
                    transform.position.z
                );

                CheckOutOfBounds();

                await UniTask.Yield(token);
            }
        }

        private void CheckOutOfBounds()
        {
            if (moveDirection > 0 && transform.position.x > rightBorder + halfSpriteX ||
                moveDirection < 0 && transform.position.x < leftBorder - halfSpriteX)
            {
                gameObject.SetActive(false);
                StopUniTask();
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