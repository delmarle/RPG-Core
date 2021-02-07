using System;
using System.Reflection;
using UnityEngine;

namespace Station
{
    public class UiHudLinks : MonoBehaviour
    {
        [HideInInspector]public string PopupType;
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
            Assembly asm = typeof(UiPanel).Assembly;
            Type source = asm.GetType(PopupType);
            UiSystem.OpenPanel(source);
        }
    }
}

