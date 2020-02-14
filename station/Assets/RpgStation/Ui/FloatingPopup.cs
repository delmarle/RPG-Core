using TMPro;
using UnityEngine;

namespace Station
{
    public class FloatingPopup : PooledItem 
    {
        [SerializeField] private Animation _animation = null;
        [SerializeField] private TextMeshProUGUI _text = null;
        private FloatingParams _params;
        private Camera _camera;
        private Vector3 _offset;
        void OnEnable()
        {
            PoolSystem.Despawn(this, _animation.clip.length);
        }

        public void Setup(FloatingParams data)
        {
            _camera = Camera.main;
            _params = data;
            if(_text) _text.text = data.Text;
            _animation.Play();
            _offset = Random.insideUnitSphere;
        }

        public void Update()
        {
            if (_params.FollowTarget)
            {
                //follow target
                Vector2 screenPosition = _camera.WorldToScreenPoint(_params.Target.transform.position+_offset);
                transform.position = screenPosition;
            }
            else
            {
                //stick to origin
                Vector2 screenPosition = _camera.WorldToScreenPoint(_params.Origin+_offset);
                transform.position = screenPosition;
            }
        }
    }

    public class FloatingParams
    {
        public FloatingParams(string text,  FloatingPopupAnchor target, Sprite icon = null)
        {
            Text = text;
            Icon = icon;
            Target = target;
            Origin = target.transform.position;
        }

        public string Text;
        public Sprite Icon;
        public FloatingPopupAnchor Target;
        public bool FollowTarget;
        public Vector3 Origin;

    }
}

