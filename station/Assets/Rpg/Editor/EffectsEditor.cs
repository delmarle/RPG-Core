using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    
    public static class AttackEditor 
    {
        public static void DrawAttack(AttackData data)
        {
            EditorGUILayout.HelpBox(" Attack speed = base weapon speed / ((haste percentage / 100) + 1)", MessageType.Info);
            data.BaseDamage.MinValue = EditorGUILayout.FloatField("Min damage: ", data.BaseDamage.MinValue, GUILayout.Width(250));
            data.BaseDamage.MaxValue = EditorGUILayout.FloatField("Max damage: ", data.BaseDamage.MaxValue, GUILayout.Width(250));
            data.Range = EditorGUILayout.FloatField("Default range: ", data.Range, GUILayout.Width(250));
            data.Speed = EditorGUILayout.FloatField("Default attack speed: ", data.Speed, GUILayout.Width(250));
            data.CriticalChance = EditorGUILayout.FloatField("Critical chance (%): ", data.CriticalChance, GUILayout.Width(250));   
        }
    }
    public static class BaseEffectEditor
    {
        public static void Draw(BaseEffect effect)
        {
            EditorGUILayout.BeginHorizontal();
            effect.EffectName = EditorGUILayout.TextField("Name: ",effect.EffectName);
            effect.EffectIcon = (Sprite)EditorGUILayout.ObjectField( effect.EffectIcon, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
        }
    }

    public static class ResurrectEffectEditor
    {public static void Draw(ResurrectEffect effect)
        {
            BaseEffectEditor.Draw(effect);
            effect.percentRefreshed = EditorGUILayout.Slider("Health refreshed percentage: ", effect.percentRefreshed, 0, 1);
        }
        
    }

    public static class TeleportEffectEditor
    {
        public static void Draw(TeleportEffect effect)
        {
            BaseEffectEditor.Draw(effect);
            EditorStatic.DrawDestination(effect.destination);
        }
    }

    public static class BuffEffectEditor
    {
        public static void Draw(ModifierEffect effect)
        {
            var attributesDb = (AttributesDb)EditorStatic.GetDb(typeof(AttributesDb));
            var vitalsDb = (VitalsDb)EditorStatic.GetDb(typeof(VitalsDb));
            var statisticsDb = (StatisticDb)EditorStatic.GetDb(typeof(StatisticDb));
            BaseEffectEditor.Draw(effect);
            effect.Length = EditorGUILayout.FloatField("Length: ", effect.Length);
            effect.Identifier = EditorGUILayout.TextField("identifier", effect.Identifier);
            effect.MaxStack = EditorGUILayout.IntField("Max stack: ", effect.MaxStack);

            EditorStatic.DrawThinLine();
            effect.ModifierType = (ModifierType)EditorGUILayout.EnumPopup(effect.ModifierType);
            EditorStatic.DrawBonusWidget(effect.AttributesBuffs, "Attributes Buffs", attributesDb);
            EditorStatic.DrawThinLine();
            EditorStatic.DrawBonusWidget(effect.VitalsBuffs, "Vitals Buffs", vitalsDb);
            EditorStatic.DrawThinLine();
            EditorStatic.DrawBonusWidget(effect.StatisticsBuffs, "Statistics Buffs", statisticsDb);
        }
    }

    public static class HealEffectEditor
    {
        public static void Draw(HealEffect effect)
        {
            BaseEffectEditor.Draw(effect);
            effect.MinValue = EditorGUILayout.FloatField("Min Heal: ", effect.MinValue);
            effect.MaxValue = EditorGUILayout.FloatField("Max Heal: ", effect.MaxValue);
        }
    }
    
    public static class DamageEffectEditor
    {
        public static void Draw(DamageEffect effect)
        {
            BaseEffectEditor.Draw(effect);
            effect.MinValue = EditorGUILayout.FloatField("Min Damage: ", effect.MinValue);
            effect.MaxValue = EditorGUILayout.FloatField("Max Damage: ", effect.MaxValue);
        }
    }

}