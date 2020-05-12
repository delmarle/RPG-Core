using UnityEngine;

namespace Station
{
    public class UiContainerPopup : UiPopup
    {
        public const string POPUP_KEY = "container_popup";
        [SerializeField] private UiContainerWidget _containerWidget;
        private ContainerReference _containerReference;
        private BaseCharacter _user;
        


        public void Setup(ContainerReference containerReference, BaseCharacter user)
        {
            _user = user;
            _containerReference = containerReference;
            _containerWidget.Init(containerReference);
        }
        public override void Show()
        {
            base.Show();
            _containerWidget.RegisterEvents();
            _containerWidget.UpdateUiSlots();
        }
        public override void Hide()
        {
            _containerWidget.UnregisterEvents();
            base.Hide();
        }

        public void OnClickCollectSlots()
        {
            var playerInventorySystem = RpgStation.GetSystemStatic<PlayerInventorySystem>();
            var playerContainer = playerInventorySystem.GetContainer(_user.GetCharacterId());
            playerContainer.TryAddAllItemsFromContainer(_containerReference.GetContainer());
        }
        
    }
}

