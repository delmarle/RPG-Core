using System;
using UnityEngine;

namespace Station
{
    public class SaveListener : MonoBehaviour
    {
        private void OnEnable()
        {
            GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnTriggerSave);
            GameGlobalEvents.OnTriggerSceneSave.AddListener(OnTriggerSave);
        }

        private void OnDisable()
        {
            GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnTriggerSave);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                OnTriggerSave();
        }

        private void OnTriggerSave()
        {
            var savingSystem = RpgStation.GetSystemStatic<SavingSystem>();
            var playerModule = savingSystem.GetModule<PlayersSave>();
            playerModule.Save();
        }
    }


}
