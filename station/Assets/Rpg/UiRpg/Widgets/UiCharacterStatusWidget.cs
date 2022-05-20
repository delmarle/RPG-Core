using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterStatusWidget : MonoBehaviour
    {
        [SerializeField] private UiCharacterEffectIconWidget _statusPrefab = null;
        [SerializeField] private LayoutGroup _listRoot = null;
        
        private BaseCharacter _target;
        private GenericUiList<RuntimeModifier, UiCharacterEffectIconWidget> _widgetList;

        public void Setup(BaseCharacter character)
        {
            if (_target != null)
            {
                UnSubscribe();
            }

            _target = character;
            
            Subscribe();
        }

        private void Awake()
        {
            _widgetList = new GenericUiList<RuntimeModifier, UiCharacterEffectIconWidget>(_statusPrefab.gameObject, _listRoot);
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }
        
        private void Subscribe()
        {
            if (_target == null) return;
            
            _target.Stats.OnModifierAdded+= OnModifierAdded;
            _target.Stats.OnModifierRemoved+= OnModifierRemoved;
        }
        
        private void UnSubscribe()
        {
            if (_target == null) return;
            
            _target.Stats.OnModifierAdded-= OnModifierAdded;
            _target.Stats.OnModifierRemoved-= OnModifierRemoved;
        }

        
        private void SetList(BaseCharacter character)
        {
            Dictionary<string, RuntimeModifier>.ValueCollection list = character ? character.Stats.Modifiers.Values : null;
            _widgetList.Generate(list, (entry, item) => { item.Setup(entry); });
        }
        
        private void OnModifierRemoved(RuntimeModifier modifier, BaseCharacter source)
        {
            
            SetList(_target);
        }

        private void OnModifierAdded(RuntimeModifier modifier, BaseCharacter source)
        {
            
            SetList(_target);
        }
    }
}

