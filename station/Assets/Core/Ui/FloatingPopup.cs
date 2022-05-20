using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
    public class FloatingPopup : PooledItem 
    {
        
       // [SerializeField] private Animation _animation = null;
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private FloatingAnimation _animationData = null;
        [SerializeField] private CanvasGroup _canvas = null;
        private FloatingParams _params;
        private Camera _camera;
        private float _elapsedRatio;
        private FloatingPopupSystem _system;
        
      

        public void Setup(FloatingParams data)
        {
            _elapsedRatio = 0;
            _camera = Camera.main;
            _animationData.Generate();
            _params = data;
            Update();
            if (_text)
            {
                _text.text = data.Text;
                _text.color =  _animationData.Gradient.Evaluate(0);
            }

            if (_system == null)
            {
                _system = GameInstance.GetSystem<FloatingPopupSystem>();
            }
            _system.DeSpawnFloatingPopup(this, _animationData.Length);
        }

        public void Update()
        {
            if (_camera == null) return;
            
            Vector3 targetPosition = _params.FollowTarget ? _params.Target.transform.position : _params.Origin;

            _canvas.alpha = _animationData.AlphaCurve.Evaluate(_elapsedRatio);
            transform.localScale = Vector3.one *  _animationData.ScaleCurve.Evaluate(_elapsedRatio);
            
            Vector2 screenPosition = _camera.WorldToScreenPoint(targetPosition);
            transform.position = screenPosition + _animationData.GetOffSet(_elapsedRatio);
            _text.color =  _animationData.Gradient.Evaluate(_elapsedRatio);
           
            _elapsedRatio += Time.deltaTime / _animationData.Length;
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

    [Serializable]
    public class FloatingAnimation
    {
        public float minLength = 1;
        public float maxLength = 2;
        
        //position
        public Gradient Gradient;
        public Vector2 StartOffset;
        public Vector2 EndOffset;
        public AnimationCurve AlphaCurve;
        public AnimationCurve ScaleCurve;
        

        public void Generate()
        {
            _length = Random.Range(minLength, maxLength);
        }
        
        //GENERATED VALUES
        private float _length;

        public float Length => _length;

        public Vector2 GetOffSet(float time)
        {
            return Vector2.Lerp(StartOffset, EndOffset, time);
        }
    }
}

