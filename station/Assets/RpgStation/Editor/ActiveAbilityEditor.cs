using System;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class ActiveAbilityEditor
    {
        private static ActiveAbilitiesDb _db;
        private static SkillDb _skillDb;
        private static VitalsDb _vitalsDb;
        
        private static Vector2 _propertyScrollPos;
        private static int _selectedIndexIndex;
        private static string style2 = "RegionBg";
        private static int _currentRank;
        private static int _toolBarIndex;
        private static Vector2 _scrollPos;
        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static ActiveAbility _cache;
        private static EffectHolder _cachedHolder;
        private static bool _displayCasting;
        private static bool displayCastingSound;
        private static bool _displayInvoking;
        private static bool displayInvokingSound;

    
        private static bool CacheDbs()
        {
            bool missing = false;
            if (_db == null)
            {
                _db = (ActiveAbilitiesDb)EditorStatic.GetDb(typeof(ActiveAbilitiesDb));
                missing = true;
            }
            if (_skillDb == null)
            {
              _skillDb = (SkillDb)EditorStatic.GetDb(typeof(SkillDb));
              missing = true;
            }
            if (_vitalsDb == null)
            {
              _vitalsDb = (VitalsDb)EditorStatic.GetDb(typeof(VitalsDb));
              missing = true;
            }

            return missing;
        }
        
        public static void DrawContent()
        {
            if (CacheDbs())
            {
                return;;
            }

            EditorGUI.BeginChangeCheck();
            {
                DrawSkillList();
                ListView(_selectedIndexIndex);
            }
            if (EditorGUI.EndChangeCheck())
            {
                _db.ForceRefresh(); 
            }
        }
    
        #region [[ DRAW ]]

        private static void DrawSkillList()
        {
            _selectedIndexIndex = EditorStatic.DrawGenericSelectionList(_db, _selectedIndexIndex, _propertyScrollPos,out _propertyScrollPos,"user",false);
        }
        #endregion
        
            private static void ListView(int selected)
    {
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos =
          EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        {
          AbilityPanel(selected);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void AbilityPanel(int selected)
    {
      var skillCount = _db.Count();
      if (selected == -1) return;
      if (skillCount == 0) return;
      if (skillCount < selected + 1) selected = 0;

      var staticData = _db.GetEntry(selected);
      GUILayout.Label("EDIT Active Abilities:", GUILayout.Width(130));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (staticData.Icon)
          _selectedTexture = staticData.Icon.texture;
        else
          _selectedTexture = null;
        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),
          GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, null, controllerId);
          _pickingTexture = true;
        }

        string commandName = Event.current.commandName;
        if (_pickingTexture && commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
          _db.GetEntry(selected).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }

        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name", GUILayout.Width(70));
            EditorStatic.DrawLocalization(staticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description", GUILayout.Width(70));
            EditorStatic.DrawLocalization(staticData.Description);
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);

        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete Ability?", "Do you want to delete: " + staticData.Name, "Delete", "Cancel"))
          {
            _db.Remove(staticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
     
      EditorStatic.DrawThinLine();
      staticData.ParentSkillId = EditorGUILayout.Popup("Skill group: ", staticData.ParentSkillId, _skillDb.ListEntryNames());
      DrawTargeting(staticData);
      EditorStatic.DrawThinLine();
      
      #region [[ RANKS ]]

      DrawRankBar(staticData);
      EditorGUILayout.Space();
      DrawRankView(staticData);
     
      #endregion

    }
    
    private static void DrawRankBar(ActiveAbility data)
    {
      _cache = data;
      EditorGUILayout.BeginHorizontal(EditorStatic.SectionHeader, GUILayout.ExpandWidth(true), GUILayout.Height(34));
      data.UseRank = EditorGUILayout.Toggle("Has Rank: ", data.UseRank,GUILayout.Height(32));
      
      if (data.UseRank)
      {
        if (EditorStatic.SizeableButton(32, 32, "", "resultset_previous"))
        {
          _currentRank--;
          if (_currentRank < 0) _currentRank = 0;
        }

        _currentRank++;
        _currentRank = EditorGUILayout.IntField(_currentRank,EditorStatic.CenterIntField, GUILayout.Height(32),GUILayout.Width(32));
        _currentRank--;
        if (_currentRank < 0) _currentRank = 0;
        if (EditorStatic.SizeableButton(32, 32, "", "resultset_next"))
        {
          _currentRank++;
          if (_currentRank > data.Ranks.Count +1) _currentRank = data.Ranks.Count;
        }
        
        EditorGUILayout.LabelField(string.Format("Rank {0}/{1} ",_currentRank+1, data.Ranks.Count), EditorStatic.HeaderText);
        
        if (EditorStatic.SizeableButton(32, 32, "", "cross"))
        {
          data.Ranks.RemoveAt(_currentRank);
        }

        if (EditorStatic.SizeableButton(100, 32, "Add rank", "plus"))
        {
          
          if (data.Ranks.Count > 0)
          {
            var lastEntry = data.Ranks.Last();
            var copy = JsonUtility.FromJson<AbilityRank>(JsonUtility.ToJson(lastEntry));
            data.Ranks.Add(copy);
          }
          else
          {
            data.Ranks.Add(new AbilityRank());
          }
        }   
      }
      else
      {
        //ENSURE ONLY ONE RANK
        if (data.Ranks.Count == 0)
        {
          data.Ranks.Add(new AbilityRank());
        }
        while(data.Ranks.Count >1)
        {
          data.Ranks.RemoveAt(data.Ranks.Count-1);
        }
        //DRAW THE RANK
      }
      
      EditorGUILayout.EndHorizontal();
     
    }
    
    private static void DrawRankView(ActiveAbility data)
    {
      if (_currentRank > data.Ranks.Count-1)
      {
        _currentRank = 0;
      }
      #region [[ Require ]]
      EditorGUILayout.BeginHorizontal( GUILayout.ExpandWidth(true));
      EditorGUILayout.BeginVertical(style2, GUILayout.ExpandWidth(true),  GUILayout.Height(100));
      EditorStatic.DrawBonusWidget(data.Ranks[_currentRank].VitalsUsed, "Vital Consumed", _vitalsDb);
     
      EditorStatic.DrawThinLine(3);
      var currentRank = data.Ranks[_currentRank];
      
      currentRank.ActionFx = EditorStatic.DrawActionEffect("Casting", ref _displayCasting, ref displayCastingSound, ref  currentRank.ActionFx);

      EditorStatic.DrawThinLine(3);
      EditorStatic.DrawActionEffect("Invoke", ref _displayInvoking, ref displayInvokingSound, ref currentRank.InvokingActionFx);
  
      EditorStatic.DrawThinLine(3);
      
      if (currentRank.ProjectileDrivers.Any() || currentRank.DirectDrivers.Any())
      {
        currentRank.CastDistance = EditorGUILayout.FloatField("Target distance: ",currentRank.CastDistance);
      }
      currentRank.CoolDown = EditorGUILayout.FloatField("Cooldown: ", currentRank.CoolDown);
        
      EditorGUILayout.EndVertical();
      
      EditorGUILayout.EndHorizontal();
      //DRIVERS BAR
      EditorGUILayout.BeginHorizontal(EditorStatic.SectionHeader, GUILayout.ExpandWidth(true), GUILayout.Height(40));
      EditorGUILayout.LabelField("DRIVERS: ");
      if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
      {
        GenericMenu menu = new GenericMenu();
        //get list types from BaseAbilityDriver
        var list = ReflectionUtils.GetClassList<BaseAbilityDriver>();
        for (var index = 0; index < list.Length; index++)
        {
          var entry = list[index];
          AddMenuEntry(menu, entry.Name, index);
        }

        menu.ShowAsContext();
      }
      EditorGUILayout.EndHorizontal();
      DrawDriver(data);
      #endregion
    }
    
    private static void AddMenuEntry(GenericMenu menu, string menuPath, int idType)
    {
      menu.AddItem(new GUIContent(menuPath),false, OnDriverSelected, idType);
    }

    private static void OnDriverSelected(object idType)
    {
      int id = (int)idType;
      switch (id)
      {
          case 0:
            _cache.Ranks[_currentRank].DirectDrivers.Add(new Direct());
            break;
          case 1:
            _cache.Ranks[_currentRank].ProjectileDrivers.Add(new Projectile());
          break;
      }
    }

    private static void DrawDriver(ActiveAbility data)
    {
      foreach (var driver in data.Ranks[_currentRank].ProjectileDrivers)
      {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        {
          DrawDriverHeader(driver, data, typeof(Projectile));
          driver.DrawEditor();
          AbilityEffectEditor.DrawEffectStack(driver.Effects);
        }
        EditorGUILayout.EndVertical();
      }
      
      foreach (var driver in data.Ranks[_currentRank].DirectDrivers)
      {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        {
          DrawDriverHeader(driver, data, typeof(Direct));
          driver.DrawEditor();
          AbilityEffectEditor.DrawEffectStack(driver.Effects);
        }
        EditorGUILayout.EndVertical();
      }
    }
    
    private static void DrawDriverHeader(BaseAbilityDriver driver, ActiveAbility data, Type type)
    {
     
        EditorGUILayout.BeginHorizontal();
        EditorStatic.DrawSectionTitle(32, driver.GetType().Name);

        if (EditorStatic.SizeableButton(32,32,"","cross"))
        {
          if (type == typeof(Projectile))
          {
            data.Ranks[_currentRank].ProjectileDrivers.Remove((Projectile)driver);
          }
          
          if (type == typeof(Direct))
          {
            data.Ranks[_currentRank].DirectDrivers.Remove((Direct)driver);
          }

          GUIUtility.ExitGUI();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private static void DrawTargeting(ActiveAbility data)
    {
      data.Targeting.UsedAbilityTargeting = (AbilityTargeting)EditorGUILayout.EnumPopup("Required target: ",data.Targeting.UsedAbilityTargeting);
      if (data.Targeting.UsedAbilityTargeting != AbilityTargeting.None)
      {
        data.Targeting.TargetRequiredState = (RequireTargetState)EditorGUILayout.EnumPopup("Required target state: ",data.Targeting.TargetRequiredState);
      }
    }


    }
}

