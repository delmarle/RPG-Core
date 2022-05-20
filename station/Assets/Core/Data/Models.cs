using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Station

{
    [Serializable] public class StringDictionary : SerializableDictionary<string, string> {}
    

    [Serializable]
    public class FieldVariable
    {
      public string Id;
      public FieldType TypeField;
      public int ValueInt;
      public float ValueFloat;
      public string ValueString;
      public bool ValueBool;
    }
    
    [Serializable]
    public class PrefabReference
    {
      public string Id;
      public GameObject Prefab;
      public int InitialAmount;
    }

    public enum FieldType
    {
      Int,Float,String,Bool
    }

    [Serializable]
    public class DestinationModel
    {
        public string SceneId;
        public int SpawnId;
    }
    
  #region SCENES
  [Serializable]
  public class Scene
  {
    public SceneReference Reference;
    public Sprite Icon;
    public string VisualName = "zone_xxx";
    public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    public string[] SpawnNames()
    {
      List<string> names = new List<string>();
      foreach (var spawn in SpawnPoints)
      {
        names.Add(spawn.VisualName.GetValue());
      }
      return names.ToArray();
    }
  }
  
  [Serializable]
  public class SpawnPoint
  {
    public LocalizedText VisualName;
    public LocalizedText Description = new LocalizedText("description");
    public Sprite Icon;
    public Vector3[] Positions;
    public Vector3 Direction;

    public SpawnPoint(string name)
    {
      VisualName = new LocalizedText(name);
    }
  }
  #endregion
  #region UI

  [Serializable]
  public class FloatingPopupModel
  {
    public string Name;
    public FloatingPopup Prefab;
    public int PoolSize = 1;
    public ShowPopupRule Rule = new ShowPopupRule();
    public bool Follow;
  }
  
  [Serializable]
  public class ShowPopupRule
  {
    public ShowRuleEntry LeaderRules = new ShowRuleEntry();
    public ShowRuleEntry TeamMemberRules= new ShowRuleEntry();
    public ShowRuleEntry NpcRules = new ShowRuleEntry();
    public ShowRuleEntry PetRules = new ShowRuleEntry();

    public bool ShowOnlyVisible;
    public float ShowRangeLimit = -1;
  }

  public class ShowRuleEntry
  {
    public bool ByAny = true;
    public bool ByLeader;
    public bool ByTeamMember;
    public bool ByNpc;
    public bool ByPet;
  }

  public enum IdentityType
  {
    ControlledCharacter,
    PlayerOwnedCharacter,
    OtherCharacter,
    Pet
  }
  #endregion

  #region LOCALIZATION
  [Serializable]
  public class LocalizedText
  {
    public LocalizedText(string defaultName)
    {
      Key = defaultName;
    }

    public string Key;

    public string GetValue()
    {
      return Key;
    }
  }

  #endregion
  #region GENERIC INTERFACE

  public interface IStationIcon
  {
    Sprite GetIcon();
  }

  #endregion
  
  #region SOUNDS
  [Serializable]
  public class SoundCategory
  {
    public string CategoryName;
  }

  [Serializable]
  public class SoundAddress
  {
    public string SoundContainerName;
  }

  [Serializable]
  public class SoundReferenceDrawer
  {
    public bool Enabled = false;
    public string GroupId;
    public string SoundId;
  }

  #endregion

  [Serializable]
  public class SceneSaveSettingsModel
  {
    public bool SaveOnExit;
    public bool SaveOnEnter;
    public int AutoSaveTimer;
  }

  [Serializable]
    public class GameSettingsModel
    {
      public AssetReference LoadingScreen;
      public List<UiPopup> _cachedUniquePopup = new List<UiPopup>();
      public List<FieldVariable> _cachedFields = new List<FieldVariable>();
      public List<PrefabReference> _poolPrefabs = new List<PrefabReference>();

      #region CACHING
      private Dictionary<string, PrefabReference> _prefabsMap = new Dictionary<string, PrefabReference>();
      private Dictionary<string, FieldVariable> _fieldsMap = new Dictionary<string, FieldVariable>();
      private Dictionary<string, UiPopup> _popupsMap = new Dictionary<string, UiPopup>();

      public void CacheData()
      {
        foreach (var entry in _cachedUniquePopup)
        {
          if (_popupsMap.ContainsKey(entry.PopupUniqueId) == false)
          {
            _popupsMap.Add(entry.PopupUniqueId, entry);
          }
          else
          {
            Debug.LogError($"DUPLICATE UNIQUE POPUP KEY: {entry.PopupUniqueId}. this need to be changed or removed");
          }
        }
        
        foreach (var entry in _cachedFields)
        {
          if (_fieldsMap.ContainsKey(entry.Id) == false)
          {
            _fieldsMap.Add(entry.Id, entry);
          }
          else
          {
            Debug.LogError($"DUPLICATE UNIQUE FIELD KEY: {entry.Id}. this need to be changed or removed");
          }
        }
        
        foreach (var entry in _poolPrefabs)
        {
          if (_prefabsMap.ContainsKey(entry.Id) == false)
          {
            _prefabsMap.Add(entry.Id, entry);
          }
          else
          {
            Debug.LogError($"DUPLICATE UNIQUE prefab KEY: {entry.Id}. this need to be changed or removed");
          }
        }
      }

      public PrefabReference GetPrefab(string key)
      {
        if (_prefabsMap.ContainsKey(key))
        {
          return _prefabsMap[key];
        }

        Debug.LogWarning($"[GetPrefab] no entry found for {key}");
        return null;
      }
      
      public FieldVariable GetField(string key)
      {
        if (_fieldsMap.ContainsKey(key))
        {
          return _fieldsMap[key];
        }

        Debug.LogWarning($"[GetField] no entry found for {key}");
        return null;
      }
      public UiPopup GetPopupPrefab(string key)
      {
        if (_popupsMap.ContainsKey(key))
        {
          return _popupsMap[key];
        }

        Debug.LogWarning($"[GetPopupPrefab] no entry found for {key}");
        return null;
      }
      #endregion
    }
}