using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class CharacterInventoryTabs : MonoBehaviour
    {
#region FIELDS

        private const string INVENTORIES_SHARED = "inventory_shared";
        private const string INVENTORIES_MULTIPLE = "inventory_multiple";

        [SerializeField] private BaseAnimation _animation = null;
        [SerializeField] private UiContainerWidget _commonInventoryWidget = null;
        [SerializeField] private Transform _inventoriesRoot = null;
        
        private Dictionary<string, UiContainerWidget> _containerWidgets = new Dictionary<string, UiContainerWidget>();
        private TeamSystem _teamSystem;
        private PlayerInventorySystem _inventorySystem;
        private ItemsSettingsDb _itemsSettingsDb;
#endregion
        #region INITIALIZATION

        public void Initialize()
        {
            _teamSystem = RpgStation.GetSystem<TeamSystem>();

            _inventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
            _itemsSettingsDb = RpgStation.GetDb<ItemsSettingsDb>();
            var containerSettings = _itemsSettingsDb.Get().ContainerSettings;
            
            if (containerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
            {

                var character = _teamSystem.GetTeamMembers().FirstOrDefault();
                var containerReference = new ContainerReference(character.GetCharacterId(), _inventorySystem);
                _commonInventoryWidget.Init(containerReference);
                
                _containerWidgets.Add(PlayerInventorySystem.PLAYER_INVENTORY_KEY, _commonInventoryWidget);
                _animation.PlayState(INVENTORIES_SHARED);
            }
            if (containerSettings.PlayerInventoryType == PlayerInventoryType.PerCharacter)
            {
                foreach (var member in _teamSystem.GetTeamMembers())
                {
                    var instance = Instantiate(_commonInventoryWidget, _inventoriesRoot);
                    _containerWidgets.Add(member.GetCharacterId() , instance);
                }
                _animation.PlayState(INVENTORIES_MULTIPLE);
            }

        }

        public void RegisterAllEvents()
        {
            foreach (var container in _containerWidgets.Values)
            {
                container.RegisterEvents();
                container.UpdateUiSlots();
            }
        }

        public void UnRegisterAllEvents()
        {
            foreach (var container in _containerWidgets.Values)
            {
                container.UnregisterEvents();
            }
        }

        private void OnEnable()
        {
            GameGlobalEvents.OnCharacterAdded.AddListener(OnCharacterAdded);
        }

        private void OnDisable()
        {
            GameGlobalEvents.OnCharacterAdded.RemoveListener(OnCharacterAdded);
        }

        #endregion
#region ADD/REMOVE CHARACTER
        private void OnCharacterAdded(BaseCharacter character)
        {
            var containerSettings = _itemsSettingsDb.Get().ContainerSettings;
                Debug.Log($"character added on ui event: {character}");
                if (containerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
                {
                    //do nothing
                }
                if (containerSettings.PlayerInventoryType == PlayerInventoryType.PerCharacter)
                {
                }


                _containerWidgets.Add(character.GetCharacterId(), _commonInventoryWidget);
        }

#endregion
        public void OnSelectCharacter(string character)
        {
            
        }
    }
}

