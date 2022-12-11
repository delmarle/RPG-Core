using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Station
{
    public class EntityInteraction : Interactible
    {
        public List<InteractionLine> Topics = new List<InteractionLine>();
        private BaseCharacter _owner;
        private UiInteractionPopup _cachedInteractionPopup;

        public void SetCharacterOwner(BaseCharacter owner)
        {
            _owner = owner;
        }

        public void SetTopics(List<InteractionLine> data)
        {
            Topics = data;
        }

        public override void Interact(BaseCharacter user)
        {
            base.Interact(user);
            if (_cachedInteractionPopup == null)
            {
                _cachedInteractionPopup = UiSystem.GetUniquePopup<UiInteractionPopup>(UiInteractionPopup.POPUP_ID);
            }

            _cachedInteractionPopup.SetData(_owner, user, this, Topics);
            _cachedInteractionPopup.Show();
        }

        public override void OnCancelInteraction(BaseCharacter user)
        {
            UiSystem.HideUniquePopup<UiInteractionPopup>(UiInteractionPopup.POPUP_ID);
            _cachedInteractionPopup.Hide();
            base.OnCancelInteraction(user);
        }

        public override string GetInteractionName()
        {
            return _owner.GetMeta<string>(RpgConst.CHARACTER_NAME);
        }

        protected override void Setup()
        {
            base.Setup();
            SetUiName(_owner.GetMeta<string>(RpgConst.CHARACTER_NAME));
        }
    }
}
