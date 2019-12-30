using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class FactionEditor
    {
        private static FactionDb _db;
        private static FactionSettingsDb _dbSettings;
        private static Vector2 _propertyScrollPos;
        private static int _selectedIndex = 1;
    
        private static Vector2 _scrollPos;
        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static int _selectedStance;
        private static int _selectedFactionToAdd;
        private static FactionRank[] _ranks;
        
        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                Draw();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }
    
        #region [[ DRAW ]]

        private static void Draw()
        {
            if (_db == null)
            {
                _db = (FactionDb)EditorStatic.GetDb(typeof(FactionDb));
            }

            
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                DrawBar();
                ListView(_selectedIndex);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawBar()
        {
            _selectedIndex = EditorStatic.DrawGenericSelectionList(_db, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_yellow",false);
        }
        #endregion

        private static void ListView(int selected)
        {
          if (_dbSettings == null)
          {
            _dbSettings = (FactionSettingsDb)EditorStatic.GetDb(typeof(FactionSettingsDb));
          }
          _ranks = _dbSettings.Get().Ranks.ToArray();
          var staticData = _db.GetEntry(selected);
          GUILayout.BeginHorizontal("box");
          {
            _scrollPos =
              EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            {
              DrawFaction(selected);
            }
            EditorGUILayout.EndScrollView();
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
          if (staticData != null)
          {
            DrawSelected(staticData);
          }


          GUILayout.EndVertical();
        }

    private static void DrawFaction(int selected)
    {

      var factionCount = _db.Count();
      if (selected == -1) return;
      if (factionCount == 0) return;
      if (factionCount < selected + 1) selected = 0;

      var staticData = _db.GetEntry(selected);

      GUILayout.Label(staticData.Name ,EditorStatic.SectionTitle, GUILayout.Width(130));
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
            staticData.Name = GUILayout.TextField(staticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description", GUILayout.Width(70));
            staticData.Description = GUILayout.TextArea(staticData.Description, GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete Faction?", "Do you want to delete: " + staticData.Name, "Delete",
            "Cancel"))
          {
            _db.Remove(staticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine();
     
      List<string> ranks =new List<string>();
      foreach (var rank in _ranks)
      {
        ranks.Add(rank.Name);
      }
      staticData.DefaultStance = EditorGUILayout.Popup(" Default stance: ", staticData.DefaultStance, ranks.ToArray());
      staticData.StanceToSelf = EditorGUILayout.Popup(" Stance to self: ", staticData.StanceToSelf, ranks.ToArray());
      
      //draw tabs
      
      
      int ranksCount = _ranks.Length;
      var  toolbarOptions = new GUIContent[ranksCount];
      int currentRank = 0;
      foreach (var rank in _ranks)
      {
        toolbarOptions[currentRank] = new GUIContent(rank.Name+ " {"+rank.Value+"}",null, "");
        currentRank++;
      }
               
               
      var height = 40 * toolbarOptions.Length;
      _selectedStance = GUILayout.SelectionGrid(_selectedStance, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
      
     
    }

    private static void DrawSelected( FactionModel data)
    {
      var factionsEntries = _db.ListEntryNames();

      GUILayout.Label(_ranks[_selectedStance].Name ,EditorStatic.SectionTitle, GUILayout.Width(130));
      EditorStatic.DrawLargeLine(5);
      //list with delete button
      _selectedFactionToAdd = EditorGUILayout.Popup(" faction to add: ", _selectedFactionToAdd, factionsEntries);
      var factionKey = _db.GetKey(_selectedFactionToAdd);
      //popup
      if (EditorStatic.SizeableButton(150,45,"Add faction","plus"))
      {
        
        
       
        if (data.Relations.Contains(factionKey) == false)
        {
          
        
          data.Relations.Add(factionKey, _selectedStance);
        }
        else
        {
          Debug.Log("faction already in other stance");
        }

      }
      
      GUILayout.Label("Added:", GUILayout.Width(70));

      foreach (var stanceNumber in data.Relations)
      {
        if (stanceNumber.Value == _selectedStance)
        {
          GUILayout.BeginHorizontal("box");
          GUILayout.Label(_db.GetEntry(stanceNumber.Key).Name, GUILayout.Width(70));
          if (EditorStatic.SizeableButton(90, 22, "delete", ""))
          {
            data.Relations.Remove(stanceNumber.Key);
            return;
          }
          GUILayout.EndHorizontal();
        }

     

        
      }


    }
    }
}
