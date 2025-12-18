using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elements.Configs;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Elements.Entities
{
    public class Element : MonoBehaviour, IPoolable<Vector2, int, IMemoryPool>
    {
        public class Pool : MonoPoolableMemoryPool<Vector2, int, IMemoryPool, Element> { }

        public event Action<Element> OnDespawnedEvent;

        [SerializeField] private ElementConfig config;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ElementAnimator animator;

        private IMemoryPool pool;
        private bool isAvailable;
        private Sequence moveSequence;
        private Sequence destroySequence;
        private CancellationTokenSource cts;

        public bool IsAvailable => isAvailable;
        public int SortingOrder => spriteRenderer.sortingOrder;

        private void Awake()
        {
            animator.InitializeStates(config.IdleStateName, config.DestroyStateName);
        }

        public void OnSpawned(Vector2 position, int order, IMemoryPool pool)
        {
            this.pool = pool;

            transform.position = position;
            SetSortingOrder(order);

            isAvailable = true;
            animator.PlayIdle();
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            isAvailable = false;
            gameObject.SetActive(false);
            StopUniTask();
            OnDespawnedEvent?.Invoke(this);
        }

        public void ReturnToPool()
        {
            pool?.Despawn(this);
        }

        public Sequence Move(Vector2 position, int order)
        {
            return MoveTo(position, order, config.SwapDuration).SetEase(config.SwapEase);
        }

        public Sequence Drop(Vector2 position, int order)
        {
            return MoveTo(position, order, config.DropDuration).SetEase(config.DropEase);
        }

        private Sequence MoveTo(Vector2 position, int order, float duration)
        {
            isAvailable = false;

            moveSequence?.Kill();
            moveSequence = DOTween.Sequence()
                .AppendCallback(() => SetSortingOrder(order))
                .Append(transform.DOMove(position, duration))
                .OnComplete(OnCompleteMovement);

            return moveSequence;
        }

        public Sequence Destroy()
        {
            isAvailable = false;

            destroySequence?.Kill();
            destroySequence = DOTween.Sequence()
                .AppendCallback(InvokeDestroy)
                .AppendInterval(config.DestroyDuration);

            return destroySequence;
        }

        private void SetSortingOrder(int value)
        {
            spriteRenderer.sortingOrder = value;
        }

        private void OnCompleteMovement()
        {
            isAvailable = true;
        }

        private void InvokeDestroy()
        {
            CheckCancellationTokenSource();
            DestroyAsync(cts.Token).Forget();
        }

        private void CheckCancellationTokenSource()
        {
            StopUniTask();
            cts = new CancellationTokenSource();
        }

        private async UniTaskVoid DestroyAsync(CancellationToken token)
        {
            try
            {
                await animator.PlayDestroyAsync(token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            ReturnToPool();
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
            moveSequence?.Kill();
            destroySequence?.Kill();
        }
    }
}