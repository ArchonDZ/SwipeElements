using System.Collections.Generic;
using Zenject;
using Elements.Entities.Background;
using Elements.Systems;

namespace Elements.MemoryPools
{
    public class BackgroundObjectPool
    {
        private int startCount;
        private int maxCount;
        private BackgroundSettings settings;

        private readonly DiContainer _container;
        private readonly Dictionary<int, BackgroundObject.Pool> _pools = new();

        public BackgroundObjectPool(DiContainer container)
        {
            _container = container;
        }

        public void Initialize(int startCount, int maxCount, BackgroundSettings settings)
        {
            this.startCount = startCount;
            this.maxCount = maxCount;
            this.settings = settings;
        }

        public BackgroundObject Spawn(BackgroundObject prefab, float direction, float speed)
        {
            int prefabId = prefab.gameObject.GetInstanceID();

            if (!_pools.TryGetValue(prefabId, out var pool))
            {
                var subContainer = _container.CreateSubContainer();

                subContainer.Bind<BackgroundSettings>().FromInstance(settings).AsSingle();

                subContainer.BindMemoryPool<BackgroundObject, BackgroundObject.Pool>()
                    .WithInitialSize(startCount)
                    .WithMaxSize(maxCount)
                    .FromComponentInNewPrefab(prefab)
                    .UnderTransformGroup("Pool_" + prefab.gameObject.name);

                pool = subContainer.Resolve<BackgroundObject.Pool>();
                _pools.Add(prefabId, pool);
            }

            return pool.Spawn(direction, speed, pool);
        }

        public int GetActiveCount()
        {
            int count = 0;
            foreach (var pool in _pools)
            {
                count += pool.Value.NumActive;
            }
            return count;
        }
    }
}
