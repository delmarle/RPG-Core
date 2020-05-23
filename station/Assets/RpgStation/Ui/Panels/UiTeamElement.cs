using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiTeamElement : UiPanel
    {
        [SerializeField] private LayoutGroup _playerRoot = null;
        [SerializeField] private UiCharacterPortraitWidget _portraitPrefab = null;
        [SerializeField] private UiCharacterTargetWidget _targetWidget = null;

        private GenericUiList<BaseCharacter, UiCharacterPortraitWidget> _PlayerWidgets = null;
        private TeamSystem _teamSystem = null;

        
        protected override void Start()
        {
            base.Start();
            UiSystem.OpenPanel<UiTeamElement>();
        }

        protected override void Awake()
        {
            base.Awake();
          
            _teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            _PlayerWidgets = new GenericUiList<BaseCharacter, UiCharacterPortraitWidget>(_portraitPrefab.gameObject, _playerRoot);
            Subscribe();
            SetList(_teamSystem.GetTeamMembers());
        }

        protected override void OnDestroy()
        {
            UnSubscribe();
            base.OnDestroy();
        }


        private void SetList(IEnumerable<BaseCharacter> data)
        {
            _PlayerWidgets.Generate(data,
                (entry, item) =>
                {
                    item.Setup(entry, character => { _teamSystem.RequestLeaderChange(character); });
                });
        }



        private void Subscribe()
        {
            GameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);
        }
        
        
        private void UnSubscribe()
        {
            GameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
        }


        private void OnLeaderChanged(BaseCharacter FollowedCharacter)
        {
            _targetWidget.Setup(FollowedCharacter);
        }
    }

}

