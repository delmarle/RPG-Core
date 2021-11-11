
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
   public class UiCharacterSelectionListWidget : MonoBehaviour
{
     #region [[ FIELDS ]]

        [SerializeField] private UiCharacterPortraitWidget _memberPrefab = null;
        [SerializeField] private Transform _root = null;
  
        
        private ICharacterSwitchable[] _componentsToSwitch;
        private Dictionary<BaseCharacter, UiCharacterPortraitWidget> _map = new Dictionary<BaseCharacter, UiCharacterPortraitWidget>();
        private TeamSystem _teamsystem;
        #endregion

        public void Awake()
        {
            _teamsystem = GameInstance.GetSystem<TeamSystem>();
            foreach (var member in _teamsystem.GetTeamMembers())
            {
                OnMemberAdded(member);
            }

            OnCharacterSelected(_teamsystem.GetTeamMembers().FirstOrDefault());
            
            GameGlobalEvents.OnCharacterAdded.AddListener(OnMemberAdded);
            GameGlobalEvents.OnCharacterRemoved.AddListener(OnMemberRemoved);
        }

        public void ApplyTarget(ICharacterSwitchable[] targets)
        {
            _componentsToSwitch = targets;
        }
        private void OnDestroy()
        {
            GameGlobalEvents.OnCharacterAdded.RemoveListener(OnMemberAdded);
            GameGlobalEvents.OnCharacterRemoved.RemoveListener(OnMemberRemoved);
        }

        #region DELEGATES

        private void OnMemberAdded(BaseCharacter member)
        {
            var instance = Instantiate(_memberPrefab, _root);
            instance.Setup(member, OnCharacterSelected);
            _map.Add(member, instance);
        }

        private void OnMemberRemoved(BaseCharacter member)
        {
            var uiObject = _map[member];
            _map.Remove(member);
            Destroy(uiObject.gameObject);
        }

        private void OnCharacterSelected(BaseCharacter focusCharacter)
        {
            foreach (var entry in _map)
            {
                if (entry.Key == focusCharacter)
                {
                    entry.Value.SetSelected();
                }
                else
                {
                    entry.Value.SetNotSelected();
                }
            }

            if (_componentsToSwitch != null)
            {
                foreach (var component in _componentsToSwitch)
                {
                    component.SwitchCharacter(focusCharacter);
                }
            }
        }
        #endregion
}

   public interface ICharacterSwitchable
   { 
       void SwitchCharacter(BaseCharacter character);
   }
}


