using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterEffectIconWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _effectName = null;
        [SerializeField] private TextMeshProUGUI _stackCount = null;
        [SerializeField] private Image _image = null;
        
        public void Setup(RuntimeModifier runtimeModifier)
        {
            //runtimeModifier.Modifier.ModifierType
            if(_effectName) _effectName.text = runtimeModifier.Modifier.EffectName;
            if (_image) _image.sprite = runtimeModifier.Modifier.EffectIcon;
            if(_stackCount) _stackCount.text = runtimeModifier.CurrentStack.ToString();
            //runtimeModifier.TimeLeft
        }
    }
}

