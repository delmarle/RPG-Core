using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Station
{
    [CustomEditor(typeof(UiHudLinks))]
    [CanEditMultipleObjects]
    public class UiHudLinksEditor : UnityEditor.Editor
    {
        private static string[] IGNORED_CLASSES = new[] { "UiPanel","UiPanelPlayerSwitchSync"};
        private Type[] baseClassList;
        List<string> ClassAvaillable =  new List<string>();
        private void OnEnable()
        {
            baseClassList = ReflectionUtils.FindDerivedClasses(typeof(UiPanel)).ToArray();
            foreach (var c in baseClassList)
            {
                if (IGNORED_CLASSES.Contains(c.Name) == false)
                {
                    ClassAvaillable.Add(c.FullName);
                }
               
            }
    

            
        }

        public override void OnInspectorGUI()
        {
            var t = (target as UiHudLinks);
            base.DrawDefaultInspector();
           
            int drawIndex = 0;
            for (var index = 0; index < ClassAvaillable.Count; index++)
            {
                var c = ClassAvaillable[index];
                if (t.PopupType == c)
                {
                    drawIndex = index;
                }
            }

            t.PopupType = ClassAvaillable[EditorGUILayout.Popup("Popup to open: ",drawIndex, ClassAvaillable.ToArray())];
            
        }
    }

}
