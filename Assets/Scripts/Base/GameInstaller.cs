using Base.MoveAnimation._3D;
using Base.MoveAnimation.UI;
using Managers.ScreensManager;
using ParticleFactory;
using ResourceSystem;
using UnityEngine;
using Zenject;

namespace Base
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ScreenManager>().AsSingle().NonLazy();
            Container.Bind<ResourceManagerGame>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AnimationManagerUI>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AnimationManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ParticleSystemFactory>().AsSingle().WithArguments(transform).NonLazy();
        }
    }
}
