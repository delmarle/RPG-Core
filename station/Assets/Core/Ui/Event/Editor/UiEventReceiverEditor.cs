using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Station
{
    [CustomEditor(typeof(UiEventReceiver))]
    [CanEditMultipleObjects]
    public class UiEventReceiverEditor : UnityEditor.Editor
    {
        private List<bool> _foldoutStates = new List<bool>();
        public override void OnInspectorGUI()
        {
            var component = target as UiEventReceiver;
            if (component == null) return;
            SetFoldoutToCorrectSize(component.Events.Count);
            EditorStatic.DrawSectionTitle(24, "Ui Event Receiver");
            if (EditorStatic.Button(true,32,"Add event", "plus"))
            {
                component.Events.Add(new UiEventReceiverData());
                GUIUtility.ExitGUI();
            }
            for (int i = 0; i < component.Events.Count; i++)
            {
                var ev = component.Events[i];
                var so = ev.EventData;
                string name = so != null ? so.name : "empty";
                _foldoutStates[i] = EditorStatic.LevelFoldout(name,  _foldoutStates[i], 22, Color.gray);
               
                if (_foldoutStates[i])
                {
                    ev.EventData = (UiEventData)EditorGUILayout.ObjectField("event asset:", ev.EventData, typeof(UiEventData), false);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (EditorStatic.Button(true, 22, "Remove event", "cross"))
                        {
                            component.Events.RemoveAt(i);
                            EditorGUILayout.EndVertical();
                            GUIUtility.ExitGUI();
                        }

                        if (EditorStatic.Button(true, 22, "Panel to show", "plus"))
                        {
                            ev._panelToShow.Add(null);
                        }

                        if (EditorStatic.Button(true, 22, "Panel to hide", "plus"))
                        {
                            ev._panelToHide.Add(null);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                  
                    EditorStatic.DrawLargeLine();
                    EditorStatic.DrawSectionTitle(24, "Panels to show:");
                    for (int j = 0; j < ev._panelToShow.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        ev._panelToShow[j] = (UiPanel)EditorGUILayout.ObjectField($"{j}:", ev._panelToShow[j], typeof(UiPanel), true);
                        EditorStatic.DrawThinLine();
                        if (EditorStatic.SizeableButton(50, 20, "X", ""))
                        {
                            ev._panelToShow.RemoveAt(j);
                            EditorGUILayout.EndHorizontal();

                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorStatic.DrawLargeLine();
                    EditorStatic.DrawSectionTitle(24, "Panels to hide:");
                    for (int j = 0; j < ev._panelToHide.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        ev._panelToHide[j] = (UiPanel)EditorGUILayout.ObjectField($"{j}:", ev._panelToHide[j], typeof(UiPanel), true);
                        EditorStatic.DrawThinLine();
                        if (EditorStatic.SizeableButton(50, 20, "X", ""))
                        {
                            ev._panelToHide.RemoveAt(j);
                            EditorGUILayout.EndHorizontal();

                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void SetFoldoutToCorrectSize(int size)
        {
            while (_foldoutStates.Count < size) { _foldoutStates.Add(false); }
            while (_foldoutStates.Count > size) { _foldoutStates.RemoveAt(_foldoutStates.Count - 1); }
        }
    }
}

