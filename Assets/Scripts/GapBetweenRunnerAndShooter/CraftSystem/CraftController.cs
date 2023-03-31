using System;
using System.Collections.Generic;
using Events;
using Runner.ReagentSystem;
using SimpleEventBus.Disposables;
using Tools.SimpleEventBus;
using UnityEngine;

namespace GapBetweenRunnerAndShooter.CraftSystem
{
    public class CraftController : IDisposable
    {
        public ModelType CurrentModelType => _currentModelType;

        private readonly Dictionary<int, List<Recipe>> _recipes = new Dictionary<int, List<Recipe>>();
        
        private readonly CompositeDisposable _subscription;
        private readonly int _maxReagent;
        
        private ModelType _currentModelType = ModelType.None;

        public CraftController(Settings settings)
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventFinish>(SearchStartEvent),
            };

            foreach (var recipe in settings.Recipes)
            {
                var count = recipe.RequiredReagents.Count;
                _maxReagent = _maxReagent > count ? _maxReagent : count;
                
                if (!_recipes.ContainsKey(count))
                {
                    _recipes.Add(count, new List<Recipe>());
                }
                
                _recipes[count].Add(recipe);
            }
        }

        public List<ReagentType> GetReagentRecipeByModelType(ModelType modelType)
        {
            foreach (var keyValuePair in _recipes)
            {
                foreach (var recipe in keyValuePair.Value)
                {
                    if (recipe.ModelType == modelType)
                    {
                        return recipe.RequiredReagents;
                    }
                }
            }

            return null;
        }

        private void SearchStartEvent(EventFinish eventFinish)
        {
            _currentModelType = ModelType.None;
            SearchModel(eventFinish.Boiler.Reagents);
        }

        private void SearchModel(List<Reagent> reagentTypes, int count = -1)
        {
            if (reagentTypes.Count == 0)
            {
                return;
            }

            if (count == 0)
            {
                FindAll(reagentTypes);
                return;
            }

            count = count == -1 ? reagentTypes.Count > _maxReagent 
                ? _maxReagent 
                : reagentTypes.Count 
                : count;

            if (!_recipes.ContainsKey(count))
            {
                SearchModel(reagentTypes, --count);
                return;
            }

            var recipes = _recipes[count];
            
            foreach (var recipe in recipes)
            {
                var numberCoincidences = 0;
                foreach (var reagent in reagentTypes)
                {
                    if (recipe.RequiredReagents.Contains(reagent.ReagentType))
                    {
                        numberCoincidences++;
                    }
                }
                
                if (count == numberCoincidences)
                {
                    _currentModelType = recipe.ModelType;
                    break;
                }
            }

            if (_currentModelType == ModelType.None)
            {
                if (count > 0)
                {
                    SearchModel(reagentTypes, --count);
                }
                else
                {
                    FindAll(reagentTypes);
                }
            }
        }

        private void FindAll(List<Reagent> reagentTypes, int count = -1)
        {
            if (count == -1)
            {
                count = reagentTypes.Count;
            }
            
            foreach (var keyValuePair in _recipes)
            {
                foreach (var recipe in keyValuePair.Value)
                {
                    var numberCoincidences = 0;
                    foreach (var reagent in reagentTypes)
                    {
                        if (recipe.RequiredReagents.Contains(reagent.ReagentType))
                        {
                            numberCoincidences++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count == numberCoincidences)
                    {
                        _currentModelType = recipe.ModelType;
                        break;
                    }
                }

                if (_currentModelType != ModelType.None)
                {
                    break;
                }
            }

            if (_currentModelType == ModelType.None)
            {
                FindAll(reagentTypes, --count);
            }
        }
        
        public void Dispose()
        {
            _subscription?.Dispose();
        }
        
        [Serializable]
        public class Settings
        {
            public List<Recipe> Recipes = new List<Recipe>();
        }

        [Serializable]
        public class Recipe
        {
            public ModelType ModelType;
            public List<ReagentType> RequiredReagents = new List<ReagentType>();
        }
    }
}