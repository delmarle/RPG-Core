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
        private RpgStation _station;
        private SavingSystem _savingSystem;
        private SceneSystem _sceneSystem;

        private const string CLASS_ID_1 = "a59e40c4-25f1-4d10-8446-609a31b7856d";
        private const string CLASS_ID_2 = "0e2d0377-2d6e-4bb0-ba06-fe452f782d57";
        public override void Init(RpgStation station)
        {
            _station = station;
            _savingSystem = RpgStation.GetSystem<SavingSystem>();
            _sceneSystem = RpgStation.GetSystem<SceneSystem>();
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
            var factionSettingsDb = RpgStation.GetDb<FactionSettingsDb>();
            var playerClassDb = RpgStation.GetDb<PlayerClassDb>();
            var defaultFaction = factionSettingsDb.Get().DefaultPlayerFaction;
            var class1 = playerClassDb.GetEntry(CLASS_ID_1);
            var class2 = playerClassDb.GetEntry(CLASS_ID_2);

            var player1 = CreateCharacter(class1, "Damien", CLASS_ID_1, defaultFaction, Vector3.up + Vector3.left);
            var player2 = CreateCharacter(class2, "Enzo", CLASS_ID_2, defaultFaction, Vector3.up + Vector3.left);
 
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
            _sceneSystem.InjectDestinationInSave(destinationModel);
            module.Save();
            //go to zone
            TravelModel model = new TravelModel();
            model.SceneName = "zone_01";

            CreatePlayerInventory();
            
            GameGlobalEvents.OnEnterGame.Invoke();
            _sceneSystem.TravelToZone(model);
        }

        public override string Description()
        {
            return "this will skip character creation and build a default save";
        }

        private void CreatePlayerInventory()
        {

            var settingsDb = RpgStation.GetDb<ItemsSettingsDb>();
            var containerSettings = settingsDb.Get().ContainerSettings;
            var module = _savingSystem.GetModule<PlayerInventorySave>();
            var inventoryList = new ContainersListSave();
            var inventory = new ContainerState(containerSettings.InitialPlayerInventorySize, _defaultItems);
            if (containerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
            {
                
            }
            else if (containerSettings.PlayerInventoryType == PlayerInventoryType.PerCharacter)
            {
            }

            inventoryList.Containers.Add(PlayerInventorySystem.PLAYER_INVENTORY_KEY, inventory);
            module.Value = inventoryList;
            module.Save();
        }

        private PlayersData CreateCharacter(PlayerClassModel classModel, string playerName, string classId, string factionId, Vector3 position)
        {
            int index = 0;
            
            BarStateSave mainBarState = new BarStateSave("main");
            foreach (var ability in classModel.OwnedAbilities)
            {
                var slot = new BarSlotState(LinkType.Ability, index);
                mainBarState.Slots.Add(slot);
                index++;
            }
         
            var linkBar = new List<BarStateSave> {mainBarState};

            var player = new PlayersData
            {
                Name = playerName,
                ClassId = classId,
                RaceId = "1316bc69-70ad-4577-9cfd-e29b35c8a18b",
                GenderId = "male",
                LastZoneId = "93436f6d-41e9-441a-a2a3-58e5cc7b4e4b",
                LastPosition = position,
                LastRotation = Vector3.zero,
                FactionId = factionId,
                LearnedActiveAbilitiesList = classModel.OwnedAbilities,
                LearnedPassiveAbilitiesList = classModel.OwnedPassiveAbilities,
                LearnedSkillList = classModel.OwnedSkills,
                BarStates = linkBar,
                VitalStatus = new List<IdIntegerValue>()
            };
            //set vitals
            if (classModel.UseHealth)
            {
                var healthStatus = new IdIntegerValue(classModel.HealthVital.Id,-1);
                player.VitalStatus.Add(healthStatus);
            }

            foreach (var energyData in classModel.EnergyVitals)
            {
                var energyStatus = new IdIntegerValue(energyData.Id,-1);
                player.VitalStatus.Add(energyStatus);
            }

            return player;
        }
    }
}

