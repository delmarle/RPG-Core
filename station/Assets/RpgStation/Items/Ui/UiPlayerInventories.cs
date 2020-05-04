using UnityEngine;

namespace Station
{
    public class UiPlayerInventories : MonoBehaviour
    {
        private PlayerInventorySystem _inventorySystem;
        private TeamSystem _teamSystem;
        private GenericUiList<UiContainerWidget> _containerUiList;
        
        //ContainerWidget
        void Init()
        {
            CacheComponents();
        }

        void CacheComponents()
        {
            _inventorySystem = RpgStation.GetSystemStatic<PlayerInventorySystem>();
            _teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            _containerUiList = new GenericUiList<UiContainerWidget>(null, null);
            //_containerUiList.
            //prewarm container widget
        }

        void LoadData()
        {
            var team = _teamSystem.GetTeamMembers();
            
        }
    }

}
