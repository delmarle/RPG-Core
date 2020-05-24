using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Station
{
    public class UiFeedEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField ]private TextMeshProUGUI _message;
        
        private void OnEnable()
        {
            transform.SetAsFirstSibling ();
           // Sequence();
        }
        
        
        public void Setup(Dictionary<string, object> data)
        {
            if (data.ContainsKey(UiConstants.TEXT_TITLE) && _title)
            {
                _title.text = (string)data[UiConstants.TEXT_TITLE];
            }
            
            if (data.ContainsKey(UiConstants.TEXT_MESSAGE) && _message)
            {
                _message.text = (string)data[UiConstants.TEXT_MESSAGE];
            }
        }
    }
}

