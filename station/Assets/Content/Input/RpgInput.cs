using Station;
using UnityEngine;

public class RpgInput : BaseInput
{
    public static RpgInput Instance;
    private RpgPlayerInput _rpgInput;
    private BaseCharacter _character;
    private static float _lookAngle;
    private static float _tiltAngle;
    private void Awake()
    {
        Instance = this;
        _rpgInput = new RpgPlayerInput();
    }
    
    private void OnDestroy()
    {
        TouchManager.OnTap-= OnTap;
    }

    
    protected override void OnActive(CoreCharacter character)
    {
        _rpgInput.Enable();
        _character = (BaseCharacter)character;
        TouchManager.OnTap-= OnTap;
        TouchManager.OnTap+= OnTap;
    }

    protected override void OnDeactivate(CoreCharacter character)
    { 
        _rpgInput.Disable();
        TouchManager.OnTap-= OnTap;
    }
    
    public override bool Jump()
    {
        return _rpgInput.Character.Jump.WasPressedThisFrame();
    }
    
    public override int MovementType()
    {
        if (_rpgInput.Character.Sprint.IsPressed())
            return 2;
        if (_rpgInput.Character.Sneak.IsPressed())
            return 0;
        return 1; //jogging
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
    #region Targeting

    private void OnTap(Finger tapData)
    {
        if (tapData.StartedOverGui == false)
        {
      
            _character.Target = tapData.PickedGameobject ? tapData.PickedGameobject.GetComponent<BaseCharacter>() : null;
        }
    }

    #endregion
    
    #region INTERACTION

    public bool UsedInteractionPrimary()
    {
        return _rpgInput.Character.InteractionPrimary.IsPressed();
    }
    
    public bool UsedInteractionSecondary()
    {
        return _rpgInput.Character.InteractionSecondary.IsPressed();
    }
    #endregion
}
