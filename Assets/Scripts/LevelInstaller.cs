using DefaultNamespace;
using FPS;
using FPS.AttackSystem;
using FPS.EnemyComponent;
using FPS.GenerationSystem;
using GapBetweenRunnerAndShooter.CraftSystem;
using Joystick_and_Swipe;
using Level_selection.Bestiary_system;
using Level_selection.Map_system;
using ParticleFactory;
using Runner.GateSystem;
using Runner.GeneratorSystem;
using Runner.ReagentSystem;
using SaveAndLoadSystem;
using UnityEditor;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField]
    private DynamicJoystick _joystickController;
    [SerializeField]
    private ParticleSystemFactory.Settings _settingsParticleSystemFactory;
    [SerializeField]
    private StripGenerator.Settings _settingsStripGenerator;
    [SerializeField]
    private StripPool.Settings _settingsStripPool;
    [SerializeField]
    private EnemyPool.Settings _settingsEnemyPool;
    [SerializeField]
    private ReagentProvider.Settings _settingsReagentProvider;
    [SerializeField]
    private ModelColorProvider.Settings _settingsReagentColorProvider;
    [SerializeField]
    private ReagentModelProvider.Settings _settingsReagentModelProvider;
    [SerializeField]
    private CraftController.Settings _settingsCraftController;
    [SerializeField]
    private Generator.Settings _settingsGenerator;
    [SerializeField]
    private ProjectileFactory.Settings _settingsProjectileFactory;
    [SerializeField]
    private MonsterSpawner.Settings _settingsMonsterSpawner;
    [SerializeField]
    private LevelManager.Settings _settingsLevelManager;
    [SerializeField]
    private Map.Settings _settingsMap;
    [SerializeField]
    private Bestiary _bestiary;
    [SerializeField]
    private IconReagentProvider.Settings _settingsIconProvider;
    [SerializeField]
    private ColorRegionProvider.Settings _settingsColorRegionProvider;
    [SerializeField]
    private ReagentNameProvider.Settings _settingsReagentNameProvider;

    public override void InstallBindings()
    {
        Container.BindInstance(_settingsParticleSystemFactory);
        Container.BindInstance(_settingsLevelManager);
        Container.BindInstance(_settingsStripGenerator);
        Container.BindInstance(_settingsStripPool);
        Container.BindInstance(_settingsReagentProvider);
        Container.BindInstance(_settingsReagentColorProvider);
        Container.BindInstance(_settingsReagentModelProvider);
        Container.BindInstance(_settingsCraftController);
        Container.BindInstance(_settingsEnemyPool);
        Container.BindInstance(_settingsGenerator);
        Container.BindInstance(_settingsProjectileFactory);
        Container.BindInstance(_settingsMonsterSpawner);
        Container.BindInstance(_settingsMap);
        Container.BindInstance(_bestiary);
        Container.BindInstance(_settingsIconProvider);
        Container.BindInstance(_settingsColorRegionProvider);
        Container.BindInstance(_settingsReagentNameProvider);

        Container.BindInstance(_joystickController);

        Container.Bind<ReagentModelProvider>().AsSingle().WithArguments(transform).NonLazy();
        Container.Bind<ModelColorProvider>().AsSingle().NonLazy();
        Container.Bind<Save>().AsSingle().NonLazy();
        Container.Bind<ReagentNameProvider>().AsSingle().NonLazy();
        Container.Bind<Load>().AsSingle().NonLazy();
        Container.Bind<IconReagentProvider>().AsSingle().NonLazy();
        Container.Bind<ProjectileFactory>().AsSingle().WithArguments(transform).NonLazy();
        Container.Bind<ReagentProvider>().AsSingle().NonLazy();
        Container.Bind<ColorRegionProvider>().AsSingle().NonLazy();
        Container.Bind<EnemyPool>().AsSingle().WithArguments(transform).NonLazy();
        
        Container.BindInterfacesAndSelfTo<CraftController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<StripPool>().AsSingle().WithArguments(transform).NonLazy();
        Container.BindInterfacesAndSelfTo<StripGenerator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Generator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Map>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MonsterSpawner>().AsSingle().WithArguments(transform).NonLazy();
    }
    
#if UNITY_EDITOR
    [MenuItem("Tools/ClearSave")]
    public static void ClearSave()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}