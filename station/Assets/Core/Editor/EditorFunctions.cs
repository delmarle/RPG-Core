using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Station;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace RPG.Editor
{
  public static partial class EditorStatic
  {
    public static T[] GetAllScriptables<T>() where T : ScriptableObject
    {
      string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);
      T[] a = new T[guids.Length];
      for(int i =0;i<guids.Length;i++)
      {
        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
        a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
      }
 
      return a;
    }
    public static BaseDb GetDb(Type name)
    {
      string dbPath = EDITOR_DB_PATH + name.Name + @".asset";
      BaseDb found = AssetDatabase.LoadAssetAtPath<BaseDb>(dbPath);
      if (found == null)
      {
        Debug.LogError("cant find db at: "+dbPath);
      }

      return found;
    }

    #region [[ DRAW REUSABLE WIDGETS ]]
    public static void Space()
    {
      try
      {
        EditorGUILayout.Space();
      }
      catch
      {
        GUIUtility.ExitGUI();
      }
    }

    public static bool DrawToggle(string toggleName,bool state)
    {
      //var buttonContent = new GUIContent("   "+toggleName,GetEditorTexture(state?"toggle_on":"toggle_off"));
      return EditorGUILayout.Toggle(toggleName,state);
    }

    public static void DrawEnumOfType(Type genericType, ref int data)
    {
      if (!genericType.IsEnum) return;
        
      string s = Enum.GetName(genericType, data);
      string[] names = Enum.GetNames(genericType);
      int index = 0;
      for (int i = 0; i < names.Length; i++) if (names[i] == s) index = i;
      index = EditorGUILayout.Popup("", index, names, GUILayout.Width(120), GUILayout.Height(30));
      object item = Enum.Parse(genericType, names[index]);
      data = Convert.ToInt32(item);
    }
	
    public static void DrawThinLine(int extraSpace = 0)
    {
      GUILayout.Space(extraSpace);
      GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
      GUILayout.Space(extraSpace);
    }

    public static void DrawLargeLine(int extraSpace = 0)
    {
      GUILayout.Space(extraSpace);
      EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
      GUILayout.Space(extraSpace);
    }
    #endregion
    private static readonly Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();
    public static Texture2D GetEditorTexture(string textureName)
    {
      if (string.IsNullOrEmpty(textureName))
      {
        return null;
      }

      if (_cachedTextures.ContainsKey(textureName) == false)
      {
        string path = EDITOR_ASSETS_PATH + textureName + ".png";
        _cachedTextures.Add(textureName,EditorGUIUtility.Load(path) as Texture2D);
      }

      return _cachedTextures[textureName];
    }
    #region [[ BUTTONS ]]

    public static bool SizeableButton(int width,int height,string text,string icon)
    {
      bool buttonValue = false;
      if(ButtonPressed(text,Color.white,GUILayout.Width(width),GUILayout.Height(height),icon))
      {
        ResetFocus();
        buttonValue = true;
      } 
      return buttonValue;
    }
    
    public static bool SimpleButton(string text,string icon)
    {
      bool buttonValue = false;
      
      GUI.backgroundColor = Color.white;
      var buttonContent = new GUIContent(text,GetEditorTexture(icon));
      GUI.backgroundColor = Color.white;
      if( GUILayout.Button(buttonContent))
      {
        ResetFocus();
        buttonValue = true;
      } 
      return buttonValue;
    }
    
    public static bool Button(bool expandWidth,int height,string text,string icon)
    {
      bool buttonValue = false;
      if(ButtonPressed(text,Color.white,GUILayout.ExpandWidth(expandWidth),GUILayout.Height(height),icon))
      {
        ResetFocus();
        buttonValue = true;
      } 
      return buttonValue;
    }
    
    public static bool IconButtonPressed(string buttonIcon)
    {
      var buttonContent = new GUIContent(GetEditorTexture(buttonIcon));
            
      return GUILayout.Button(buttonContent);
    }
    
    public static bool ButtonPressed(string buttonName,Color buttonColor, string buttonIcon = null)
    {
      GUI.backgroundColor = buttonColor;
      var buttonContent = new GUIContent("   "+buttonName,GetEditorTexture(buttonIcon));
      GUI.backgroundColor = Color.white;
      return GUILayout.Button(buttonContent);
    }
    
    public static bool ButtonPressed(string buttonName,Color buttonColor,GUILayoutOption width,GUILayoutOption height, string buttonIcon = null)
    {
      GUI.backgroundColor = buttonColor;
      var buttonContent = new GUIContent(buttonName,GetEditorTexture(buttonIcon));
      GUI.backgroundColor = Color.white;
      return GUILayout.Button(buttonContent,width,height);
    }
    
    public static bool ButtonDelete(string text = "")
    {
      return Button(EDITOR_BUTTON_SIZE,"cross",text);
    }
    
    public static bool ButtonAdd(string text = "")
    {
      return Button(EDITOR_BUTTON_SIZE,"plus",text);
    }
    
    public static bool Button(float size,string icon,string text = "")
    {
      GUI.backgroundColor = Color.white;
      GUIContent buttonContent;
      if(string.IsNullOrEmpty(text))
        buttonContent = new GUIContent(GetEditorTexture(icon));
      else
        buttonContent = new GUIContent(text,GetEditorTexture(icon));
      GUI.backgroundColor = Color.white;
      return GUILayout.Button(buttonContent,GUILayout.Width(size),GUILayout.Height(size));
    }
#endregion

    #region [[ GENERIC LISTS ]]
    
    public static int DrawGenericSelectionList<T>(
      DictGenericDatabase<T> database,
      int selectedIndex,
      Vector2 currentScrollPos,
      out Vector2 propertyScrollPos,
      string iconName,
      bool intern, Action addAction = null) where T : class, new()
    {
      var entries = database.ListEntryNames();
      GUILayout.BeginVertical("box",GUILayout.Width(LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
      {
        propertyScrollPos = GUILayout.BeginScrollView(currentScrollPos,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
        {
          GUILayout.Label("SELECT "+database.ObjectName()+": " ,GUILayout.Width(LIST_VIEW_WIDTH -30));
          var  toolbarOptions = new GUIContent[database.Count()];
          for (int i = 0; i < database.Count(); i++)
          {
            IStationIcon foundIcon = database.GetEntry(i) as IStationIcon;
            Texture2D icon = null;
            if (foundIcon != null)
            {
              if (foundIcon.GetIcon() != null)
              {
                Object txture = foundIcon?.GetIcon()?.texture;
                if (txture)
                {
                  string nameIcon = AssetDatabase.GetAssetPath(txture);
                  icon = EditorGUIUtility.FindTexture(nameIcon);
                }
              }
            

            }

            if (icon == null)
            {
              icon = intern ? EditorGUIUtility.FindTexture(iconName) : GetEditorTexture(iconName);
            }

            toolbarOptions[i] = new GUIContent(entries[i],icon, "");
          }
         
          var previousIndex = selectedIndex;

          float lElemH2 = toolbarOptions.Length * 40;
          selectedIndex = GUILayout.SelectionGrid(selectedIndex, toolbarOptions,1,VerticalBarStyle, GUILayout.Height(lElemH2), GUILayout.Width(LIST_VIEW_WIDTH - 10));
          if(previousIndex != selectedIndex)ResetFocus();
        }
        GUILayout.EndScrollView();
        #region Add Button
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        {
          string secondLine = "\n Total "+database.ObjectName()+" "+ + database.Count();
          if (addAction == null)
          {
            if(ButtonPressed("Add "+database.ObjectName()+secondLine,Color.white,"plus"))
            {
              database.Add(new T());
              selectedIndex = database.Count() - 1;
              ResetFocus();
              return selectedIndex;
            }
          }
          else
          {
            addAction.Invoke();
          }

         
        }
        GUILayout.EndHorizontal();
        if (database.Count()>0)
        {
          selectedIndex = DrawControls(database,selectedIndex);
        }

        #endregion
      }
      GUILayout.EndVertical();
      return selectedIndex;
    }

    
    public static int DrawGenericList<T>(DictGenericDatabase<T> database,int selectedIndex,Vector2 currentScrollPos,out Vector2 propertyScrollPos,string iconName, bool intern) where T : class, new()
    {
      var entries = database.ListEntryNames();
      GUILayout.BeginVertical("box",GUILayout.Width(LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
      {
        propertyScrollPos = GUILayout.BeginScrollView(currentScrollPos,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
        {
          GUILayout.Label("SELECT "+database.ObjectName()+": " ,GUILayout.Width(100));
          var  toolbarOptions = new GUIContent[database.Count()];
          for (int i = 0; i < database.Count(); i++)
          {
            toolbarOptions[i] = new GUIContent(entries[i],intern?EditorGUIUtility.FindTexture(iconName):GetEditorTexture(iconName), "");
          }

          var previousIndex = selectedIndex;
          selectedIndex = GUILayout.SelectionGrid(selectedIndex, toolbarOptions,1,VerticalBarStyle);
          if(previousIndex != selectedIndex)ResetFocus();
        }
        GUILayout.EndScrollView();
      }
      GUILayout.EndVertical();
      return selectedIndex;
    }
    
    #endregion
    #region [[ CONTROLS ]]
    private static int DrawControls<T>(DictGenericDatabase<T> database,int selectedIndex) where T : class
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.BeginVertical();
        {
          if (IconButtonPressed("bullet_arrow_up"))
          {
            if(database.MoveEntryUp(selectedIndex))
              selectedIndex--;
          }

          if (IconButtonPressed("bullet_arrow_down"))
          {
            if(database.MoveEntryDown(selectedIndex))
              selectedIndex++;
          }

        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        {
          if(IconButtonPressed("duplicate"))
          {
            database.Duplicate(selectedIndex);
            selectedIndex = database.Count()-1;
          }
          if(IconButtonPressed("cross"))
          {
            string objName = database.ObjectName();
            if (EditorUtility.DisplayDialog("Delete confirmation",
              "Do you want to delete: "+objName,"Delete","Cancel"))
            {
              database.Remove(database.GetEntry(selectedIndex));
              ResetFocus();
              var countTotal = database.Count();
              return countTotal > 0 ? countTotal-1 : -1;
            }
          }
        }
        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();
      return selectedIndex;
    }

    #endregion
    
    #region [[ HEADER ]]

    public static void DrawSectionTitle(string title,int extraSpacing)
    {
      GUILayout.Space(extraSpacing);
      GUILayout.Label(string.Format("<b>{0}</b>",title),ReviewBanner,GUILayout.ExpandWidth(true));
      GUILayout.Space(extraSpacing);
    }
    
    public static void DrawSectionTitle(int height, string title)
    {
      LoadStyles();
      GUILayout.Label(string.Format("<b>{0}</b>",title),ReviewBanner,GUILayout.ExpandWidth(true), GUILayout.Height(height));
    }
    
    public static void DrawSectionTitle(string title,int size,int extraSpacing)
    {
      GUILayout.Space(extraSpacing);
      GUILayout.Label(string.Format("<b>{0}</b>",title),ReviewBanner,GUILayout.Width(size),GUILayout.Height(25));
      GUILayout.Space(extraSpacing);
    }


    #endregion
    
    #region [[ DRAW FIELDS WRAPPERS ]]
    public static string DrawTextArea(string label,string property)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label(label, GUILayout.Width(145));
        property = EditorGUILayout.TextArea(property, GUILayout.Height(45));
      }
      GUILayout.EndHorizontal();
      return property;
    }

    public static GameObject DrawGameobjectField(string label, GameObject go)
    {
      go = (GameObject) EditorGUILayout.ObjectField(label, go, typeof(GameObject), false);
      return go;
    }

    #endregion

    #region [[ FOLDOUTS ]]
    
    public static bool LevelFoldout(string title, bool display, int height, Color bgColor)
    {
      var style = new GUIStyle("ShurikenModuleTitle");
      style.font = new GUIStyle(EditorStyles.boldLabel).font;
      float verticalOffset = height * 0.2f;
      GUI.backgroundColor = bgColor;

      style.border = new RectOffset(15, 7, (int)verticalOffset, (int)verticalOffset);
      style.fixedHeight = height;
      style.contentOffset = new Vector2(20f, -2);

      var rect = GUILayoutUtility.GetRect(16f, height, style);
      GUI.Box(rect, title, style);
      var e = Event.current;

      var toggleRect = new Rect(rect.x + 4f, rect.y+5, 13f, 13f);
      if (e.type == EventType.Repaint) {
        EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
      }

      if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) 
      {
        display = !display;
        e.Use();
      }
      GUI.backgroundColor = Color.white;
      return display;
    }

    public static void DrawSoundReference(SoundReferenceDrawer drawer)
    {
      if (drawer.Enabled == false)
      {
        if (SizeableButton(200, 32, "Enable Sound", "red"))
        {
          drawer.Enabled = true;
        }
      }
      else
      {
        if (SizeableButton(200, 32, "Disable Sound", "green"))
        {
          drawer.Enabled = false;
        }
        var soundsDb = (SoundsDb)GetDb(typeof(SoundsDb));
        var groupsNames = soundsDb.ListEntryNames();
        if (soundsDb.Db.Count == 0)
        {
          EditorGUILayout.HelpBox("there is no group in the sound DB", MessageType.Warning);
          GUIUtility.ExitGUI();
        }

        if (string.IsNullOrEmpty(drawer.GroupId))
        {
          drawer.GroupId = soundsDb.GetKey(0);
        }

        var groupFound = soundsDb.GetEntry(drawer.GroupId);
        int index = soundsDb.GetIndex(drawer.GroupId);
        if (soundsDb == null)
        {
          groupFound = soundsDb.GetEntry(0);
        }

        index = EditorGUILayout.Popup(index, groupsNames);
        drawer.GroupId = soundsDb.GetKey(index);
      }
      
    }

    public static bool SoundFoldout(string title,ref SoundConfig sound, bool display, int height, Color bgColor)
    {
      var style = new GUIStyle("ShurikenModuleTitle");
      style.font = new GUIStyle(EditorStyles.boldLabel).font;
      float verticalOffset = height * 0.2f;
      GUI.backgroundColor = bgColor;

      style.border = new RectOffset(15, 7, (int)verticalOffset, (int)verticalOffset);
      style.fixedHeight = height;
      style.contentOffset = new Vector2(20f, -2);

      var rect = GUILayoutUtility.GetRect(16f, height, style);
      GUI.Box(rect, title, style);
      
      var e = Event.current;
      
      
      var toggleRect = new Rect(rect.x + 4f, rect.y+5, 13f, 13f);
      var itemRect = rect;
      itemRect.x += 200;
      itemRect.y += 4;
      itemRect.height = 18;
      itemRect.width = 200;
      sound = (SoundConfig)EditorGUI.ObjectField(itemRect,sound,typeof(SoundConfig), false);
      if (e.type == EventType.Repaint) {
        EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
      }

      if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) 
      {
        display = !display;
        e.Use();
      }
// Code
      GUI.backgroundColor = Color.white;
      return display;
    }
    
    #endregion
    
    #region SOUNDS && EFFECTS

    public static void DrawSoundWidget(ref SoundConfig sound, string category, string optionalFileName = "")
    {
      var styleSubContent = new GUIStyle("SelectionRect");
  
      styleSubContent.padding = new RectOffset(3,3,6,5);
      EditorGUILayout.BeginVertical(styleSubContent);
      if (sound)
      {
        EditorGUILayout.BeginHorizontal();
        sound.Volume = EditorGUILayout.Slider("Volume: ", sound.Volume, 0, 1);
        //delay
        sound.DelayAtStart = EditorGUILayout.Slider("Delay : ", sound.DelayAtStart, 0, 10);
        EditorGUILayout.EndHorizontal();
      
        // fade in
        EditorGUILayout.BeginHorizontal();
        sound.FadeInTime = EditorGUILayout.Slider("Fade In : ", sound.FadeInTime, 0, 10);
        sound.FadeOutTime = EditorGUILayout.Slider("Fade Out : ", sound.FadeOutTime, 0, 10);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.MinMaxSlider("Pitch : ",ref sound.MinPitch, ref sound.MaxPitch, 0,2);
        sound.MinPitch = EditorGUILayout.FloatField(sound.MinPitch, GUILayout.Width(32));
        sound.MaxPitch = EditorGUILayout.FloatField(sound.MaxPitch, GUILayout.Width(32));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        sound.Looping = EditorGUILayout.Toggle("Looping : ", sound.Looping);
        sound.SourceConfig = (SourcePoolConfig)EditorGUILayout.ObjectField("Pool config:", sound.SourceConfig, typeof(SourcePoolConfig), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical("box");
        //HEADER
        EditorGUILayout.BeginHorizontal();
        {
          EditorGUILayout.Space();
          if (SizeableButton(300, 32, "ADD CLIP", "plus"))
          {
            sound.Clips.Add(null);
          }
          EditorGUILayout.Space();
          EditorGUILayout.LabelField("Clips: "+sound.Clips.Count);
        }
        EditorGUILayout.EndHorizontal();

        if (sound.Clips.Count > 0)
        {
          DrawThinLine();
          for (var index = 0; index < sound.Clips.Count; index++)
          {
            //LIST
            EditorGUILayout.BeginHorizontal();
            sound.Clips[index] = (AudioClip)EditorGUILayout.ObjectField("Clip : ", sound.Clips[index], typeof(AudioClip), false);
            if (SizeableButton(100, 18, "DELETE", ""))
            {
              sound.Clips.RemoveAt(index);
              GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
            DrawThinLine();
          }
        }
        EditorGUILayout.EndVertical();
      }
      else
      {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("No sound asset created", MessageType.Warning);
        EditorGUILayout.Space();
        if (SizeableButton(200, 36, "Create sound asset", "plus"))
        {
          string fileName = category+Guid.NewGuid()+".asset";
          if (optionalFileName != "")
          {
             fileName = optionalFileName+".asset";
          }

          var path = PathUtils.BuildSoundPath(category);
          sound = ScriptableHelper.CreateScriptableObject<SoundConfig>(path, fileName);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
      }
      EditorGUILayout.EndVertical();
    }
    
 
    #endregion

    public static void ResetFocus()
    {
      GUI.SetNextControlName("");
      GUI.FocusControl("");
      GUIUtility.keyboardControl = 0;
    }
    
    #region SAVING
    public static void OpenDataFolder()
    {
      if (IoUtils.IsDirectoryExist(PathUtils.SavePath()) == false)
      {
        Debug.LogWarning("The save folder is not created yet.");
      }
      else
      {
        UnityEditor.EditorUtility.RevealInFinder(PathUtils.SavePath()); 
      }

            
    }

    public static void ClearSaveFolder()
    {
      bool success = FileUtil.DeleteFileOrDirectory( PathUtils.SavePath());
      if (success)
      {
        Debug.Log("Deleted the save folder.");
      }
      else
      {
        Debug.LogWarning("The save folder is not found.");
      }
    }
    #endregion
    


    public static void DrawDestination(DestinationModel model)
    {
      if (model != null)
      {
        var scene = model.SceneId;
        var spawn = model.SpawnId;
        var sceneDb = (ScenesDb) GetDb(typeof(ScenesDb));
           
           

        if (sceneDb.Any() == false)
        {
          EditorGUILayout.HelpBox( "no scenes in the database found", MessageType.Warning);
        }
        else
        {
          int index = sceneDb.GetIndex(scene);
          index = EditorGUILayout.Popup(index, sceneDb.ListEntryNames());
          if (index == -1)
          {
            index = 0;
          }
          model.SceneId = sceneDb.GetKey(index);
          var spawnsName = sceneDb.GetEntry(model.SceneId).SpawnNames();
          model.SpawnId = EditorGUILayout.Popup(model.SpawnId,spawnsName );
        }

      }
    }
    
    #region LOCALIZATION

    public static void DrawLocalization(LocalizedText localization, string fieldName = null)
    {
      //TODO
      localization.Key = EditorGUILayout.TextField(fieldName, localization.Key);
    }
    
    public static void DrawLocalizationLabel(LocalizedText localization, int height,string fieldName = null)
    {
      //TODO
      localization.Key = EditorGUILayout.TextField(fieldName, localization.Key, GUILayout.Height(height));
    }

    #endregion

    public static string DrawDbIdReference<T>(BaseDb db, string current) where T : class
    {
      var dictDb = (DictGenericDatabase<T>) db;
      var raritiesDict = dictDb.Db;
      if (raritiesDict.Any())
      {
        if (string.IsNullOrEmpty(current) || dictDb.HasKey(current) == false)
        {
          current = dictDb.GetKey(0);
        }

        string prefix = dictDb.ObjectName() + ":";
        int currentIndex = dictDb.GetIndex(current);
        currentIndex = EditorGUILayout.Popup(prefix, currentIndex, dictDb.ListEntryNames());
        current = dictDb.GetKey(currentIndex);
      }
      else
      {
        EditorGUILayout.HelpBox("Missing entries in the db: "+dictDb.name, MessageType.Warning);
      }
      return current;
    }
  }
}


