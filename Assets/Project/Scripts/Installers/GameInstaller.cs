using Elements.Configs;
using Elements.Systems;
using UnityEngine;
using Zenject;

namespace Elements.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ElementDatabaseConfig elementDatabaseConfig;
        [SerializeField] private CameraSystem cameraSystem;
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private BackgroudSystem backgroudSystem;

        public override void InstallBindings()
        {
            Container.Bind<ElementDatabaseConfig>().FromInstance(elementDatabaseConfig).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CameraSystem>().FromInstance(cameraSystem).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GridSystem>().FromInstance(gridSystem).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BackgroudSystem>().FromInstance(backgroudSystem).AsSingle().NonLazy();
        }
    }
}