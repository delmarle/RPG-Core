
using UnityEngine;

namespace Station
{
    public class UiCharacterTab : UiElementBase, ICharacterSwitchable
    {
        #region [[ FIELDS ]]

        [SerializeField] private UiCharacterSelectionListWidget _charSelection;
        [SerializeField] private UiCharacterPortraitWidget _characterWidget = null;
        [SerializeField] private UiCharacterStatsListWidget _statsListWidget = null;
        [SerializeField] private UiCharacterVitalsListWidget _vitalsListWidget = null;
        [SerializeField] private UiCharacterAttributesListWidget _attributesListWidget = null;
        protected BaseCharacter _character;

        #endregion

        #region [[ MONOBEHAVIOURS ]]

        protected override void Awake()
        {
            _attributesListWidget.Setup();
            _vitalsListWidget.Setup();
            _statsListWidget.Setup();
            _charSelection.ApplyTarget(new ICharacterSwitchable[] {this});
            GameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderUpdate);
            base.Awake();
        }

        protected void OnDestroy()
        {
            UnFollowCharacter();
            GameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderUpdate);
        }

        #endregion

        public override void Show()
        {
            var teamSystem = GameInstance.GetSystem<TeamSystem>();
            OnLeaderUpdate(teamSystem.GetCurrentLeader());
            base.Show();
        }

        private void UnFollowCharacter()
        {
            if (_character)
            {
                _character.OnVitalsUpdated -= OnVitalsUpdated;
                _character.OnStatisticUpdated -= OnStatisticsUpdated;
                _character.OnAttributesUpdated -= OnAttributesUpdated;
            }
        }

        protected void OnLeaderUpdate(BaseCharacter character)
        {
            UnFollowCharacter();
            _character = character;
            character.OnVitalsUpdated += OnVitalsUpdated;
            character.OnStatisticUpdated += OnStatisticsUpdated;
            character.OnAttributesUpdated += OnAttributesUpdated;
            UpdateUi();
        }

        private void OnAttributesUpdated(CoreCharacter character)
        {
            UpdateAttributes();
        }

        private void OnVitalsUpdated(CoreCharacter character)
        {
            UpdateVitals();
        }

        private void OnStatisticsUpdated(CoreCharacter character)
        {
            UpdateStatistics();
        }

        private void UpdateAttributes()
        {
            _attributesListWidget?.UpdateAttributes(_character);
        }

        private void UpdateVitals()
        {
            _vitalsListWidget?.UpdateVitals(_character);
        }

        private void UpdateMeta()
        {
            _characterWidget?.Setup(_character, null);
        }

        public void UpdateUi()
        {
            UpdateMeta();
            UpdateAttributes();
            UpdateStatistics();
            UpdateVitals();
        }

        private void UpdateStatistics()
        {
            _statsListWidget?.UpdateStatistics(_character);
        }

        public void SwitchCharacter(BaseCharacter character)
        {
            OnLeaderUpdate(character);
        }
    }
}