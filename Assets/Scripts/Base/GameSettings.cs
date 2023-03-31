using Base.MoveAnimation._3D;
using Base.MoveAnimation.UI;
using Managers.ScreensManager;
using ParticleFactory;
using ResourceSystem;
using UnityEngine;
using Zenject;

namespace Base
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings", order = 0)]
    public class GameSettings : ScriptableObjectInstaller
    {
        [SerializeField]
        private ResourceManagerGame.Settings _settingsResourceManagerGame;
        [SerializeField]
        private AnimationManagerUI.Settings _settingsAnimationManagerUI;
        [SerializeField]
        private ScreenManager.Settings _settingsScreenManager;
        [SerializeField]
        private AnimationSettings _animationSettings;
        [SerializeField]
        private ParticleSystemFactory.Settings _settingsParticleSystemFactory;

        public override void InstallBindings()
        {
            Container.BindInstance(_settingsAnimationManagerUI);
            Container.BindInstance(_settingsResourceManagerGame);
            Container.BindInstance(_settingsScreenManager);
            Container.BindInstance(_animationSettings);
            Container.BindInstance(_settingsParticleSystemFactory);
        }
    }
}