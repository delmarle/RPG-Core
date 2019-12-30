using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
  public static partial class EditorStatic
  {
    #region FIELDS

    public static GUIStyle PublisherNameStyle;
    public static GUIStyle ToolBarStyle;
    public static GUIStyle VerticalBarStyle;
    public static GUIStyle SectionTitle;
    public static GUIStyle CenteredVersionLabel;
    public static GUIStyle ReviewBanner;
    public static GUIStyle SectionHeader;
    public static GUIStyle HeaderText;
    public static GUIStyle CenterIntField;
    
    private static bool _stylesNotLoaded = true;

    #endregion
  
    public static void LoadStyles()
    {
      if (_stylesNotLoaded == false) return;
    
      LoadToolBar();
      PublisherNameStyle = new GUIStyle(EditorStyles.label)
      {
        alignment = TextAnchor.MiddleCenter,
        richText = true
      };

      ToolBarStyle = new GUIStyle("LargeButtonMid")
      {
        alignment = TextAnchor.MiddleCenter,
        richText = true
      };

      VerticalBarStyle = new GUIStyle("LargeButtonMid")
      {
        alignment = TextAnchor.MiddleLeft,
        richText = true,
        margin = new RectOffset(2,2,2,2)
      };

      SectionTitle = new GUIStyle(EditorStyles.boldLabel)
      {
        alignment = TextAnchor.MiddleLeft,
        fontSize = 20
        
        
      };

      CenteredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
      {
        alignment = TextAnchor.MiddleCenter,
      };

      ReviewBanner = new GUIStyle("AppToolbar")
      {
        alignment = TextAnchor.MiddleCenter,
        richText = true
      };
      
      SectionHeader = new GUIStyle("ShurikenEffectBg")
      {
        alignment = TextAnchor.MiddleCenter,
        richText = true,
        padding = new RectOffset(2,2,2,6)
      };
      
      HeaderText = new GUIStyle("IN TitleText")
      {
        alignment = TextAnchor.LowerCenter,
        richText = true,
        contentOffset = new Vector2(0,5)
      };

      CenterIntField = new GUIStyle(EditorStyles.numberField)
      {
        alignment = TextAnchor.MiddleCenter,
        richText = true,
      };

      _stylesNotLoaded = false;
    }

	
  }
}


