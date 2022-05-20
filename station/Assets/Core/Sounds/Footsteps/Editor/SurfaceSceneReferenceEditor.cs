using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Station
{
    [CustomEditor(typeof(SurfaceSceneReference))]
    [CanEditMultipleObjects]
    public class SurfaceSceneReferenceEditor : UnityEditor.Editor
    {
         public List<bool> _entriesStates = new List<bool>();
        private FootstepsDb _footstepDb;

        private void Cache()
        {
            _footstepDb = (FootstepsDb) EditorStatic.GetDb(typeof(FootstepsDb));
        }

        public override void OnInspectorGUI()
        {
            Cache();
            EditorGUI.BeginChangeCheck();

            var sceneReference = (SurfaceSceneReference) target;
            EnforceListCorrect(sceneReference);
            //  base.DrawDefaultInspector();
       

            EditorStatic.DrawThinLine(2);
           
            if (EditorStatic.Button(true, 32, "Clear", "cog"))
            {
            }
            EditorGUILayout.HelpBox("Add the texture used in the scene for each category", MessageType.Info);
            EditorStatic.DrawThinLine(2);
            for (var index = 0; index < sceneReference.Entries.Count; index++)
            {
                DrawEntry(index, sceneReference);
            }

            if (EditorGUI.EndChangeCheck())
            {
                // This code will unsave the current scene if there's any change in the editor GUI.
                // Hence user would forcefully need to save the scene before changing scene
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private void EnforceListCorrect(SurfaceSceneReference component)
        {
            while (_entriesStates.Count < component.Entries.Count)
            {
                _entriesStates.Add(false);
            }

            while (component.Entries.Count < _footstepDb.Surfaces.Count)
            {
                component.Entries.Add(new SurfaceSceneEntry());
            }
            
            while (component.Entries.Count > _footstepDb.Surfaces.Count)
            {
                component.Entries.RemoveAt(component.Entries.Count-1);
            }

            for (var index = 0; index < _footstepDb.Surfaces.Count; index++)
            {
                var surface = _footstepDb.Surfaces[index];
                component.Entries[index].SurfaceName = surface;
            }
        }


        private static bool SurfaceFoldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);
            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }

        private void DrawEntry(int index, SurfaceSceneReference component)
        {
            
            var entry = component.Entries[index];
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
           
     

            _entriesStates[index] = SurfaceFoldout(entry.SurfaceName, _entriesStates[index]);
  

            EditorGUILayout.EndHorizontal();
           
            if (_entriesStates[index])
            {
                EditorGUILayout.BeginVertical("box");
                if (EditorStatic.Button(true, 32, "Add Texture", "plus"))
                {
                    entry.Textures.Add(null);
                }
                for (var i = 0; i < entry.Textures.Count; i++)
                {
                    var texture = entry.Textures[i];
                    EditorGUILayout.BeginHorizontal();
                    entry.Textures[i] = (Texture) EditorGUILayout.ObjectField(texture, typeof(Texture), true);
                    if (EditorStatic.Button(true, 18, "DELETE", ""))
                    {
                        entry.Textures.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

        }
        
    }
}

