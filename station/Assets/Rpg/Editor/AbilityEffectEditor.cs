using System;
using System.Collections;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
   public static class AbilityEffectEditor
{
  private static EffectHolder _cachedHolder;
  
  #region [[EFFECT STACK ]]
  public static void DrawEffectStack(EffectHolder holder)
  {
    //Add button
    EditorStatic.DrawSectionTitle(20,"Effects: ");
    if (EditorStatic.SizeableButton(140, 20, "Add", "plus"))
    {
      GenericMenu menu = new GenericMenu();
      var list = ReflectionUtils.GetClassList<BaseEffect>();
      for (var index = 0; index < list.Length; index++)
      {
        var entry = list[index];
        AddEffectEntry(menu, entry.Name, index);
      }

      menu.ShowAsContext();
      _cachedHolder = holder;
    }

    if (holder.Damages != null)
    {
      foreach (var dmg in holder.Damages)
      {
        DrawEffect("Damage", holder.Damages, dmg, () => {  DamageEffectEditor.Draw(dmg);});
      }
    }
    
    if (holder.Heals != null)
    {
      foreach (var heal in holder.Heals)
      {
        DrawEffect("Heal", holder.Heals, heal, () => {  HealEffectEditor.Draw(heal);});
      }
    }

    if (holder.Modifiers != null)
    {
      foreach (var buff in holder.Modifiers)
      {
        DrawEffect("Modifier", holder.Modifiers, buff, () => {  BuffEffectEditor.Draw(buff);});
      }
    }
    
    if (holder.Teleports != null)
    {
      foreach (var tp in holder.Teleports)
      {
        DrawEffect("Teleport", holder.Teleports, tp, () => {  TeleportEffectEditor.Draw(tp);});
      }
    }
    
    if (holder.Resurects != null)
    {
      foreach (var tp in holder.Resurects)
      {
        DrawEffect("Resurrect", holder.Teleports, tp, () => {  ResurrectEffectEditor.Draw(tp);});
      }
    }
  }

  private static void DrawEffect(string title, IList list, object item, Action drawCallback)
  {
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.BeginVertical("box", GUILayout.Height(42));
    EditorGUILayout.LabelField(title, EditorStatic.SectionTitle, GUILayout.Width(100), GUILayout.Height(36));
    EditorGUILayout.EndVertical();
    
    EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true),GUILayout.Height(42));
    drawCallback.Invoke();
    EditorGUILayout.EndVertical();
    
    EditorGUILayout.BeginVertical(EditorStatic.CenteredVersionLabel, GUILayout.Width(64));
    if (EditorStatic.SizeableButton(60, 36, "DELETE", ""))
    {
      list.Remove(item);
      GUIUtility.ExitGUI();
    }
    EditorGUILayout.EndVertical();
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.Space();

  }


  private static void AddEffectEntry(GenericMenu menu, string menuPath, int idType)
  {
    menu.AddItem(new GUIContent(menuPath),false, OnEffectSelected, idType);
  }
    
  private static void OnEffectSelected(object idType)
  {
    int id = (int)idType;
    switch (id)
    {
      case 0:
        _cachedHolder.Resurects.Add(new ResurrectEffect());
        break;
      case 1:
        _cachedHolder.Teleports.Add(new TeleportEffect());
        break;
      case 2:
        _cachedHolder.Modifiers.Add(new ModifierEffect());
        break;
      case 3:
        _cachedHolder.Heals.Add(new HealEffect());
        break;  
      case 4:
        _cachedHolder.Damages.Add(new DamageEffect());
        break;
    }
  }

  #endregion
} 

}
