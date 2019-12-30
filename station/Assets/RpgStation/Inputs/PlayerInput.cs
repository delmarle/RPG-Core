
using Station;
using UnityEngine;

public class PlayerInput : BaseInput
{
  #region [[ SINGLETON ]]

  private BaseCharacter _character;
  public static PlayerInput Instance;

  private void Awake()
  {
    Instance = this;
  }

  private void OnDestroy()
  {
    TouchManager.OnTap-= OnTap;
  }

  #endregion

  private static float _lookAngle;
  private static float _tiltAngle;

  protected override void OnActive(BaseCharacter character)
  {
    _character = character;
    //TouchManager.OnTap-= OnTap;
    TouchManager.OnTap+= OnTap;
  }

  protected override void OnDeactivate(BaseCharacter character)
  { 
    TouchManager.OnTap-= OnTap;
  }

  public override Vector3 Movement()
  {
    Vector3 moveVector;
    float horizontalAxis = Input.GetAxis("Horizontal");
    float verticalAxis = Input.GetAxis("Vertical");

    if (Camera.main != null)
    {
      // Calculate the move vector relative to camera rotation
      Vector3 scalerVector = new Vector3(1f, 0f, 1f);
      Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, scalerVector).normalized;
      Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, scalerVector).normalized;

      moveVector = (cameraForward * verticalAxis + cameraRight * horizontalAxis);
    }
    else
    {
      // Use world relative directions
      moveVector = (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
    }

    if (moveVector.magnitude > 1f)
    {
      moveVector.Normalize();
    }

    return moveVector;
  }

  public override Quaternion Rotation()
  {
    float mouseSensitivity = 3f;
    float minTiltAngle = -75f;
    float maxTiltAngle = 45f;
    bool pressedInput = Input.GetMouseButton(1);

    float mouseX = pressedInput ? Input.GetAxis("Mouse X") : 0;
    float mouseY = pressedInput ? Input.GetAxis("Mouse Y") : 0;

    // Adjust the look angle (Y Rotation)
    _lookAngle += mouseX * mouseSensitivity;
    _lookAngle %= 360f;

    // Adjust the tilt angle (X Rotation)
    _tiltAngle += mouseY * mouseSensitivity;
    _tiltAngle %= 360f;
    _tiltAngle = MathfExtensions.ClampAngle(_tiltAngle, minTiltAngle, maxTiltAngle);

    var controlRotation = Quaternion.Euler(-_tiltAngle, _lookAngle, 0f);
    return controlRotation;
  }

  public override int MovementType()
  {
    if (Input.GetKey(KeyCode.LeftShift))
      return 2;
    if (Input.GetKey(KeyCode.C))
      return 0;
    return 1; //jogging
  }

  public override bool Jump()
  {
    return Input.GetButton("Jump");
  }
  
  #region Targeting

  private void OnTap(Finger tapData)
  {
    if (tapData.StartedOverGui == false)
    {
      
      _character.Target = tapData.PickedGameobject ? tapData.PickedGameobject.GetComponent<BaseCharacter>() : null;
    }
  }

  #endregion
}
