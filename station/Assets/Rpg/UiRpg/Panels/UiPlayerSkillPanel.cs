
using UnityEngine;

namespace Station
{
    public class UiPlayerSkillPanel 
    {
        #region fields

        [SerializeField] private UiPlayerSkillListWidgets _skillsListWidget;

        #endregion

 

        #region skills related
        /*
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
*/
       


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

