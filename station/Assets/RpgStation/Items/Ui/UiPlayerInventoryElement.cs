using UnityEngine;

namespace Station
{
    public class UiPlayerInventoryElement : UiPanel
    {
        private PlayerInventorySystem _inventorySystem;
        private TeamSystem _teamSystem;
        private GenericUiList<string, UiContainerWidget> _containerUiList;
        
        #region [[ INITIALIZATION ]]

        protected override void Awake()
        {
            base.Awake();
            CacheComponents();
        }
        
        void CacheComponents()
        {
            _inventorySystem = RpgStation.GetSystemStatic<PlayerInventorySystem>();
            _teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            _containerUiList = new GenericUiList<string, UiContainerWidget>(null, null);//new GenericUiList<UiContainerWidget>(null, null);
            
            //prewarm container widget
        }
        #endregion

        public override void Show()
        {
            base.Show();
            _inventorySystem.OnContainerChanged.AddListener(OnContainerChanged);
        }
        
        public override void Hide()
        {
            _inventorySystem.OnContainerChanged.RemoveListener(OnContainerChanged);
            base.Hide();
        }

        private void OnContainerChanged(string containerId)
        {
            
        }

    

        void LoadData()
        {
            var team = _teamSystem.GetTeamMembers();
            
        }
    }

}
