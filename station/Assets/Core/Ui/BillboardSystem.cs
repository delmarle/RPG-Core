using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class BillboardSystem : BaseSystem
    {
        private static Vector2 REFERENCE_RESOLUTION = new Vector2(2048, 1024);
        private static BillboardSystem _instance;
        private readonly List<Transform> _activeBillboards = new List<Transform>();
        private readonly List<Transform> _activeBillboardsToRescale = new List<Transform>();
        private Transform _cameraTransform;
        private Camera _camera;
        private float _scaleFactor;
        
        protected override void OnInit()
        {
            if (_instance == null)
            {
                _instance = this;
                _scaleFactor = Mathf.Pow(2f, Mathf.Lerp(Mathf.Log(Screen.width / REFERENCE_RESOLUTION.x, 2f), Mathf.Log(Screen.height / REFERENCE_RESOLUTION.y, 2f), 0));
            }
        }

        protected override void OnDispose()
        {
            
        }

        protected override void OnDataBaseReady()
        {
            
        }

        private void CacheCamera()
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    _camera = Camera.main;
                    _cameraTransform = Camera.main.transform;
                }
            }
        }

        public void AddBillboard(Transform obj, bool rescale)
        {
            CacheCamera();
            if (_activeBillboards.Contains(obj) == false)
            {
                _activeBillboards.Add(obj);
                UpdateRotation(obj);
            }

            if (_activeBillboardsToRescale.Contains(obj) == false)
            {
                _activeBillboardsToRescale.Add(obj);
                Resize(obj);
            }
        }

        public void RemoveBillboard(Transform obj)
        {
            CacheCamera();
            if (_activeBillboards.Contains(obj))
            {
                _activeBillboards.Remove(obj);
            }
            if (_activeBillboardsToRescale.Contains(obj))
            {
                _activeBillboardsToRescale.Remove(obj);
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    _cameraTransform = Camera.main.transform;
                }
            }
            else
            {
                if (_activeBillboards.Count == 0) return;
                
                foreach (var billboard in _activeBillboards)
                {
                    UpdateRotation(billboard);
                    Resize(billboard);
                }
            }
        }
        
        private void Resize(Transform billBoard)
        {
            if (_camera != null)
            {
                
                transform.rotation = _cameraTransform.rotation;
                float distanceToCam = Vector3.Distance(_cameraTransform.position, billBoard.position);
                float camHeight = 2.0f * distanceToCam * Mathf.Tan(Mathf.Deg2Rad * (_camera.fieldOfView * 0.5f));
                transform.localScale = (camHeight / Screen.width) * Vector3.one * _scaleFactor;
            }

        }

        private void UpdateRotation(Transform billboard)
        {
            var rotation = _cameraTransform.rotation;
            billboard.LookAt(billboard.position + rotation * Vector3.forward, rotation * Vector3.up);
        }

        public static void AddBillBoard(Transform obj, bool resize)
        {
            _instance.AddBillboard(obj, resize);
        }
        
        public static void RemoveBillBoard(Transform obj)
        {
            _instance.RemoveBillboard(obj);
        }
    }
    
}

