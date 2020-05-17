
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
        
        private Dictionary<BaseCharacter, UiCharacterPortraitWidget> _map = new Dictionary<BaseCharacter, UiCharacterPortraitWidget>();
        private TeamSystem _teamsystem;
        #endregion

        public void Awake()
        {
            _teamsystem = RpgStation.GetSystemStatic<TeamSystem>();
            foreach (var member in _teamsystem.GetTeamMembers())
            {
                OnMemberAdded(member);
            }

            OnLeaderChanged(_teamsystem.GetTeamMembers().FirstOrDefault());
            
            GameGlobalEvents.OnCharacterAdded.AddListener(OnMemberAdded);
            GameGlobalEvents.OnCharacterRemoved.AddListener(OnMemberRemoved);
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
            instance.Setup(member, null);
            _map.Add(member, instance);
        }

        private void OnMemberRemoved(BaseCharacter member)
        {
            var uiObject = _map[member];
            _map.Remove(member);
            Destroy(uiObject.gameObject);
        }

        private void OnLeaderChanged(BaseCharacter leader)
        {
            foreach (var entry in _map)
            {
                if (entry.Key == leader)
                {
                    entry.Value.SetSelected();
                }
                else
                {
                    entry.Value.SetNotSelected();
                }
            }
        }
        #endregion
}
}

