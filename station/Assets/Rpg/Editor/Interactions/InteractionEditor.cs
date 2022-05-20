using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class InteractionEditor 
    {
        public static List<InteractionLine> DrawInteractionLineList(List<InteractionLine> lines, string widgetName)
        {
            //header
            EditorGUILayout.BeginHorizontal();
            EditorStatic.DrawSectionTitle(28, widgetName);
            
            if (EditorStatic.SizeableButton(120, 30, "add interaction", ""))
            {
                lines.Add(null);
            }
            EditorGUILayout.EndHorizontal();
            EditorStatic.DrawThinLine(1);
            EditorGUILayout.BeginVertical("box");
            for (var index = 0; index < lines.Count; index++)
            {
                var line = lines[index];
                EditorGUILayout.BeginHorizontal();


                if (line == null)
                {
                    //button to create asset
                    if (EditorStatic.SizeableButton(90, 20, "Create asset", ""))
                    {
                        DrawAddButton(index, lines);
                    }
                }
                else
                {
                }

                lines[index] = (InteractionLine)EditorGUILayout.ObjectField(line, (typeof(InteractionLine)));
                //delete button
                if (EditorStatic.SizeableButton(20, 20, "X", ""))
                {
                    lines.RemoveAt(index);
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            return lines;
        }

        private static void DrawAddButton(int index, List<InteractionLine> list)
        {
            var lookupItemTypes = typeof(InteractionLine);
            var found = ReflectionUtils.FindDerivedClasses(lookupItemTypes);
            GenericMenu toolsMenu = new GenericMenu();
            foreach (var entry in found)
            {
                toolsMenu.AddItem(new GUIContent(entry.Name), false, (data =>
                {
                    var instance = AddItemType(entry);
                    list[index] = instance;
                }), entry.FullName);
            }
            toolsMenu.ShowAsContext();
        }
        
        private static InteractionLine AddItemType(object  itemType)
        {
            var assembly = typeof(InteractionLine).Assembly;
            Type t = assembly.GetType(itemType.ToString());
            var path = PathUtils.BuildInteractionPath();
            var instance = ScriptableHelper.CreateScriptableObject(t, path, "Interaction_"+t.Name+"_"+Guid.NewGuid()+".asset");
            Selection.activeObject = instance;
            EditorGUIUtility.PingObject(instance);
            return (InteractionLine)instance;
        }
    }

}
