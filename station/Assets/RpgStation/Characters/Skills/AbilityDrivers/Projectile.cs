using System;
using UnityEditor;
using UnityEngine;

namespace Station
{
  //RUNTIME CODE
  [Serializable]
  public class Projectile : BaseAbilityDriver
  {
    public EffectHolder Effects = new EffectHolder();
    public float TravelSpeed = 3f;


    public GameObject EffectPrefab;
    //list of effect you can get in base
    
    #if UNITY_EDITOR
    protected override void DrawFields()
    {
      TravelSpeed = EditorGUILayout.FloatField("Travel speed: ", TravelSpeed);
      EffectPrefab = (GameObject)EditorGUILayout.ObjectField("FX Prefab: ", EffectPrefab, typeof(GameObject),false);
    }
    #endif
  }
}

