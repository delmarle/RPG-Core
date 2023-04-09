
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Station
{
    public class UiInteractionPopup : UiPopup
    {
        #region FIELDS

        public const string POPUP_ID = "interaction_popup";
        
        [SerializeField] private TextMeshProUGUI _nameText = null;
        [SerializeField] private TextMeshProUGUI _roleText = null;
        [SerializeField] private Image _icon = null;

        [SerializeField] private LayoutGroup _entriesRoot = null;
        [SerializeField] private UiButton _prefabEntry = null;
        
        private GenericUiList<InteractionLine, UiButton> _entriesList;
        private BaseCharacter _owner;
        private BaseCharacter _demander;
        private Interactible _interactionComponent;
        private List<InteractionLine> _interactions;
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _entriesList = new GenericUiList<InteractionLine, UiButton>(_prefabEntry.gameObject, _entriesRoot);
        }

        public void SetData(BaseCharacter owner, BaseCharacter demander,Interactible interaction, List<InteractionLine> interactions)
        {
            _owner = owner;
            _demander = demander;
            _interactionComponent = interaction;
            _interactions = interactions;

            _nameText.text = _owner.GetLocalizedName();
            _roleText.text = _owner.GetLocalizedRole();
            _icon.sprite = _owner.GetLocalizedIcon();
            int index = 0;
            _entriesList.Generate(interactions, (data, button) =>
            {
                if (data.CanTrigger(demander))
                {
                    button.SetName(data.GetUnLockedLocalization());
                }
                else
                {
                    button.SetName(data.GetLockedLocalization());
                }
                
                button.SetIndex(index);
                button.SetCallBack(OnClickInteraction);
                index++;
            });
        }

        private void OnClickInteraction(int interactionIndex)
        {
            var clickedEntry = _interactions[interactionIndex];
            clickedEntry.Trigger(_owner, _demander);
        }

        public void OnDeselect()
        {
            _owner.Action.CancelCasting();
           _interactionComponent.OnCancelInteraction(_owner);
           //Mouse was clicked outside
            Hide();
        }
    }

}
