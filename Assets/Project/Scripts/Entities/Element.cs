using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elements.Configs;
using UnityEngine;
using Zenject;

namespace Elements.Entities
{
    public class Element : MonoBehaviour, IPoolable<Vector2, int, IMemoryPool>
    {
        public class Pool : MonoPoolableMemoryPool<Vector2, int, IMemoryPool, Element> { }

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ElementAnimator animator;

        [Inject] private ElementConfig config;

        private IMemoryPool pool;
        private bool isAvailable;
        private Sequence sequence;

        public bool IsAvailable => isAvailable;
        public int SortingOrder => spriteRenderer.sortingOrder;

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
        }

        public void ReturnToPool()
        {
            pool?.Despawn(this);
        }

        public Sequence MoveTo(Vector2 position, int order)
        {
            isAvailable = false;

            sequence?.Kill();
            sequence = DOTween.Sequence()
                .AppendCallback(() => SetSortingOrder(order))
                .Append(transform.DOMove(position, config.SwapDuration))
                .OnComplete(OnCompleteMovement);

            return sequence;
        }

        public void Destroy()
        {
            DestroyAsync().Forget();
        }

        private void SetSortingOrder(int value)
        {
            spriteRenderer.sortingOrder = value;
        }

        private async UniTaskVoid DestroyAsync()
        {
            isAvailable = false;
            await animator.PlayDestroyAsync();
            ReturnToPool();
        }

        private void OnCompleteMovement()
        {
            isAvailable = true;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}