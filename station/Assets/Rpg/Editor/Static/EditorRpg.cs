using System.Collections.Generic;
using Station;
using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
  public static partial class EditorStatic
  {
    #region FIELDS
    //[[GAME CONFIG TAB]]
    public const string CONFIG_TAB_GAMEPLAY = "<size=11><b> Gameplay</b></size>";
    public const string CONFIG_TAB_CHARACTER_CREATION = "<size=11><b> Character Creation</b></size>";
    public const string CONFIG_TAB_DIFFICULTY = "<size=11><b> Difficulty</b></size>";
    public const string CONFIG_TAB_OPTIONS = "<size=11><b> Options</b></size>";
    public const string CONFIG_TAB_PLATFORMS = "<size=11><b> Platforms</b></size>";
    public const string CONFIG_TAB_INPUT_EVENTS = "<size=11><b> Input Events</b></size>";
    public const string CONFIG_TAB_FLOATING_POPUPS = "<size=11><b> Floating Popups</b></size>";
    public const string CONFIG_TAB_UI_CHANNELS = "<size=11><b> Ui Notification Channels</b></size>";
    public const string CONFIG_TAB_SOUNDS = "<size=11><b> Sounds</b></size>";
    public const string CONFIG_TAB_FOOTSTEPS = "<size=11><b> Footsteps</b></size>";
    public const string CONFIG_TAB_UI_PREFABS = "<size=11><b> Fields and Prefabs</b></size>";
    
    //[[STATS TAB]]
    public const string STATS_TAB_ATTRIBUTES = "<size=11><b> Attributes</b></size>";
    public const string STATS_TAB_VITALS = "<size=11><b> Vitals</b></size>";
    public const string STATS_TAB_STATISTICS = "<size=11><b> Statistics</b></size>";
    public const string STATS_TAB_ELEMENTS = "<size=11><b> Elements</b></size>";
    public const string STATS_TAB_STATUS_EFFECTS = "<size=11><b> Status effects</b></size>";
    
    //[[CHARACTERS TAB]]
    public const string CHARACTERS_TAB_RACES =  "<size=11><b> Races</b></size>";
    public const string CHARACTERS_TAB_PLAYER_CLASSES ="<size=11><b> Player classes</b></size>";
    public const string CHARACTERS_TAB_NPC = "<size=11><b> Npcs</b></size>";
    public const string CHARACTERS_TAB_PETS = "<size=11><b> Pets</b></size>";
    
    //[[SKILLS TAB]]
    public const string SKILLS_TAB_SKILLS =  "<size=11><b> Skills</b></size>";
    public const string SKILLS_TAB_ACTIVE_ABILITIES =  "<size=11><b> Active Abilities</b></size>";
    public const string SKILLS_TAB_PASSIVE_ABILITIES =  "<size=11><b> Passive Abilities</b></size>";
    
    //[[ WORLD ]]
    public const string WORLD_TAB_CONFIGURATION = "<size=11><b> Configuration</b></size>";
    public const string WORLD_TAB_SCENES = "<size=11><b> Scenes</b></size>";
    public const string WORLD_TAB_SPAWN_POINTS = "<size=11><b> Spawn points</b></size>";
    
    //[[ FACTION ]]
    public const string FACTION_TAB_SETTINGS = "<size=11><b> Settings</b></size>";
    public const string FACTION_TAB_CONFIGURATION = "<size=11><b> Configuration</b></size>";
    public const string FACTION_TAB_FACTIONS = "<size=11><b> Factions</b></size>";
    
    //[[ INTERACTION ]]
    public const string INTERACTION_TAB_CONFIGURATION = "<size=11><b> Configuration</b></size>";
    
    // [[ SOUND CATEGORIES ]]
    public const string ABILITIES_CATEGORY = "Abilities";
    
    //[[ITEMS TAB]]
    public const string ITEMS_TAB_SETTINGS = "<size=11><b> Settings</b></size>";
    public const string ITEMS_TAB_ITEMS = "<size=11><b> Items</b></size>";
    public const string ITEMS_TAB_LOOT_TABLES = "<size=11><b> Loot tables</b></size>";
    public const string ITEMS_TAB_CURRENCIES = "<size=11><b> Currencies</b></size>";
    public const string ITEMS_TAB_SHOPS = "<size=11><b> Shops</b></size>";
    public const string ITEMS_TAB_CRAFT = "<size=11><b> Craft</b></size>";
    public const string ITEMS_TAB_RESOURCES_NODES = "<size=11><b> Resources Nodes</b></size>";
    public const string ITEMS_TAB_CHEST_NODES = "<size=11><b> Chest Nodes</b></size>";
    //-Settings
    public const string ITEMS_TAB_SETTINGS_TAGS = "<size=11><b> Tags</b></size>";
    public const string ITEMS_TAB_SETTINGS_RARITIES = "<size=11><b> Item rarities</b></size>";
    public const string ITEMS_TAB_SETTINGS_ITEMS_CATEGORIES = "<size=11><b> Item categories</b></size>";
    public const string ITEMS_TAB_SETTINGS_EQUIPMENT_SLOTS = "<size=11><b> Equipment slots</b></size>";
    public const string ITEMS_TAB_SETTINGS_EQUIPMENT_TYPES = "<size=11><b> Equipment Types</b></size>";
    public const string ITEMS_TAB_SETTINGS_CONTAINERS = "<size=11><b> Containers</b></size>";
    public const string ITEMS_TAB_SETTINGS_CRAFTING = "<size=11><b> Crafting</b></size>";
    
    #endregion

    public static ActionFxData DrawActionEffect(string effectType, ref bool isFoldoutOpen, ref bool isSoundFoldoutOpen, ref ActionFxData actionFx)
    {
      EditorGUILayout.BeginHorizontal();
      isFoldoutOpen = LevelFoldout(effectType, isFoldoutOpen, 32, Color.white);
      bool useCasting = actionFx.HasData;
      string buttonName = useCasting ? "ON" : "OFF";
      string iconName = useCasting ? "bullet_green" : "bullet_red";
      if (SizeableButton(80, 28, buttonName, iconName))
      {
        actionFx.HasData = !actionFx.HasData;
      }
      EditorGUILayout.EndHorizontal();
      if (isFoldoutOpen)
      {
   
        if (actionFx.HasData)
        { 
          actionFx.Length = EditorGUILayout.FloatField($"{effectType} time: ", actionFx.Length);
          actionFx.AnimationId = EditorGUILayout.IntField($"{effectType} animation ID: ", actionFx.AnimationId);
          
          isSoundFoldoutOpen = SoundFoldout($"{effectType} sound: ", ref actionFx.StartSound, isSoundFoldoutOpen, 28, Color.cyan);
          if (isSoundFoldoutOpen)
          {
            DrawSoundWidget(ref actionFx.StartSound, ABILITIES_CATEGORY);
          }
          
          actionFx.Option = (ExitMode)EditorGUILayout.EnumPopup("Mode: ", actionFx.Option);
          actionFx.Effect = DrawVfxData(actionFx.Effect);
        }
      }

      return actionFx;
    }

    #region ACTIONS

    public static VfxData DrawVfxData(VfxData data)
    {
      data.EffectPrefab = (BaseVfxPlayer)EditorGUILayout.ObjectField("Effect prefab:",data.EffectPrefab, typeof(BaseVfxPlayer), false);
      return data;
    }

    #endregion
       #region [[ WIDGETS ]]
    
    public static void DrawBonusWidget<T>(List<IdFloatValue> current, string bonusName, DictGenericDatabase<T> dict) where T : class
    {
      EditorGUILayout.BeginHorizontal();
      DrawSectionTitle(bonusName, 350, 3);
      if (SizeableButton(100, 22, "Add", "plus")) { current.Add(new IdFloatValue(dict.GetKey(0), 5)); }
      EditorGUILayout.EndHorizontal();
      for (var index = 0; index < current.Count; index++)
      {
        var bonus = current[index];
        GUILayout.BeginHorizontal("box");
        {
          int bonusIndex = dict.GetIndex(bonus.Id);
          bonusIndex = EditorGUILayout.Popup(bonusIndex, dict.ListEntryNames(), GUILayout.Width(100));
          bonus.Id = dict.GetKey(bonusIndex);
          GUILayout.Space(5);
          bonus.Value = EditorGUILayout.FloatField(bonus.Value, GUILayout.Width(125));
          GUILayout.Space(5);

          if (SizeableButton(65, 16, "DELETE", ""))
          {
            current.Remove(bonus);
            return;
          }
        }
        GUILayout.EndHorizontal();
      }
    }

    public static void DrawBonusWidget<T>(List<IdIntegerValue> current, string bonusName, DictGenericDatabase<T> dict) where T : class
    {
      EditorGUILayout.BeginHorizontal();
      DrawSectionTitle(bonusName, 350, 3);
      if (SizeableButton(100, 22, "Add", "plus")) { current.Add(new IdIntegerValue(dict.GetKey(0), 5)); }
      EditorGUILayout.EndHorizontal();
      for (var index = 0; index < current.Count; index++)
      {
        var bonus = current[index];
        GUILayout.BeginHorizontal("box");
        {
          int bonusIndex = dict.GetIndex(bonus.Id);
          if (bonusIndex < 0)
          {
            bonusIndex = 0;
          }

          bonusIndex = EditorGUILayout.Popup(bonusIndex, dict.ListEntryNames(), GUILayout.Width(100));
          Debug.Log(bonusIndex+"  --  "+dict.GetKey(bonusIndex));
          bonus.Id = dict.GetKey(bonusIndex);
          GUILayout.Space(5);
          bonus.Value = EditorGUILayout.IntField(bonus.Value, GUILayout.Width(125));
          GUILayout.Space(5);

          if (SizeableButton(65, 16, "DELETE", ""))
          {
            current.Remove(bonus);
            return;
          }
        }
        GUILayout.EndHorizontal();
      }
    }

    #endregion
  }
}

