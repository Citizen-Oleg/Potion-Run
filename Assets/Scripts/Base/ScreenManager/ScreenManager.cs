using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers.ScreensManager;
using Cysharp.Threading.Tasks;
using LoadingSystem;

namespace Managers.ScreensManager
{
    public class ScreenManager
    {
        private readonly ScreenLoader _screenLoader;
        private readonly List<BaseScreen> _screens = new List<BaseScreen>();
        private readonly List<ScreenAddressablesID> _screenAddressablesId;

        public ScreenManager(Settings settings)
        {
            _screenLoader = new ScreenLoader();
            _screenAddressablesId = settings.ScreenAddressablesID;
        }
        
        public async UniTask OpenScreen(ScreenType screenType)
        {
            if (IsScreenOpened(screenType))
            {
                return;
            }

            var screen = await _screenLoader.ScreenLoadInternal<BaseScreen>(screenType, GetScreenIDByType(screenType));
            screen.ScreenManager = this;
            _screens.Add(screen);
        }
        
        public async UniTask OpenScreenWithContext<TContext>(ScreenType screenType,
            TContext context) where TContext : BaseContext
        {
            if (IsScreenOpened(screenType))
            {
                return;
            }
            
            var screen = await _screenLoader.ScreenLoadInternal<BaseScreenWithContext<TContext>>(screenType, GetScreenIDByType(screenType));
            screen.ScreenManager = this;
            screen.ApplyContext(context);

            _screens.Add(screen);
        }

        public void CloseScreenByType(ScreenType screenType)
        {
            if (_screens.Count == 0)
            {
                return;
            }

            var screen = GetScreenByType(screenType);
            if (screen != null)
            {
                _screens.Remove(screen);
                _screenLoader.UnloadInternalByScreenType(screenType);
            }
        }

        public void CloseAllScreens()
        {
            foreach (var screen in _screens)
            {
                _screenLoader.UnloadInternalByScreenType(screen.ScreenType);
            }

            _screens.Clear();
        }

        private bool IsScreenOpened(ScreenType screenType)
        {
            foreach (var screen in _screens)
            {
                if (screen.ScreenType == screenType)
                {
                    return true;
                }
            }

            return false;
        }

        private string GetScreenIDByType(ScreenType screenType)
        {
            foreach (var screenID in _screenAddressablesId)
            {
                if (screenID.ScreenType == screenType)
                {
                    return screenID.AssetID;
                }
            }

            return null;
        }

        private BaseScreen GetScreenByType(ScreenType screenType)
        {
            return _screens.FirstOrDefault(screen => screen.ScreenType == screenType);
        }

        [Serializable]
        public class Settings
        {
            public List<ScreenAddressablesID> ScreenAddressablesID = new List<ScreenAddressablesID>();
        }

        [Serializable]
        public class ScreenAddressablesID
        {
            public ScreenType ScreenType;
            public string AssetID;
        }
    }
}
