#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Station
{
  public static class DrawUtils
  {
    static readonly float padSize = 2f;
    /// <summary>
    /// Draw a GUI button, choosing between a short and a long button text based on if it fits
    /// </summary>
    static public bool ButtonHelper(Rect position, string msgShort, string msgLong, GUIStyle style,
      string tooltip = null)
    {
      GUIContent content = new GUIContent(msgLong);
      content.tooltip = tooltip;

      float longWidth = style.CalcSize(content).x;
      if (longWidth > position.width)
        content.text = msgShort;

      return GUI.Button(position, content, style);
    }

    /// <summary>
    /// Given a position rect, get its field portion
    /// </summary>
    static public Rect GetFieldRect(Rect position)
    {
      position.width -= EditorGUIUtility.labelWidth;
      position.x += EditorGUIUtility.labelWidth;
      return position;
    }

    /// <summary>
    /// Given a position rect, get its label portion
    /// </summary>
    static public Rect GetLabelRect(Rect position)
    {
      position.width = EditorGUIUtility.labelWidth - padSize;
      return position;
    }
  }
}
#endif