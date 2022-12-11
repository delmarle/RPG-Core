﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    
public class PlayersSave : SaveModule<Dictionary<string, PlayersData>>
{
    
    public void AddPlayer(string key, PlayersData data)
    {
        Value.Add(key, data);
    }

    public override void FetchData()
    {
        var sceneSystem = GameInstance.GetSystem<RpgSceneSystem>();
        var teamSystem = GameInstance.GetSystem<TeamSystem>();
        
        var teamMembers = teamSystem.GetTeamMembers();
  

        if (Value != null && teamMembers != null)
        {
            if (Value.Count != teamMembers.Count)
            {
                //Debug.Log("the player count in save is different than players in team");
            }
            //we already injected destination
            if (sceneSystem.IsTraveling == false)
            {
                //save current position
                foreach (var playerCharacter in teamMembers)
                {
                   
                    var key = (string)playerCharacter.GetMeta(RpgConst.CHARACTER_ID);
                    if (Value.ContainsKey(key) == false)
                    {
                        Value.Add(key, new PlayersData());
                    }

                    var transformPlayer = playerCharacter.transform;
                    Value[key].LastPosition = transformPlayer.position;
                    Value[key].LastRotation = transformPlayer.rotation.eulerAngles;
                    var playerActionHandler = playerCharacter.Action as RpgActionHandler;
                    Value[key].LearnedActiveAbilitiesList = playerActionHandler?.GetAbilitiesState();
                    Value[key].LearnedSkillList = playerCharacter.Skills.GenerateSave();
                }
            }
        }
    }
}

[Serializable]
public class PlayersData
{
    public string Name;
    public string LastZoneId;
    public Vector3 LastPosition;
    public Vector3 LastRotation;
    
    //meta
    public string RaceId;
    public string ClassId;
    public string GenderId;
    public string FactionId;

    public List<IdIntegerValue> VitalStatus; 
    
    public List<RankProgression> LearnedSkillList;
    public List<RankedTimeIdSave> LearnedActiveAbilitiesList;
    public List<RankedIdSave> LearnedPassiveAbilitiesList;

    public List<BarStateSave> BarStates;
}

[Serializable]
public class BarStateSave
{
    public BarStateSave(string id)
    {
        Id = id;
    }

    public string Id;
    public List<BarSlotState> Slots = new List<BarSlotState>();
}

[Serializable]
public class BarSlotState
{
    public BarSlotState(LinkType type, int id)
    {
        Link = type;
        Index = id;
    }

    public LinkType Link;
    public int Index;
}

public enum LinkType
{
    Ability,
    Inventory,
    Empty
}

}


