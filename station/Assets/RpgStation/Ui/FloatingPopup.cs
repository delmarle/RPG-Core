using TMPro;
using UnityEngine;

namespace Station
{
    public class FloatingPopup : MonoBehaviour 
    {
        [SerializeField] private Animation _animation = null;
        [SerializeField] private TextMeshProUGUI _damageText = null;

        void OnEnable()
        {
            PoolSystem.Despawn(gameObject, _animation.clip.length);
            
        }

        public void Setup(FloatingParams data)
        {
            if(_damageText) _damageText.text = data.Text;
            _animation.Play();
        }
    }

    public class FloatingParams
    {
        public FloatingParams(string text, Sprite icon = null)
        {
            Text = text;
            Icon = icon;
        }

        public string Text;
        public Sprite Icon;
    }
}

