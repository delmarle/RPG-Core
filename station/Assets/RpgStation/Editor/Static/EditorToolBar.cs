using UnityEngine;

namespace RPG.Editor
{
  public static partial class EditorStatic
  {
    #region FIELDS
    public static GUIContent[] ToolbarOptions;  
    public static GUILayoutOption ToolbarHeight;

    #endregion

    private static void LoadToolBar()
    {
      ToolbarHeight = GUILayout.Height(50);
      ToolbarOptions = new GUIContent[9];
      
      ToolbarOptions[0] = new GUIContent("<size=11><b> Game config</b></size>",GetEditorTexture("cog"),"");
      ToolbarOptions[1] = new GUIContent("<size=11><b> Stats</b></size>", GetEditorTexture("heart"), "");
      ToolbarOptions[2] = new GUIContent("<size=11><b> Characters</b></size>", GetEditorTexture("group"), "");
      ToolbarOptions[3] = new GUIContent("<size=11><b> Factions</b></size>", GetEditorTexture("flag_flyaway_green"), "");
      ToolbarOptions[4] = new GUIContent("<size=11><b> Progression</b></size>", GetEditorTexture("plus_button"), "");
      ToolbarOptions[5] = new GUIContent("<size=11><b> Skills</b></size>", GetEditorTexture("tree_list"), "");
      ToolbarOptions[6] = new GUIContent("<size=11><b> Items</b></size>", GetEditorTexture("package"), "");
      ToolbarOptions[7] = new GUIContent("<size=11><b> World</b></size>", GetEditorTexture("map"), "");
      ToolbarOptions[8] = new GUIContent("<size=11><b> Interactions</b></size>", GetEditorTexture("hand_property"), "");
    }
  }
}


