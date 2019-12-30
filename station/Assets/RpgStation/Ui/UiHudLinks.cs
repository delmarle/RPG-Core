using UnityEngine;

namespace Station
{
    public class UiHudLinks : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button _playerProfileBtn = null;

        private void Awake()
        {
            _playerProfileBtn.onClick.AddListener(OnClickProfileBtn);
        }
        
        private void OnDestroy()
        {
            _playerProfileBtn.onClick.RemoveListener(OnClickProfileBtn);
        }

        private void OnClickProfileBtn()
        {
            PanelSystem.OpenPanel<UiPlayerProfilePanel>();
        }
    }
}

