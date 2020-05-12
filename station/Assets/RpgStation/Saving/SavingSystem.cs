using System;
using System.Collections.Generic;
using UnityEngine;


namespace Station
{
    public class SavingSystem : BaseSystem
    {
        
        private readonly Dictionary<Type, object> _modules = new Dictionary<Type,object>();
        private readonly  Dictionary<Type, IAreaSave> _modulesArea = new Dictionary<Type,IAreaSave>();

        protected override void OnInit()
        {
            RegisterSceneRelatedCallback();
            AddModule<PlayersSave>(new PlayersSave());
            GetModule<PlayersSave>().Initialize();
            
            AddModule<PlayerInventorySave>(new PlayerInventorySave());
            GetModule<PlayerInventorySave>().Initialize();
            
            AddModule<SpawnerSave>(new SpawnerSave());
            GetModule<SpawnerSave>().Initialize();
            
            AddModule<AreaContainersSave>(new AreaContainersSave());
            GetModule<AreaContainersSave>().Initialize();
        }

        protected override void OnDispose()
        {
            UnRegisterSceneRelatedCallback();
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

        public void AddModule<T>(object ModuleToAdd)
        {
            var module = (T) ModuleToAdd;
            if (module is IAreaSave)
            {
                _modulesArea.Add(module.GetType(), (IAreaSave)module);
            }
            else
            {
                _modules.Add(module.GetType(), module);
            }
        }
        
        #region SCENE PART RELATED
        private void RegisterSceneRelatedCallback()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnSceneSave);
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneInitialize);
            GameGlobalEvents.OnTriggerSceneSave.AddListener(OnSceneSave);
        }

   

        private void UnRegisterSceneRelatedCallback()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnSceneSave);
        }
        

        private void OnSceneInitialize()
        {
           // Debug.Log($" the scene is INIT {GetType()} Path: ");
            
        }
        
        private void OnSceneSave()
        {
           // Debug.Log($" the scene is SAVED {GetType()} Path: ");
        }

        #endregion
    }
    

    public class SaveModule<T> 
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

        protected virtual void FetchData()
        {
        }

        public void Save()
        {
            FetchData();
            Debug.Log($"SAVING: {GetType()} -- {_fullPath}");
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

    public class AreaSaveModule<T> : IAreaSave
    {
        private IDataContainer DataDataContainer;
        private string fullPath;
        public T Value { get; set; }
        
        public void Initialize()
        {
            BuildDefaultData();
            IDataContainer dataContainer = DataStorageGenerator.GenerateDataStorage();
            DataDataContainer = dataContainer;
        }

        public void Save()
        {
            Debug.Log($"SAVING: {GetType()} -- {fullPath}");
            Write();
        }

        private void Write()
        {
            DataDataContainer.Save(Value);
        }

        public void Load(string areaName)
        {
            fullPath = $"{PathUtils.SavePath()+areaName}/{GetType().Name}";
            DataDataContainer.SetPath(fullPath);           

            if (!System.IO.File.Exists(fullPath))
            {
                Save();
            }
            
            var tempData = DataDataContainer.Load<T>();
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