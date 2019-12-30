using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private TextMeshProUGUI _textValue = null;
        [SerializeField] private Image _image = null;
    
        public void Setup(WidgetData data)
        {
            _text.text = data.VisualInfo;
            _textValue.text = data.VisualValue;
            _image.sprite = data.Icon;
            _image.color = data.IconColor;
        }
    }

}
