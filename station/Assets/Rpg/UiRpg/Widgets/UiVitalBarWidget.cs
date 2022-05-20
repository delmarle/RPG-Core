using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiVitalBarWidget : MonoBehaviour
    {
        [SerializeField] private Slider _slider = null;
        
        public void SetVitalValue(int current, int max)
        {
            if (_slider == null) return;
            
            _slider.maxValue = max;
            _slider.value = current;
            
        }

        public void ReceiveDamage(int current)
        {
            if (_slider == null) return;


            _slider.value = current;
        }

        public void ReceiveHeal(int current)
        {
            if (_slider == null) return;


            _slider.value = current;
        }

        public void Setup(VitalModel dataEnergyName)
        {
            _slider.interactable = false;
        }
    } 

}

