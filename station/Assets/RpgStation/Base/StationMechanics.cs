
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{
    public abstract class StationMechanics : ScriptableObject
    {
        protected RpgStation _station;
        [SerializeField] private GameObject _cameraReference = null;
        public AssetReference LoadingScreen;
        public GameObject UiPrefab;

        public void Init(RpgStation station)
        {
            _station = station;
        }

      

        public virtual CameraController CreateCamera()
        {
            var previousCamera = Camera.main;
            if (previousCamera!= null)
            {
                Destroy(previousCamera.gameObject);
            }

            var instance = Instantiate(_cameraReference);
            instance.name = _cameraReference.name;
            return instance.GetComponentInChildren<CameraController>();
        }

        public  virtual void CreateUi()
        {
            Instantiate(UiPrefab);
        }

        public abstract void OnReceiveEvent(string eventName, object[] localParams);
        public abstract string Description();
        public abstract AsyncOperationHandle<GameObject>? OnCreateCharacter(PlayerCharacterType typeHandler, object[] data, Action<GameObject> onPlayerInstanced, string prefabId);
        public abstract void OnBuildPlayer(BaseCharacter character, PlayersData save, PlayerClassModel classData);
        public abstract void OnBuildNpc(BaseCharacter character, NpcModel model, string npcId);
        
        public virtual IFactionHandler FactionHandler()
        {
            return null;
        }

    }
}
