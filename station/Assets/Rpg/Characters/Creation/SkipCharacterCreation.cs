using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class SkipCharacterCreation : BaseCharacterCreation
    {
        [SerializeField] private List<ItemStack> _defaultItems = new List<ItemStack>();
        
        //cache

        private SavingSystem _savingSystem;


        private const string CLASS_ID_1 = "a59e40c4-25f1-4d10-8446-609a31b7856d";
        private const string CLASS_ID_2 = "0e2d0377-2d6e-4bb0-ba06-fe452f782d57";
        public override void Init(GameInstance station)
        {
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
        }

        public override bool HasData()
        {
            var module = _savingSystem.GetModule<PlayersSave>();
            int playerSaveCount = module.Value?.Count ?? 0;
            return playerSaveCount > 0;
        }

        public override void StartSequence()
        {
            var module = _savingSystem.GetModule<PlayersSave>();
            var factionSettingsDb = GameInstance.GetDb<FactionSettingsDb>();
            var playerClassDb = GameInstance.GetDb<PlayerClassDb>();
            var defaultFaction = factionSettingsDb.Get().DefaultPlayerFaction;
            var class1 = playerClassDb.GetEntry(CLASS_ID_1);
            var class2 = playerClassDb.GetEntry(CLASS_ID_2);

         
            var player1 =CharacterUtils.CreateCharacterSave(class1, "Damien", "1316bc69-70ad-4577-9cfd-e29b35c8a18b",
                CLASS_ID_1, "male", defaultFaction,"93436f6d-41e9-441a-a2a3-58e5cc7b4e4b", Vector3.up + Vector3.left);
            var player2 = CharacterUtils.CreateCharacterSave(class2, "Enzo", "1316bc69-70ad-4577-9cfd-e29b35c8a18b",
                CLASS_ID_2, "male", defaultFaction,"93436f6d-41e9-441a-a2a3-58e5cc7b4e4b", Vector3.up + Vector3.left);
            module.Value = new Dictionary<string, PlayersData>();
            string player1Key = Guid.NewGuid().ToString();
            module.AddPlayer(player1Key, player1);
            string player2Key = Guid.NewGuid().ToString();
            module.AddPlayer(player2Key, player2);
            var destinationModel = new DestinationModel
            {
                SceneId = "93436f6d-41e9-441a-a2a3-58e5cc7b4e4b", 
                SpawnId = 0
            };
            var rpgSceneSystem =  GameInstance.GetSystem<RpgSceneSystem>();
            rpgSceneSystem.InjectDestinationInSave(destinationModel);
            module.Save(false);
            //go to zone
            TravelModel model = new TravelModel();
            model.SceneName = "zone_01";

            InventoryUtils.CreatePlayerInventory(_defaultItems);

            GameGlobalEvents.OnEnterGame.Invoke();
            rpgSceneSystem.TravelToZone(model);
        }

        public override string Description()
        {
            return "this will skip character creation and build a default save";
        }
    }
}

