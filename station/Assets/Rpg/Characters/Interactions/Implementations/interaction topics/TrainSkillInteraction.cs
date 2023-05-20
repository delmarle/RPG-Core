using System;
using System.Collections.Generic;
using Station;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class TrainSkillInteraction : InteractionLine
    {
        public List<TrainSkillData> SkillTrained = new List<TrainSkillData>();
        
        public override void Trigger(BaseCharacter owner, BaseCharacter demander)
        {
            var trainSkillPopup = UiSystem.GetUniquePopup<UiSkillTrainPopup>(UiSkillTrainPopup.POPUP_ID);
            if (trainSkillPopup)
            {
                trainSkillPopup.SetData(owner, demander, this);
                trainSkillPopup.Show();
            }
            else
            {
                Debug.LogError($"could not find the train skill popup, make sure it is cached in the pool");
            }
           
        }
    }
}

[Serializable]
public class TrainSkillData
{
    public string SkillId;

    public TrainSkillRankData[] TrainableRank;
    //can learn each rank : rank, requirement, cost

    public SkillData GetSkillData()
    {
        var dbsystem = GameInstance.GetDb<DbSystem>();
        var skillDb = dbsystem.GetDb<SkillDb>();
        if (skillDb.HasKey(SkillId))
        {
            return skillDb.GetEntry(SkillId);
        }

        return null;
    }
}

[Serializable]
public class TrainSkillRankData
{
    
}