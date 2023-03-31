using System.Collections.Generic;
using GapBetweenRunnerAndShooter.CraftSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Level_selection.Bestiary_system
{
    public class BestiaryItem : MonoBehaviour
    {
        public ModelType ModelType => _modelType;
        
        [SerializeField]
        private ModelType _modelType;
        [SerializeField]
        private Color _colorOpen;
        [SerializeField]
        private RawImage _rawImage;
        [SerializeField]
        private GameObject _newPanel;
        [SerializeField]
        private List<Image> _reagentSprites = new List<Image>();
        [SerializeField]
        private Animator _animatorModel;

        public void ShowOpenImages(List<Sprite> sprites, bool showNewPanel = false)
        {
            _animatorModel.enabled = true;
            _rawImage.color = _colorOpen;
            _newPanel.gameObject.SetActive(showNewPanel);
            
            for (var i = 0; i < _reagentSprites.Count; i++)
            {
                _reagentSprites[i].sprite = sprites[i];
            }
        }

        private void OnDisable()
        {
            _newPanel.gameObject.SetActive(false);
        }
    }
}