using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiHudPanel : UiPanel
    {
        [SerializeField] private ActionCastBarWidget _castBar;
        [SerializeField] private LayoutGroup _playerRoot = null;
        [SerializeField] private UiCharacterPortraitWidget _portraitPrefab = null;
        [SerializeField] private UiCharacterTargetWidget _targetWidget = null;
        [SerializeField] private UiEventData _initialEventData;
        
        private GenericUiList<BaseCharacter, UiCharacterPortraitWidget> _PlayerWidgets = null;
        private TeamSystem _teamSystem = null;

        
        protected override void Start()
        {
            base.Start();
            _PlayerWidgets = new GenericUiList<BaseCharacter, UiCharacterPortraitWidget>(_portraitPrefab.gameObject, _playerRoot);
            GameGlobalEvents.OnUiEvent.Invoke(_initialEventData);
            SetList(_teamSystem.GetTeamMembers());
            UiSystem.OpenPanel<UiHudPanel>();
        }

        protected override void Awake()
        {
            base.Awake();
          
            _teamSystem = GameInstance.GetSystem<TeamSystem>();
            
            Subscribe();
         
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
            RpgGameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);
        }
        
        
        private void UnSubscribe()
        {
            RpgGameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
        }


        private void OnLeaderChanged(BaseCharacter FollowedCharacter)
        {
            _targetWidget?.Setup(FollowedCharacter);
            _castBar?.FollowCharacter(FollowedCharacter);
        }
    }

}

