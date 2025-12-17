using Elements.Entities;
using System.Collections.Generic;
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

            return pool.Spawn(position, order, pool);
        }
    }
}