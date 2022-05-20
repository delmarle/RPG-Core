
using System;
using UnityEngine;

namespace Station
{
   

    public class UiCharacterHotBar : MonoBehaviour
    {
        #region [[ FIELDS ]]

        private const float FADE_TIME = 0.1f;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private UiBarLinkWidget _abPrefab = null;

        private BaseCharacter _cachedCharacter;

        #endregion
        public void OnCreate(BaseCharacter character)
        {
            _cachedCharacter = character;
            int max = ActionHandler.LINKED_AMOUNT;
            for (int i = 0; i < max; i++)
            {
                var instance = Instantiate(_abPrefab, _canvasGroup.transform);
                var bindType = character.Action.GetLinkType("main", i);
                switch (bindType)
                {
                    case LinkType.Ability:
                        var ability = character.Action.GetBindAbility("main", i);
                        instance.BindAction(ability, character.Action);
                        break;
                    case LinkType.Inventory:
                        break;
                    case LinkType.Empty:
                        instance.BindAction(null,character.Action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }
 
        public void OnCancel()
        {
      
        }

        private void OnAbilityLinkUpdated(BaseCharacter character)
        {
            Debug.Log(_cachedCharacter);
        }

        public void Select()
        {
            if (_canvasGroup.alpha < 0.9f)
            {
                _canvasGroup.alpha = 1;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

      
        }

        public void Unselect()
        {
            if (_canvasGroup.alpha > 0.1f)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }
    }

}
