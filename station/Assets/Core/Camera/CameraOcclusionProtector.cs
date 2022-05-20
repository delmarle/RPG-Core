using UnityEngine;

namespace Station
{
    public class CameraOcclusionProtector : MonoBehaviour
{
    private struct ClipPlaneCornerPoints
    {
        public Vector3 UpperLeft { get; set; }

        public Vector3 UpperRight { get; set; }

        public Vector3 LowerLeft { get; set; }

        public Vector3 LowerRight { get; set; }
    }

    // Serializable fields
    [SerializeField]
    [Range(1, 15)]
    private float _distanceToTarget = 2.5f; // In meters

    [SerializeField]
    [Range(1, 2)]
    private float _nearClipPlaneExtentMultiplier = 1.2f;

    [SerializeField]
    [Range(0, 1)]
    private float _occlusionMoveTime = 0.025f; // The lesser, the better

    [SerializeField]
    private LayerMask _ignoreLayerMask = 0; // What objects should the camera ignore when checked for clips and occlusions

#if UNITY_EDITOR
    [SerializeField]
    private bool _visualizeInScene = true;
#endif

    // Private fields
    private Camera _camera;
    private Transform _pivot;
    private Vector3 _cameraVelocity;
    private float _nearClipPlaneHalfHeight;
    private float _nearClipPlaneHalfWidth;
    private float _sphereCastRadius;

    protected virtual void Awake()
    {
        _camera = GetComponent<Camera>();
        _pivot = transform.parent;

        float halfFov = (_camera.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
        _nearClipPlaneHalfHeight = Mathf.Tan(halfFov) * _camera.nearClipPlane * _nearClipPlaneExtentMultiplier;
        _nearClipPlaneHalfWidth = _nearClipPlaneHalfHeight * _camera.aspect;
        _sphereCastRadius = new Vector2(_nearClipPlaneHalfWidth, _nearClipPlaneHalfHeight).magnitude; // Pythagoras
    }

    protected virtual void LateUpdate()
    {
        UpdateCameraPosition();

#if UNITY_EDITOR
        DrawDebugVisualization();
#endif
    }

#if UNITY_EDITOR
    private void DrawDebugVisualization()
    {
        if (_visualizeInScene)
        {
            ClipPlaneCornerPoints nearClipPlaneCornerPoints = GetNearClipPlaneCornerPoints(transform.position);

            Debug.DrawLine(_pivot.position, transform.position - transform.forward * _camera.nearClipPlane, Color.red);
            Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.UpperLeft, Color.green);
            Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperLeft, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperRight, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerRight, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerLeft, nearClipPlaneCornerPoints.UpperLeft, Color.green);
        }
    }
#endif

    private bool IsCameraOccluded(Vector3 cameraPosition, ref float outDistanceToTarget)
    {
        // Cast a sphere along a ray to see if the camera is occluded
        Ray ray = new Ray(_pivot.transform.position, -transform.forward);
        float rayLength = _distanceToTarget - _camera.nearClipPlane;
        RaycastHit hit;

        if (Physics.SphereCast(ray, _sphereCastRadius, out hit, rayLength, ~_ignoreLayerMask))
        {
            outDistanceToTarget = hit.distance + _sphereCastRadius;
            return true;
        }
        else
        {
            outDistanceToTarget = -1f;
            return false;
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 newCameraLocalPosition = transform.localPosition;
        newCameraLocalPosition.z = -_distanceToTarget;
        Vector3 newCameraPosition = _pivot.TransformPoint(newCameraLocalPosition);
        float newDistanceToTarget = _distanceToTarget;
        
        if (IsCameraOccluded(newCameraPosition, ref newDistanceToTarget))
        {
            newCameraLocalPosition.z = -newDistanceToTarget;
            _pivot.TransformPoint(newCameraLocalPosition);
        }
        
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition, newCameraLocalPosition, ref _cameraVelocity, _occlusionMoveTime);
    }

    private ClipPlaneCornerPoints GetNearClipPlaneCornerPoints(Vector3 cameraPosition)
    {
        ClipPlaneCornerPoints nearClipPlanePoints = new ClipPlaneCornerPoints();

        var transform1 = transform;
        var right = transform1.right;
        nearClipPlanePoints.UpperLeft = cameraPosition - right * _nearClipPlaneHalfWidth;
        var up = transform1.up;
        nearClipPlanePoints.UpperLeft += up * _nearClipPlaneHalfHeight;
        var forward = transform1.forward;
        var nearClipPlane = _camera.nearClipPlane;
        nearClipPlanePoints.UpperLeft += forward * nearClipPlane;

        nearClipPlanePoints.UpperRight = cameraPosition + right * _nearClipPlaneHalfWidth;
        nearClipPlanePoints.UpperRight += up * _nearClipPlaneHalfHeight;
        nearClipPlanePoints.UpperRight += forward * nearClipPlane;

        nearClipPlanePoints.LowerLeft = cameraPosition - right * _nearClipPlaneHalfWidth;
        nearClipPlanePoints.LowerLeft -= up * _nearClipPlaneHalfHeight;
        nearClipPlanePoints.LowerLeft += forward * nearClipPlane;

        nearClipPlanePoints.LowerRight = cameraPosition + right * _nearClipPlaneHalfWidth;
        nearClipPlanePoints.LowerRight -= up * _nearClipPlaneHalfHeight;
        nearClipPlanePoints.LowerRight += forward * nearClipPlane;

        return nearClipPlanePoints;
    }

    //private void OnDrawGizmos()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(this.pivot.transform.position - (this.transform.forward * (this.distanceToTarget - this.camera.nearClipPlane)), this.sphereCastRadius);
    //    }
    //}
}
}

