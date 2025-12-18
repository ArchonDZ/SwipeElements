using Elements.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Elements.MemoryPools
{
    public class ElementPool
    {
        private int startCount;
        private int maxCount;

        private readonly DiContainer _container;
        private readonly Dictionary<int, Element.Pool> _pools = new();
        private readonly List<Element> _currentlyActive = new();

        public ElementPool(DiContainer container)
        {
            _container = container;
        }

        public void Initialize(int startCount, int maxCount)
        {
            this.startCount = startCount;
            this.maxCount = maxCount;
        }

        public Element Spawn(Element prefab, Vector2 position, int order)
        {
            int prefabId = prefab.gameObject.GetInstanceID();

            if (!_pools.TryGetValue(prefabId, out var pool))
            {
                var subContainer = _container.CreateSubContainer();

                subContainer.BindMemoryPool<Element, Element.Pool>()
                    .WithInitialSize(startCount)
                    .WithMaxSize(maxCount)
                    .ExpandByDoubling()
                    .FromComponentInNewPrefab(prefab)
                    .UnderTransformGroup("Pool_" + prefab.gameObject.name);

                pool = subContainer.Resolve<Element.Pool>();
                _pools.Add(prefabId, pool);
            }

            Element element = pool.Spawn(position, order, pool);
            element.OnDespawnedEvent += Element_OnDespawnedEvent;
            _currentlyActive.Add(element);

            return element;
        }

        public void DespawnAll()
        {
            var list = _currentlyActive.ToList();
            foreach (var obj in list)
            {
                obj.ReturnToPool();
            }
        }

        private void Element_OnDespawnedEvent(Element element)
        {
            element.OnDespawnedEvent -= Element_OnDespawnedEvent;
            _currentlyActive.Remove(element);
        }
    }
}