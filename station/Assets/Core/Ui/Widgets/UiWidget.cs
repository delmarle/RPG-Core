using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiWidget : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _text = null;
        [SerializeField] protected TextMeshProUGUI _textValue = null;
        [SerializeField] protected Image _image = null;
    
        public void Setup(WidgetData data)
        {
            if(_text)_text.text = data.VisualInfo;
            if(_textValue)_textValue.text = data.VisualValue;
            if (_image)
            {
                _image.sprite = data.Icon;
                _image.color = data.IconColor;
            }
        }
    }

}
