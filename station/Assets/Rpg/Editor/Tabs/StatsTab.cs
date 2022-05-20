using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEngine;

namespace Station
{
    public class StatsTab
    {
    #region [[FIELDS]]
    private static int _toolBarIndex;
    private string _categoryName;
    private object _selecteditem;
    #endregion
    
    public static void DrawTab()
    {
    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
    {
        DrawLeftBar();
        DrawProperties();
    }
    GUILayout.EndHorizontal();
    }

    #region [[ DRAW SECTIONS ]]
    private static void DrawLeftBar()
    {
    GUILayout.BeginVertical("box",GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
    {
        var  toolbarOptions = new GUIContent[5];
        toolbarOptions[0] = new GUIContent(EditorStatic.STATS_TAB_ATTRIBUTES,null, "");
        toolbarOptions[1] = new GUIContent(EditorStatic.STATS_TAB_VITALS, null, "");
        toolbarOptions[2] = new GUIContent(EditorStatic.STATS_TAB_STATISTICS, null, "");
        toolbarOptions[3] = new GUIContent(EditorStatic.STATS_TAB_ELEMENTS, null, "");
        toolbarOptions[4] = new GUIContent(EditorStatic.STATS_TAB_STATUS_EFFECTS, null, "");
        var previousIndex = _toolBarIndex;
        var height = 40 * toolbarOptions.Length;
        _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
        if(_toolBarIndex != previousIndex)EditorStatic.ResetFocus();
    }
    GUILayout.EndVertical();
    }
    
    private static void DrawProperties()
    {
    switch (_toolBarIndex)
    {
        case 0:
        AttributesEditor.DrawContent();
        break;
        case 1:
        VitalsEditor.DrawContent();
        break;
        case 2:
        StatisticEditor.DrawContent();
        break;
        case 3:
       // ElementEditor.DrawContent();
        break;
                
    }
    }


    #endregion
}
}

