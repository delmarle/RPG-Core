
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiSkillTrainPopup : UiPopup
    {
        #region FIELDS

        public const string POPUP_ID = "skill_train_popup";
        [SerializeField] private UiSkillDisplayWidget _prefabSkill;
        [SerializeField] private LayoutGroup _layoutGroup;
        private BaseCharacter _demander;
        private TrainSkillInteraction _trainSkillInteraction;

        private GenericUiList<TrainSkillData, UiSkillDisplayWidget> _skillList;
        #endregion
        protected virtual void Awake()
        {
            base.Awake();
            _skillList = new GenericUiList<TrainSkillData, UiSkillDisplayWidget>(_prefabSkill.gameObject, _layoutGroup, 20);
        }
        
        public void SetData(BaseCharacter owner, BaseCharacter demander, TrainSkillInteraction trainInteraction)
        {
            _demander = demander;
            _trainSkillInteraction = trainInteraction;
            var skillTrainedData = trainInteraction.SkillTrained;

            _skillList.Generate(skillTrainedData, (entry, item) =>
            {
                item.SetupAsTrain(_demander, entry);
            });
        }

        private void TrainRankUp(RankProgression skillToTrain)
        {
            
            if (_demander.Skills.Skills.ContainsKey(skillToTrain.Id))
            {
                _demander.Skills.AddSkillRank(skillToTrain.Id);
            }
            else
            {
                _demander.Skills.AddSkill(skillToTrain);
            }
            
        }
    }
}