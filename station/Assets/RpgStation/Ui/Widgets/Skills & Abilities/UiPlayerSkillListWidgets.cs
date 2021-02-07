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
        private GenericUiList<RankProgression, UiSkillDisplayWidget> _skillList;
        private GameObject _skillDisplayPrefab;
        private LayoutGroup _skillLayoutGroup;
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
               // item.Setup(this, _itemDb, _containerReference);
                //item.SetData(entry);
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

