using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class BillboardSystem : BaseSystem
    {
        private static BillboardSystem _instance;
        private readonly List<Transform> _activeBillboards = new List<Transform>();
        private Transform _cameraTransform;
        
        protected override void OnInit()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        protected override void OnDispose()
        {
            
        }

        private void CacheCamera()
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    _cameraTransform = Camera.main.transform;
                }
            }
        }

        public void AddBillboard(Transform obj)
        {
            CacheCamera();
            if (_activeBillboards.Contains(obj) == false)
            {
                _activeBillboards.Add(obj);
                UpdateRotation(obj);
            }
        }

        public void RemoveBillboard(Transform obj)
        {
            CacheCamera();
            if (_activeBillboards.Contains(obj))
            {
                _activeBillboards.Remove(obj);
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
                }
            }
        }

        private void UpdateRotation(Transform billboard)
        {
            var rotation = _cameraTransform.rotation;
            billboard.LookAt(billboard.position + rotation * Vector3.forward, rotation * Vector3.up);
        }

        public static void AddBillBoard(Transform obj)
        {
            _instance.AddBillboard(obj);
        }
        
        public static void RemoveBillBoard(Transform obj)
        {
            _instance.RemoveBillboard(obj);
        }
    }
    
}

