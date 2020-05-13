using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class InteractionConfigEditor  
    {
      private static Vector2 _scrollPos;
      private static Texture2D _selectedTexture;
      private static bool _pickingTexture;
      private static bool _showCastingData;
      private static bool _showCastingSound;
      private static Vector2 _propertyScrollPos;
      private static int _selectedIndex;
      private static InteractionConfigsDb _localDb;

      public static void DrawContent()
      {
        if (_localDb == null)
        {
          _localDb = (InteractionConfigsDb) EditorStatic.GetDb(typeof(InteractionConfigsDb));
        }

        if (_localDb == null)
        {
          EditorGUILayout.HelpBox("Could not find the db: InteractionConfigsDb", MessageType.Warning);
          return;
        }

        EditorGUI.BeginChangeCheck();
        {
          DrawAttributesList();
          ListView(_selectedIndex);
        }
        if (EditorGUI.EndChangeCheck())
        {
          _localDb.ForceRefresh();
        }
      }
    
        #region [[ DRAW ]]

        private static void DrawAttributesList()
        {
            AutoResizeDb();
            _selectedIndex = EditorStatic.DrawGenericList(_localDb, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_yellow",false);
        }
        #endregion

        private static void AutoResizeDb()
        {
    
            var foundClasses = ReflectionUtils.GetClassList<Interactible>();
            var size = foundClasses.Length;

     
            while (_localDb.Count()>size) _localDb.Remove(_localDb.GetEntry(_localDb.Count()-1));
            while (_localDb.Count() < size) _localDb.Add(new InteractionConfig());

            for (int i = 0; i < _localDb.Count(); i++)
            {
                _localDb.GetEntry(i).InteractibleType = foundClasses[i];
            }
        }
        
          private static void ListView(int selectedIndex)
    {
      var interactionCount = _localDb.Count();
      if (selectedIndex == -1) return;
      if (interactionCount == 0) return;
      if (interactionCount < selectedIndex + 1) selectedIndex = 0;

      var currentClass = _localDb.GetEntry(selectedIndex);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos =
          EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        {
          InteractionConfigPanel(currentClass, selectedIndex);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void InteractionConfigPanel(InteractionConfig current, int selectedId)
    {
      GUILayout.Label("EDIT Config:", GUILayout.Width(90));
      EditorStatic.DrawLargeLine(5);
      if (current != null)
      {
        FirstSection(current, selectedId);
      }
      EditorStatic.DrawThinLine(10);
    }

    #region [[ SECTIONS ]] 
    private static void FirstSection(InteractionConfig current, int selectedId)
    {
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        GUILayout.BeginVertical();
        {
          EditorGUILayout.LabelField(current.InteractibleType.Type.Name, EditorStatic.SectionTitle, GUILayout.Height(25));
          GUILayout.Space(3);
          current.Layer = EditorGUILayout.LayerField("GameObject layer:", current.Layer);
          GUILayout.Space(3);
          current.ShowHintMode = (ShowHintType)EditorGUILayout.EnumPopup("Show Hint: ", current.ShowHintMode);
          GUILayout.Space(3);
          GUILayout.BeginVertical("box");
          {
            EditorGUILayout.LabelField("Hide hint option",EditorStatic.SectionTitle, GUILayout.Height(25));
            GUILayout.Space(5);
            if (current.ShowHintMode != ShowHintType.None )
            {
              current.HideHintOptions.UseDelay = EditorGUILayout.Toggle("Use Delay: ", current.HideHintOptions.UseDelay);
              if (current.HideHintOptions.UseDelay)
              {
                current.HideHintOptions.Delay = EditorGUILayout.FloatField("Delay: ", current.HideHintOptions.Delay);
              }
              current.HideHintOptions.UseDistance = EditorGUILayout.Toggle("Use Distance: ", current.HideHintOptions.UseDistance);
              if (current.HideHintOptions.UseDistance)
              {
                current.HideHintOptions.Distance = EditorGUILayout.FloatField(" Distance: ", current.HideHintOptions.Distance);
              }
              current.HideHintOptions.UseCameraAngle = EditorGUILayout.Toggle("Use Camera angle: ", current.HideHintOptions.UseCameraAngle);
              if (current.HideHintOptions.UseCameraAngle)
              {
                current.HideHintOptions.CameraAngle = EditorGUILayout.FloatField("Camera Angle: ", current.HideHintOptions.CameraAngle);
              }
              GUILayout.Space(3);
            }
            else
            {
              current.HideHintOptions.UseDelay = false;
              current.HideHintOptions.UseDistance = false;
              current.HideHintOptions.UseCameraAngle = false;
              EditorGUILayout.LabelField("No hint will be used");
            }
          }
          GUILayout.EndVertical();
         
          current.TryInteractMode = (InteractType)EditorGUILayout.EnumPopup("Try Interact: ", current.TryInteractMode);
          if (current.TryInteractMode == InteractType.Tap ||
              current.TryInteractMode == InteractType.EnterDistance ||
              current.TryInteractMode == InteractType.HoverAndInput ||
              current.TryInteractMode == InteractType.UiInput)
          {
            current.InteractionRange = EditorGUILayout.FloatField("Max distance: ", current.InteractionRange);
          }
          
          switch (current.TryInteractMode)
          {
            case InteractType.None:
              break;
            case InteractType.Tap:
              break;
            case InteractType.EnterDistance:
              break;
            case InteractType.Collide:
              break;
            case InteractType.HoverAndInput:
              current.HoverMode = (HoverMode)EditorGUILayout.EnumPopup("Mode: ", current.HoverMode);
              break;
            case InteractType.UiInput:
              break;
          }
          GUILayout.Space(3);
        }
        EditorStatic.DrawCastingData(ref current._CastingData, ref _showCastingData, ref _showCastingSound);
        GUILayout.EndVertical();

     

      }
      GUILayout.EndHorizontal();
    }

    #endregion
    }
    
}