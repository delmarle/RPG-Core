using System;
using System.Collections;
using System.Collections.Generic;
using Malee.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    [CustomEditor(typeof(ClipsAnimation))]
    [CanEditMultipleObjects]
    public class ClipsAnimationEditor : UnityEditor.Editor
    {
        private ReorderableList reorderableList;
        
        private void OnEnable()
        {
            reorderableList = new ReorderableList(serializedObject.FindProperty("AnimationModels"));
        }



        public override void OnInspectorGUI()
        {
            var anim = target as ClipsAnimation;
            if (anim == null) return;
            
        
          
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseDefaultState"));
            if (anim.UseDefaultState)
            {
                EditorGUILayout.BeginVertical("box");
               
               EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultState"));

               EditorGUILayout.EndVertical();
            }

           
            //draw the list using GUILayout, you can of course specify your own position and label
            reorderableList.DoLayoutList();


            SetClipLegacy();
            SetAnimation();
            serializedObject.ApplyModifiedProperties();
        }

        private void SetClipLegacy()
        {
            var anim = target as ClipsAnimation;

            if (anim || anim.AnimationModels!= null)
            {
                foreach (var animation in anim.AnimationModels)
                {
                    if (animation.AnimationClipModels != null)
                    {
                        foreach (var entry in animation.AnimationClipModels)
                        {
                            if (entry.Clip)
                            {
                                entry.Clip.legacy = true;
                            }

                          
                        }
                    }

                   
                }
            }
        }

        private void SetAnimation()
        {
            var anim = target as ClipsAnimation;
            var animation = anim.GetComponent<Animation>();
            if (animation)
            {
                animation.playAutomatically = false;
            }
        }
    }
}

