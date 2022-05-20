using System;
using System.Collections.Generic;
using Station.Data;
using UnityEngine;


namespace Station
{
    public class SavingSystem : BaseSystem
    {
        private readonly Dictionary<Type, ISaveModule> _modules = new Dictionary<Type,ISaveModule>();
        private readonly  Dictionary<Type, IAreaSave> _modulesArea = new Dictionary<Type,IAreaSave>();

        public CoreGameSave MainSave = new CoreGameSave();
        protected override void OnInit()
        {
            RegisterSceneRelatedCallback();
            var modules = ReflectionUtils.FindSaveModuleTypes();
            foreach (var mod in modules)
            {
                var moduleInstance = Activator.CreateInstance(mod);
                var saveCast = (ISaveModule) moduleInstance;
                saveCast.Initialize();
                AddModule(moduleInstance);
            }
            InitializeMainSave();
        }

        protected override void OnDispose()
        {
            UnRegisterSceneRelatedCallback();
        }

        protected override void OnDataBaseReady()
        {
            
        }


        public T GetModule<T>()
        {
            if (_modules.ContainsKey(typeof(T)))
            {
                var d = _modules[typeof(T)];
                return (T) d;
            }

            if (_modulesArea.ContainsKey(typeof(T)))
            {
                var d = _modulesArea[typeof(T)];
                return (T) d;
            }

            Debug.LogError($"The save module {typeof(T)} was not initialized");
            return default;
        }

        public void AddModule(object ModuleToAdd)
        {
   
            if (ModuleToAdd is IAreaSave)
            {
                _modulesArea.Add(ModuleToAdd.GetType(), (IAreaSave)ModuleToAdd);
            }
            else
            {
                _modules.Add(ModuleToAdd.GetType(), (ISaveModule)ModuleToAdd);
            }
        }
        
        #region SCENE PART RELATED
        private void RegisterSceneRelatedCallback()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneInitialize);
        }

   

        private void UnRegisterSceneRelatedCallback()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
        }
        

        private void OnSceneInitialize(SceneType sceneType)
        {

            
        }

        public void TryEnterSceneSaveTrigger()
        {
            Debug.Log($" the scene is SAVED ");
            var gameMode = GameInstance.GetCurrentGameMode;
            if (gameMode != null)
            {
                if (gameMode.SaveSettings.SaveOnEnter)
                {
                    FetchAndSaveAll();
                }
            }
        }
        public void TryExitSceneSaveTrigger()
        {
            Debug.Log($" the scene is SAVED ");
            var gameMode = GameInstance.GetCurrentGameMode;
            if (gameMode != null)
            {
                if (gameMode.SaveSettings.SaveOnExit)
                {
                    FetchAndSaveAll();
                }
            }
        }

        private void FetchAndSaveAll()
        {
            foreach (var module in _modules.Values)
            {
                module.FetchData();
                module.Save();
            }
            
            foreach (var module in _modulesArea.Values)
            {
                var castedModule = (ISaveModule)module;
                castedModule.FetchData();
                castedModule.Save();
            }
            
            //MainSave.FetchData();
            //MainSave.Save();
            SaveMain();
        }
        #endregion
        
        #region Main Save
        private bool _isLoaded = false;
        private IDataContainer _dataContainer;
        private string _fullPath =>  PathUtils.SavePath()+ "MainSave";
        
        public void InitializeMainSave()
        {
            BuildMainSaveData();
            _dataContainer = DataStorageGenerator.GenerateDataStorage();
            _dataContainer.SetPath(_fullPath);

            if (!System.IO.File.Exists(_fullPath))
            {
                SaveMain();
            }
            LoadMainSave();
        }
        
        private void BuildMainSaveData()
        {
            MainSave = new CoreGameSave();
        }
        
        public void SaveMain()
        {
            WriteMainSave();
        }

        private void WriteMainSave()
        {
            _dataContainer.Save(MainSave);
        }

        public void LoadMainSave(bool force = false)
        {
            if (!_isLoaded || force)
            {
                _dataContainer.SetPath(_fullPath);       
                _isLoaded = true;
                var tempData = _dataContainer.Load<CoreGameSave>();
                if (tempData != null)
                {
                    MainSave = tempData;
                }
            }
        }

        #endregion
    }

    public interface ISaveModule
    {
        void Initialize();
        void FetchData();
        void Save();
    }
    public abstract class SaveModule<T> : ISaveModule
    {
        private bool _isLoaded = false;
        private IDataContainer _dataContainer;
        private string _fullPath =>  PathUtils.SavePath()+ GetType().Name;
        public T Value { get; set; }
        
        public void Initialize()
        {
            BuildDefaultData();
            _dataContainer = DataStorageGenerator.GenerateDataStorage();
            _dataContainer.SetPath(_fullPath);

            if (!System.IO.File.Exists(_fullPath))
            {
                Save();
            }
            Load();
        }
        public virtual void FetchData()
        {
        }

        public void Save()
        {
            FetchData();
            Write();
        }

        private void Write()
        {
            _dataContainer.Save(Value);
        }

        public void Load(bool force = false)
        {
            if (!_isLoaded || force)
            {
                _dataContainer.SetPath(_fullPath);       
                _isLoaded = true;
                var tempData = _dataContainer.Load<T>();
                if (tempData != null)
                {
                    Value = tempData;
                }
            }
        }
        

        private void BuildDefaultData()
        {
            Value = default;
        }

    }

    public interface IAreaSave
    {
        void Save();
        void Load(string areaName);
    }

    public abstract class AreaSaveModule<T> : IAreaSave, ISaveModule
    {
        private IDataContainer _dataDataContainer;
        private string _fullPath;
        public T Value { get; set; }
        
        public void Initialize()
        {
            BuildDefaultData();
            IDataContainer dataContainer = DataStorageGenerator.GenerateDataStorage();
            _dataDataContainer = dataContainer;
        }

        public virtual void FetchData()
        {
            //TODO get all containers
        }
        
        public void Save()
        {
            if (Value == null) return;
            
            Write();
        }

        private void Write()
        {
            _dataDataContainer.Save(Value);
        }

        public void Load(string areaName)
        {
            _fullPath = $"{PathUtils.SavePath()+areaName}/{GetType().Name}";
            _dataDataContainer.SetPath(_fullPath);           

            if (!System.IO.File.Exists(_fullPath))
            {
                Save();
            }
            
            var tempData = _dataDataContainer.Load<T>();
            if (tempData != null)
            {
                Value = tempData;
            }
        }
        

        private void BuildDefaultData()
        {
            Value = default;
        }
    }


}