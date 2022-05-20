using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterCreationListPanel : UiPanel
    {
        #region FIELDS
        [SerializeField] private UiButton _widgetPrefab;
        [SerializeField] private LayoutGroup _listRoot;
        [SerializeField] private GenericUiList<PlayersData, UiButton> _list;
        [SerializeField] private UnityEngine.UI.Button _factionBtn;
        [SerializeField] private UnityEngine.UI.Button _enterGameBtn;
        
        private SimpleCharacterCreation _creationAsset;
        private StationAction _onClickFaction;
        private StationAction _onClickEnterGame;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _list = new GenericUiList<PlayersData, UiButton>(_widgetPrefab.gameObject, _listRoot);
            _factionBtn.onClick.AddListener(OnClickFactionBtn);
            _enterGameBtn.onClick.AddListener(OnClickEnterGameBtn);
        }

        private void OnClickEnterGameBtn()
        {
            _onClickEnterGame?.Invoke();
        }

        private void OnClickFactionBtn()
        {
            _onClickFaction?.Invoke();
        }

        public void Setup(SimpleCharacterCreation creationAsset, StationAction<int> selectCharacterToEditCallback,StationAction onClickEditFaction, StationAction onClickEnterGame)
        {
            _creationAsset = creationAsset;
            _onClickFaction = onClickEditFaction;
            _onClickEnterGame = onClickEnterGame;
            var raceDb = GameInstance.GetDb<RaceDb>();
            var classDb = GameInstance.GetDb<PlayerClassDb>();
            List<PlayersData> charList = creationAsset.GetPlayers;
            int index = 0;
            _list.Generate(charList,(entry, uiItem) =>
            {
                if (string.IsNullOrEmpty(entry.Name))
                {
                    uiItem.SetState("empty");
                }
                else
                {
                    var raceMeta = raceDb.GetEntry(entry.RaceId);
                    var classMeta = classDb.GetEntry(entry.ClassId);
                    uiItem.SetName(entry.Name);
                    uiItem.SetSecondText($"{raceMeta.Name.GetValue()} {classMeta.Name.GetValue()}");
                    uiItem.SetState("has_character");
                    uiItem.SetIcon(classMeta.Icon);
                }
                uiItem.SetIndex(index);
                uiItem.SetCallBack(selectCharacterToEditCallback);
                uiItem.SetSecondCallBack(selectCharacterToEditCallback);
                index++;
            });
            

            _enterGameBtn.interactable = creationAsset.AllCharacterCreated();
        }
    }

}
