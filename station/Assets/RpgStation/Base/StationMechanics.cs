
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{
    public abstract class StationMechanics : ScriptableObject
    {
        #region FIELDS

        [SerializeField] private GameObject _cameraReference = null;
        public AssetReference LoadingScreen;
        public GameObject UiPrefab;
        [SerializeField] private CharacterBuilder[] _charactersBuilders;
        private Dictionary<Type, CharacterBuilder> _builderMap = new Dictionary<Type,CharacterBuilder>();
        
        #endregion
        public void OnEnable()
        {
            RegisterCharacterBuilder();
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

        public virtual BasicTask<BaseCharacter> InstantiateCharacter(BaseCharacterData baseData,object[] data, Action<BaseCharacter> onCharacterInstanced, string prefabId)
        {
            return null;
        }

        public void OnBuildCharacter(BaseCharacter character,BaseCharacterData baseData, object[] data)
        {
            var characterType = character.GetMeta(StationConst.CHARACTER_TYPE);
            if (_builderMap.ContainsKey(characterType.GetType()))
            {
                var MatchingCharacter = _builderMap[characterType.GetType()];
                MatchingCharacter.Build(character, baseData, data);
            }
            else
            {
                Debug.LogError("no character builder for type: "+characterType.GetType());
            }
        }

        public virtual void RegisterCharacterBuilder()
        {
            foreach (var builder in _charactersBuilders)
            {
                var builderType = builder.GetMatchingType();
                if (_builderMap.ContainsKey(builderType))
                {
                    Debug.LogError("the builder for this type is duplicate: "+builderType);
                }
                else
                {
                    _builderMap.Add(builderType, builder);
                }
            }
        }

        public virtual IFactionHandler FactionHandler()
        {
            return null;
        }

    }
}
