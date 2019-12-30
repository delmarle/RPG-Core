using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiTeamPanel : UiPanelAnim
    {
        [SerializeField] private LayoutGroup _playerRoot = null;
        [SerializeField] private UiCharacterPortraitWidget _portraitPrefab = null;
        [SerializeField] private UiCharacterTargetWidget _targetWidget = null;
        [SerializeField] private UiCharacterHotBarSwitcher _uiHotBarSwitcher = null;
        private GenericUiList<BaseCharacter> _PlayerWidgets = null;
        private TeamSystem _teamSystem = null;

        
        protected override void Start()
        {
            base.Start();
            PanelSystem.OpenPanel<UiTeamPanel>();
        }

        protected override void Awake()
        {
            base.Awake();
          
            _teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            _PlayerWidgets = new GenericUiList<BaseCharacter>(_portraitPrefab.gameObject, _playerRoot);
            Subscribe();
            SetList(_teamSystem.GetTeamMembers());
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }


        private void SetList(IEnumerable<BaseCharacter> data)
        {
            _PlayerWidgets.Generate<UiCharacterPortraitWidget>(data, (entry, item) => { item.Setup(entry); });
        }

        private void Subscribe()
        {
            TeamSystem.OnLeaderChanged.AddListener(OnLeaderChanged);
        }
        
        
        private void UnSubscribe()
        {
            TeamSystem.OnLeaderChanged.RemoveListener(OnLeaderChanged);
        }


        private void OnLeaderChanged(BaseCharacter FollowedCharacter)
        {
            _targetWidget.Setup(FollowedCharacter);
        }
    }

}

