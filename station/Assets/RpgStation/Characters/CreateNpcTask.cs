using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CreateNpcTask : BasicTask<BaseCharacter>
    {
        private string _npcId;
        private Vector3 _position;
        private Vector3 _rotation;
        private StationMechanics _mechanics;
        
        public CreateNpcTask(string npcId, Vector3 position, Vector3 rotation)
        {
            Proxy = new ProxyWithRunner();
            _npcId = npcId;
            _position = position;
            _rotation = rotation;
        }

        protected override IEnumerator HandleExecute()
        {
            
            var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            var npcDb = dbSystem.GetDb<NpcDb>();
            var npcMeta = npcDb.GetEntry(_npcId);
            var settingsDb = dbSystem.GetDb<GameSettingsDb>();
            _mechanics = settingsDb.Get().Mechanics;


            if (npcMeta != null)
            {
                FinishTaskWithError("could not find npc meta for npc: " + _npcId);
            }


            var characterData = new List<object>
            {
                npcMeta.RaceId, _npcId, "male"
            };
            string prefabId = npcMeta.PrefabId;

            var op = _mechanics.OnCreateCharacter(new PlayerCharacterType(), characterData.ToArray(), OnPlayerInstanced,
                prefabId);

            Debug.Log(1);

            if (op != null)
            {
                Debug.Log(2);
                while (op.Value.IsDone == false)
                {
                    Debug.Log(3);
                    yield return null;
                }

                var instance = op.Value.Result;
                var component = instance.GetComponent<BaseCharacter>();
                Debug.Log(4);

                if (component != null)
                {
                    _mechanics.OnBuildNpc(component, npcMeta, _npcId);
                    Debug.Log(5);

                    component.transform.position = _position;
                    component.transform.Rotate(_rotation);
                    component.Control.SetRotation(Quaternion.identity.eulerAngles);
                    component.AddMeta("identity", IdentityType.Npc.ToString());

                    component.Stats.SetVitalsValue(GetVitalsValues(npcMeta));
                    FinishTask(component);
                }
                else
                {
                    FinishTaskWithError("no component found for on player prefab");
                }
            }

            else
            {
                FinishTaskWithError("failed to load npc");
            }


            yield return null;
        }
        
        public List<IdIntegerValue> GetVitalsValues(NpcModel npc)
        {
            var vitalStatus = new List<IdIntegerValue>();
            if (npc.UseHealth)
            {
                var healthStatus = new IdIntegerValue(npc.HealthVital.Id,-1);
                vitalStatus.Add(healthStatus);
            }

            foreach (var energyData in npc.EnergyVitals)
            {
                var energyStatus = new IdIntegerValue(energyData.Id,-1);
                vitalStatus.Add(energyStatus);
            }

            return vitalStatus;
        }
        
        private void OnPlayerInstanced(GameObject instance)
        {

        }
    }
}

