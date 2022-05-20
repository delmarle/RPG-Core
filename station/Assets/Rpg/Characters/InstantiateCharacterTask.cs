using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Station
{
    
    public class InstantiateCharacterTask : BasicTask<BaseCharacter>
    {
        private string _prefabId;
        private BaseCharacterData _baseData;
        private StationMechanics _mechanics;
        private object[] _data;
        
        

        public InstantiateCharacterTask(string prefabId, BaseCharacterData baseData,object[] data, StationMechanics mechanics)
        {
            Proxy = new ProxyWithRunner();
            
            _prefabId = prefabId;
            _baseData = baseData;
            _data = data;
            _mechanics = mechanics;
        }

        protected override IEnumerator HandleExecute()
        {
            var operationHandle = Addressables.InstantiateAsync(_prefabId, _baseData.Position, Quaternion.Euler(_baseData.Rotation));
            while (operationHandle.IsDone == false)
            {
                yield return null;
            }

            var component = operationHandle.Result.GetComponent<BaseCharacter>();
            component.transform.SetPositionAndRotation(_baseData.Position, Quaternion.Euler(_baseData.Rotation));
            component.AddMeta(StationConst.CHARACTER_TYPE, _baseData.CharacterType);
            _mechanics.OnBuildCharacter(component, _baseData, _data);
            
            FinishTask(component);
        }
    }
    
    public class BaseCharacterData
    {
        public string CharacterId;
        public BaseCharacterType CharacterType;
        public Vector3 Position;
        public Vector3 Rotation;
        public string RaceId;
        public string Identifier;
        public string Gender;
    }
}

