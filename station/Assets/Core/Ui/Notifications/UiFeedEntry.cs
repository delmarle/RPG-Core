using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Station
{
    public class UiFeedEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title = null;
        [SerializeField ]private TextMeshProUGUI _message = null;
        
        private void OnEnable()
        {
            transform.SetAsFirstSibling ();
           // Sequence();
        }
        
        
        public void Setup(Dictionary<string, object> data)
        {
            transform.localScale = Vector3.one;
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

