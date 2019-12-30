using UnityEngine;

namespace Station
{
    public class CameraController : MonoBehaviour
{
  #region [[ FIELDS ]]
  [SerializeField] private Transform _target; // The target to follow

  [SerializeField] [Range(0, 1)] private float _catchSpeedDamp = 0;

  [SerializeField] [Range(0, 30)] [Tooltip("How fast the camera rotates around the pivot")]
  private float _rotationSmoothing = 15.0f;

  // private fields
  private PlayerInput _input;
  private Transform _rig; // The root transform of the camera rig
  private Transform _pivot; // The point at which the camera pivots around
  private Quaternion _pivotTargetLocalRotation; // Controls the X Rotation (Tilt Rotation)
  private Quaternion _rigTargetLocalRotation; // Controlls the Y Rotation (Look Rotation)
  private Vector3 _cameraVelocity; // The velocity at which the camera moves
#endregion

  private void Start()
  {
    _input = PlayerInput.Instance;
    _pivot = transform.parent;
    _rig = _pivot.parent;

    transform.localRotation = Quaternion.identity;
  }

  protected virtual void Update()
  {
    if (_input == null) return;
    var controlRotation = _input.Rotation();
    UpdateRotation(controlRotation);
  }

  protected virtual void LateUpdate()
  {
    FollowTarget();
  }


  private void FollowTarget()
  {
    if (_target == null)
    {
      return;
    }

    _rig.position = Vector3.SmoothDamp(_rig.position, _target.transform.position, ref _cameraVelocity, _catchSpeedDamp);
  }

  private void UpdateRotation(Quaternion controlRotation)
  {
    if (_target != null)
    {
      // Y Rotation (Look Rotation)
      _rigTargetLocalRotation = Quaternion.Euler(0f, controlRotation.eulerAngles.y, 0f);

      // X Rotation (Tilt Rotation)
      _pivotTargetLocalRotation = Quaternion.Euler(controlRotation.eulerAngles.x, 0f, 0f);

      if (_rotationSmoothing > 0.0f)
      {
        _pivot.localRotation =
          Quaternion.Slerp(_pivot.localRotation, _pivotTargetLocalRotation, _rotationSmoothing * Time.deltaTime);

        _rig.localRotation =
          Quaternion.Slerp(_rig.localRotation, _rigTargetLocalRotation, _rotationSmoothing * Time.deltaTime);
      }
      else
      {
        _pivot.localRotation = _pivotTargetLocalRotation;
        _rig.localRotation = _rigTargetLocalRotation;
      }
    }
  }

  public void SetTarget(Transform target)
  {
    _target = target;
  }
}
}

