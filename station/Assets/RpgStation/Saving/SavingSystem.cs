using System;
using System.Collections.Generic;
using UnityEngine;


namespace Station
{
    public class SavingSystem : BaseSystem
    {
        
        private readonly Dictionary<Type, object> map = new Dictionary<Type,object>();

        protected override void OnInit()
        {

            AddModule<PlayersSave>(new PlayersSave());
            GetModule<PlayersSave>().Initialize();
            
            AddModule<SpawnerSave>(new SpawnerSave());
            GetModule<SpawnerSave>().Initialize();
        }

        protected override void OnDispose()
        {
            
        }


        public T GetModule<T>()
        {
            if (!map.ContainsKey(typeof(T)))
            {
                Debug.LogError("could not initialize this save module: "+typeof(T));
            }

            var d = map[typeof(T)];
     
            return (T) d;
        }

        public void AddModule<T>(object ModuleToAdd)
        {
            var module = (T)ModuleToAdd;
            map.Add(module.GetType(), module);
            
        }
    }

    public class SaveModule<T>
    {
        private bool _isLoaded = false;
        protected IContainer _dataContainer;
        protected string fullPath;
        public T Value { get; set; }
        
        public void Initialize()
        {
            string fileName = GetType().Name;
            fullPath = PathUtils.SavePath() + fileName;
            BuildDefaultData();
            IContainer container = DataStorageGenerator.GenerateDataStorage();
                       
            container.SetPath(fullPath);           
            _dataContainer = container;
            if (!System.IO.File.Exists(fullPath))
            {
                Save();
            }
            Load();
        }
        
        public void Save()
        {
            FetchData();
            Write();
        }

        public void Write()
        {
            _dataContainer.Save(Value);
        }

        public void Load()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                var tempData = _dataContainer.Load<T>();
                if (tempData != null)
                {
                    Value = tempData;
                }
            }

            ApplyData();
        }

        protected virtual void FetchData()
        {
            Debug.Log("FetchData:"+GetType());
        }

        protected virtual void ApplyData()
        {
        }

        protected void BuildDefaultData()
        {
            Value = default;

        }

    }


}