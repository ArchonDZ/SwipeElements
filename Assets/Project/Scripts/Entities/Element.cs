using UnityEngine;
using Zenject;

namespace Elements.Entities
{
    public class Element : MonoBehaviour, IPoolable<Vector2, int, IMemoryPool>
    {
        public class Pool : MonoPoolableMemoryPool<Vector2, int, IMemoryPool, Element> { }

        [SerializeField] private SpriteRenderer spriteRenderer;

        private IMemoryPool pool;
        private bool isAvailable;

        public bool IsAvailable => isAvailable;

        public void OnSpawned(Vector2 position, int order, IMemoryPool pool)
        {
            this.pool = pool;

            transform.position = position;
            SetSortingOrder(order);

            isAvailable = true;
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

        public void SetSortingOrder(int value)
        {
            spriteRenderer.sortingOrder = value;
        }
    }
}