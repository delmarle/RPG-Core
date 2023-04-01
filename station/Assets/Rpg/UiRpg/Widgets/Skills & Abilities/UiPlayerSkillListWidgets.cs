using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiPlayerSkillListWidgets : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private GenericUiList<RankProgression, UiSkillDisplayWidget> _skillList;
        [SerializeField] private GameObject _skillDisplayPrefab;
        [SerializeField] private LayoutGroup _skillLayoutGroup;
        #endregion
        private void Awake()
        {
            _skillList = new GenericUiList<RankProgression, UiSkillDisplayWidget>(_skillDisplayPrefab.gameObject, _skillLayoutGroup, 40);
        }

        public void UpdateSkillList(BaseCharacter target)
        {
            var listSkills = target.Skills.GenerateSave();
            
            _skillList.Generate(listSkills, (entry, item) =>
            {
                item.Setup(target, entry);
            });
        }
        //switch characters
        //list skills 
        //passive abilities
        //active abilities
        //draw and drop to player bar
        //proxy player bar
    }
    
    
}

