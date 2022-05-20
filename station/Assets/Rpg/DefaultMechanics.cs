using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{
    [CreateAssetMenu]
    public class DefaultMechanics : StationMechanics
    {
        private DefaultFactionHandler _factionHandler;
     
 
        public override BasicTask<BaseCharacter> InstantiateCharacter(BaseCharacterData baseData,object[] data, Action<BaseCharacter> onCharacterInstanced, string prefabId)
        {
            _factionHandler = new DefaultFactionHandler();

            if (data.Length == 0)
            {
                Debug.Log("need update");
            }

            if (string.IsNullOrEmpty(prefabId))
            {
                Debug.Log("MISSING CHARACTER address");
               
            }
            else
            {
           
                var createCharTask = new InstantiateCharacterTask(prefabId, baseData, data, this);
                createCharTask.Execute();
                return createCharTask;
            }

            return null;
        }

        public override void OnReceiveEvent(string eventName, object[] localParams)
        {
            
        }

        public override IFactionHandler FactionHandler()
        {
            return _factionHandler;
        }
        
        public override string Description()
        {
            return "this is the default mechanics used as demo";

        }
    }
}

