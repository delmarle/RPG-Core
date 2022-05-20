using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Station
{
    public class GameInstance : MonoBehaviour
    {
        #region FIELDS
        public List<BaseDb> _dbsToLoad = new List<BaseDb>();
        public GameMode DefaultGameMode;
 
        
        public static GameObject RootGameObject;
        private readonly Dictionary<Type, Component> _systemsMap = new Dictionary<Type, Component>();
        private static GameInstance _instance;
        private static GameMode _currentGameMode;
        public static GameMode GetCurrentGameMode => _currentGameMode;
        private static DbSystem _dbs;
        #endregion
        #region Mono
        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            if (Application.isPlaying && gameObject.activeInHierarchy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            foreach (var value in _systemsMap.Select(entry => (BaseSystem) entry.Value).Where(value => value))
            {
                value.Dispose();
            }

            _instance = null;
        }
        #endregion

        public static List<BaseDb> GetDbToLoad => _instance._dbsToLoad;
        private void Init()
        {
            _instance = this;
            Addressables.InitializeAsync();
            RootGameObject = gameObject;
            var list = ReflectionUtils.FindDerivedClasses(typeof(BaseSystem));
            //add to mapping
            foreach (var localType in list)
            {
                var addedComponent = gameObject.AddComponent(localType);
                _systemsMap.Add(localType, addedComponent);
            }
            //init each
            foreach (var system in _systemsMap.Select(entry => (BaseSystem) entry.Value))
            {
                system.Init(this);
            }

            _dbs = FindSystem<DbSystem>();
        }

        #region SYSTEMS & DBS
        private T FindSystem<T>() where T : BaseSystem
        {
            _systemsMap.TryGetValue(typeof(T), out var found);
            return (T)found;
        }

        public static T GetSystem<T>() where T : BaseSystem
        {
            if (_instance == null)
            {
                return null;
            }

            _instance._systemsMap.TryGetValue(typeof(T), out var found);
            return (T)found;
        }

        public static T GetDb<T>()
        {
            return _dbs.GetDb<T>();
        }
        
        #endregion

        #region GAMEMODE

        public static void StartGameMode()
        {
            _instance.StartGm();
        }
        private void StartGm()
        {
            //has scene override
            var sceneIdentity = FindObjectOfType<SceneIdentity>();
            if (sceneIdentity != null || sceneIdentity.GetGameModeOverride != null)
            {
                _currentGameMode = sceneIdentity.GetGameModeOverride;
            }
            else
            {
                _currentGameMode = DefaultGameMode;
            }

            if (_currentGameMode != null)
            {
                _currentGameMode.DoEnterScene();
            }
            
            var saveSystem = GetSystem<SavingSystem>();
            saveSystem.TryEnterSceneSaveTrigger();
        }

        public static void EndGameMode()
        {
            _instance.EndGm();
        }
        private void EndGm()
        {
            GameGlobalEvents.OnBeforeLeaveScene?.Invoke();
            var saveSystem = GetSystem<SavingSystem>();
            saveSystem.TryExitSceneSaveTrigger();
            if (_currentGameMode != null)
            {
                _currentGameMode.DoExitScene();
                _currentGameMode = null;
            }
        }
        #endregion
    }

}

