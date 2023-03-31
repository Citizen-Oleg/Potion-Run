using System;
using Managers.ScreensManager;
using UnityEngine;

namespace Assets.Scripts.Managers.ScreensManager
{
    /// <summary>
    /// Этот базовый класс для представления экранов
    /// </summary>
    public abstract class BaseScreen : MonoBehaviour
    {
        public ScreenManager ScreenManager { private get; set; }
        public ScreenType ScreenType => _screenType;

        [SerializeField]
        private ScreenType _screenType;

        private void OnDestroy()
        {
            ScreenManager.CloseScreenByType(_screenType);
        }
    }
}
