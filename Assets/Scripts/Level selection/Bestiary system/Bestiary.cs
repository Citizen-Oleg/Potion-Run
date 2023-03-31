using System.Collections.Generic;
using System.Linq;
using Events;
using GapBetweenRunnerAndShooter.CraftSystem;
using Level_selection.Map_system;
using SaveAndLoadSystem;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using Zenject;

namespace Level_selection.Bestiary_system
{
    public class Bestiary : MonoBehaviour
    {
        public Map Map
        {
            set => _map = value;
        }
        
        [SerializeField]
        private List<Tab> _tabs = new List<Tab>();

        private Save _save;
        private Map _map;
        private CraftController _craftController;
        private CompositeDisposable _subscription;
        private List<ModelType> _modelTypes = new List<ModelType>();

        private void OnEnable()
        {
            OpenStartTab();
        }

        [Inject]
        public void Initialize(Save save, Load load, CraftController craftController, IconReagentProvider iconReagentProvider)
        {
            _craftController = craftController;
            _save = save;

            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventCreateMonster>(CheckOpenModel),
                EventStreams.UserInterface.Subscribe<EventOpenBestiary>(Open)
            };
            
            foreach (var tab in _tabs)
            {
                tab.Initialize(load.GetBestiaryItemByWorldType(tab.WorldType), iconReagentProvider);
                tab.OnClick += OpenTabByWorldType;
            }
            
            gameObject.SetActive(false);
        }

        private void OpenTabByWorldType(WorldType worldType)
        {
            foreach (var tab in _tabs)
            {
                if (tab.WorldType == worldType)
                {
                    tab.Open();
                }
                else
                {
                    tab.Hide();
                }
            }
        }

        private void OpenStartTab()
        {
            OpenTabByWorldType(_map?.CurrentRegion.Information.WorldType ?? WorldType.Fire);
        }

        private void CheckOpenModel(EventCreateMonster eventCreateMonster)
        {
            var currentModelType = _craftController.CurrentModelType;
            
            foreach (var tab in _tabs)
            {
                if (tab.ContainsModel(currentModelType))
                {
                    if (tab.IsOpenModel(currentModelType))
                    {
                        return;
                    }

                    tab.OpenModel(currentModelType);

                    foreach (var tabSave in _tabs)
                    {
                        _modelTypes.Clear();
                        
                        foreach (var tabSaveOpenItem in tabSave.OpenItems)
                        {
                            _modelTypes.Add(tabSaveOpenItem.ModelType);
                        }
                        
                        _save.SaveOpenModelByWorldType(tabSave.WorldType, _modelTypes);
                    }

                    EventStreams.UserInterface.Publish(new EventOpenNewModel());
                    return;
                   
                }
            }
        }

        public float GetPercentOpenByWorldType(WorldType worldType)
        {
            foreach (var tab in _tabs)
            {
                if (tab.WorldType == worldType)
                {
                    var percent = tab.ProgressPercentOpen;
                    return percent;
                }
            }

            return 0f;
        }

        public int GetCountOpenModelByWorldType(WorldType worldType)
        {
            foreach (var tab in _tabs)
            {
                if (tab.WorldType == worldType)
                {
                    return tab.CountOpenItems;
                }
            }

            return 0;
        }

        public Tab GetTabByWorldType(WorldType worldType)
        {
            foreach (var tab in _tabs)
            {
                if (tab.WorldType == worldType)
                {
                    return tab;
                }
            }

            return null;
        }

        private void Open(EventOpenBestiary eventOpenBestiary)
        {
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
            foreach (var tab in _tabs)
            {
                tab.OnClick -= OpenTabByWorldType;
            }
        }
    }
}