using System;
using System.Collections.Generic;
using System.Linq;
using GapBetweenRunnerAndShooter.CraftSystem;
using Level_selection.Map_system;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level_selection.Bestiary_system
{
    public class Tab : MonoBehaviour, IPointerClickHandler
    {
        public event Action<WorldType> OnClick;
        
        public List<BestiaryItem> OpenItems => _openItems;
        public WorldType WorldType => _worldType;
        public float ProgressPercentOpen => _openItems.Count / (float) _bestiaryItems.Count * 100f;
        public int CountOpenItems => _openItems.Count;

        [SerializeField]
        private GameObject _newModelPanel;
        [SerializeField]
        private WorldType _worldType;
        [SerializeField]
        private List<BestiaryItem> _bestiaryItems = new List<BestiaryItem>();
        [SerializeField]
        private GameObject _content;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private Animator _animator;

        private List<BestiaryItem> _openItems = new List<BestiaryItem>();
        private IconReagentProvider _iconReagentProvider;
        private TabAnimationController _tabAnimationController;

        public void Initialize(List<ModelType> openItems, IconReagentProvider iconReagentProvider)
        {
            _tabAnimationController = new TabAnimationController(_animator);
            _iconReagentProvider = iconReagentProvider;

            if (openItems != null)
            {
                foreach (var modelType in openItems)
                {
                    foreach (var bestiaryItem in _bestiaryItems.Where(bestiaryItem => modelType == bestiaryItem.ModelType))
                    {
                        _openItems.Add(bestiaryItem);
                        bestiaryItem.ShowOpenImages(iconReagentProvider.GetSpriteByModelType(bestiaryItem.ModelType));
                        bestiaryItem.transform.SetAsFirstSibling();
                    }
                }
            }
        }

        public void Open()
        {
            _newModelPanel.SetActive(false);
            _scrollRect.verticalNormalizedPosition = 1f;
            _content.SetActive(true);
            _tabAnimationController.SetSelect(true);
        }

        public void Hide()
        {
            _content.SetActive(false);
            _tabAnimationController.SetSelect(false);
        }

        public bool ContainsModel(ModelType modelType)
        {
            foreach (var bestiaryItem in _bestiaryItems)
            {
                if (modelType == bestiaryItem.ModelType)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsOpenModel(ModelType modelType)
        {
            foreach (var bestiaryItem in _openItems)
            {
                if (modelType == bestiaryItem.ModelType)
                {
                    return true;
                }
            }

            return false;
        }

        public void OpenModel(ModelType modelType)
        {
            foreach (var bestiaryItem in _bestiaryItems)
            {
                if (bestiaryItem.ModelType == modelType)
                {
                    _newModelPanel.SetActive(true);
                    _openItems.Add(bestiaryItem);
                    bestiaryItem.transform.SetAsFirstSibling();
                    bestiaryItem.ShowOpenImages(_iconReagentProvider.GetSpriteByModelType(modelType), true);
                    break;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(_worldType);
        }
    }
}