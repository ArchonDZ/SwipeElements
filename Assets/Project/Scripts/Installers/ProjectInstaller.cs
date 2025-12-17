using Elements.Configs;
using Elements.Systems;
using UnityEngine;
using Zenject;

namespace Elements.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private ProjectSettingsConfig projectSettingsConfig;
        [SerializeField] private InputSystem inputSystem;
        [SerializeField] private LevelSystem levelSystem;

        public override void InstallBindings()
        {
            Container.Bind<ProjectSettingsConfig>().FromInstance(projectSettingsConfig).AsSingle().NonLazy();
            Container.Bind<SaveSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputSystem>().FromInstance(inputSystem).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelSystem>().FromInstance(levelSystem).AsSingle().NonLazy();
        }
    }
}