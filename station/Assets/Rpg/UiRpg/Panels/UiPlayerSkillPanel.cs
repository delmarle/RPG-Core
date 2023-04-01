
using UnityEngine;

namespace Station
{
    public class UiPlayerSkillPanel : UiPanel
    {
        #region fields

        [SerializeField] private UiPlayerSkillListWidgets _skillsListWidget;

        private BaseCharacter _character;
        #endregion

        public override void Show()
        {
            base.Show();
            Setup();
        }

        private void Setup()
        {
            var teamSystem = GameInstance.GetSystem<TeamSystem>();
            _character = teamSystem.GetCurrentLeader();
            UnFollowCharacter();
            FollowCharacter();
            RefreshSkillView();
        }

        #region skills related

        private void RefreshSkillView()
        {
            _skillsListWidget.UpdateSkillList(_character);
        }
        private void FollowCharacter()
        {
            if (_character)
            {
                _character.OnSkillGained += OnSkillGained;
                _character.OnSkillRemoved += OnSkillRemoved;
                _character.OnSkillUpdated += OnSkillUpdated;
            }
        }

        private void UnFollowCharacter()
        {
            if (_character)
            {
                _character.OnSkillGained -= OnSkillGained;
                _character.OnSkillRemoved -= OnSkillRemoved;
                _character.OnSkillUpdated -= OnSkillUpdated;
            }
        }

       


        protected void OnLeaderUpdate(BaseCharacter character)
        {
            
        }

        private void OnSkillGained(BaseCharacter character, RankProgression progress, int data)
        {
            
        }
        
        private void OnSkillRemoved(BaseCharacter character, RankProgression progress, int data)
        {
            
        }
        
        private void OnSkillUpdated(BaseCharacter character, RankProgression progress, int data)
        {
            
        }
        
        #endregion
    }
}

