using ResourceSystem;
using TMPro;
using UnityEngine;
using Zenject;

namespace Base
{
    public class ResourceView: MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _cashText;
    
        private ResourceManagerGame _resourceManagerGame;

        [Inject]
        public void Constructor(ResourceManagerGame resourceManagerGame)
        {
            _resourceManagerGame = resourceManagerGame;
            _resourceManagerGame.OnResourceChange += SetCashText;
            SetCashText(_resourceManagerGame.GetResource(ResourceType.Cash));
        }

        private void SetCashText(Resource resource)
        {
            _cashText.text = resource.Amount.ToString();
        }

        private void OnDestroy()
        {
            if (_resourceManagerGame != null)
            {
                _resourceManagerGame.OnResourceChange -= SetCashText;
            }
        }
    }
}