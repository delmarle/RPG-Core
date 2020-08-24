using UnityEngine;

namespace Station
{
    public class UiPlayerInventoryElement : UiPanel
    {
        [SerializeField] private CharacterInventoryTabs _characterInventoryTabs = null;
        private PlayerInventorySystem _inventorySystem;
        private TeamSystem _teamSystem;
        private GenericUiList<string, UiContainerWidget> _containerUiList;
        
        #region [[ INITIALIZATION ]]

        protected override void Awake()
        {
            base.Awake();
            CacheComponents();
        }

        protected override void Start()
        {
            base.Start();
            _characterInventoryTabs.Initialize();
        }


        void CacheComponents()
        {
            _inventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
            _teamSystem = RpgStation.GetSystem<TeamSystem>();
      //      _containerUiList = new GenericUiList<string, UiContainerWidget>(null, null);//new GenericUiList<UiContainerWidget>(null, null);
            
            //prewarm container widget
        }
        #endregion

        public override void Show()
        {
            base.Show();
            _inventorySystem.OnContainerChanged.AddListener(OnContainerChanged);
            _characterInventoryTabs.RegisterAllEvents();
        }
        
        public override void Hide()
        {
            _inventorySystem.OnContainerChanged.RemoveListener(OnContainerChanged);
            _characterInventoryTabs.UnRegisterAllEvents();
            base.Hide();
        }

        public void ClosePanel()
        {
            UiSystem.HidePanel<UiPlayerInventoryElement>(true);
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
