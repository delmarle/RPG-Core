using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
    public class FloatingPopup : PooledItem 
    {
        
        [SerializeField] private Animation _animation = null;
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private FloatingAnimation _animationData = null;
        [SerializeField] private CanvasGroup _canvas;
        private FloatingParams _params;
        private Camera _camera;
        private float _elapsedRatio;
        
      

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

            PoolSystem.Despawn(this, _animationData.Length);
        }

        public void Update()
        {
            Vector3 targetPosition = _params.FollowTarget ? _params.Target.transform.position : _params.Origin;

            _canvas.alpha = _animationData.AlphaCurve.Evaluate(_elapsedRatio);
            transform.localScale = Vector3.one *  _animationData.ScaleCurve.Evaluate(_elapsedRatio);

            var worldPos = targetPosition + _animationData.GetOffSet(_elapsedRatio);
            Vector2 screenPosition = _camera.WorldToScreenPoint(worldPos);
            transform.position = screenPosition;
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
        public Vector3 FromPosition;
        public Vector3 ToPosition;
        public AnimationCurve AlphaCurve;
        public AnimationCurve ScaleCurve;
 


        public void Generate()
        {
            Length = Random.Range(minLength, maxLength);
        }
        
        //GENERATED VALUES
        public float Length;

        public Vector3 GetOffSet(float time)
        {

            return Vector3.Lerp(FromPosition, ToPosition, time);

        }
    }
}

