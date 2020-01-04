using System;
using UnityEngine;

namespace Station
{
  public class CharacterControl : MonoBehaviour
{
  private const float MIN_MOVEMENT = 0.01f;
  [SerializeField] private MovementSettings _movementSettings;
  [SerializeField] private GravitySettings _gravitySettings;
  [SerializeField] private RotationSettings _rotationSettings;
  [SerializeField] private BaseInput _input;
  
  
  // Private fields
  private Vector3 _moveVector;
  private Quaternion _controlRotation;
  private CharacterController _controller;
  private BaseCharacter _baseCharacter;

  private float _maxHorizontalSpeed; // In meters/second
  private float _targetHorizontalSpeed; // In meters/second
  private float _currentHorizontalSpeed; // In meters/second
  private float _currentVerticalSpeed; // In meters/second
  private bool _blockedByAbility;
  private bool _blockedByAction;
  
  #region Unity Methods

  protected virtual void Awake()
  {
    _controller = GetComponent<CharacterController>();
    _baseCharacter  = GetComponent<BaseCharacter>();
  }
  
  private void Start()
  {
    if (_baseCharacter)
    {
      var abilities = _baseCharacter.Action;
      if (abilities != null)
      {
        abilities.OnStartCasting += OnStartCasting;
        abilities.OnCancelCasting += OnCancelCasting;
        abilities.OnCompleteCasting += OnCompleteCasting;
        abilities.OnStartInvoking += OnStartInvoking;
        abilities.OnCompleteInvoking += OnCompleteInvoking;
        abilities.OnStartAction+= OnStartAction;
        abilities.OnFinishAction+= OnFinishAction;
      }
    }
  }



  private void OnDestroy()
  {
    if (_baseCharacter)
    {
      var abilities = _baseCharacter.Action;
      if (abilities != null)
      {
        abilities.OnStartCasting -= OnStartCasting;
        abilities.OnCancelCasting -= OnCancelCasting;
        abilities.OnCompleteCasting -= OnCompleteCasting;
        abilities.OnStartInvoking -= OnStartInvoking;
        abilities.OnCompleteInvoking -= OnCompleteInvoking;
        abilities.OnStartAction -= OnStartAction;
        abilities.OnFinishAction -= OnFinishAction;
      }
    }
  }

  protected virtual void Update()
  {
    if (!_input || _baseCharacter.IsDead) return;
    
    ApplyGravity();
    MoveVector = _blockedByAbility? Vector3.zero : _input.Movement();
    if (_blockedByAction || _blockedByAbility)
    {
      MoveVector = Vector3.zero;
      _controlRotation = Quaternion.identity;
    }
    else
    {
      MoveVector = _input.Movement();
      if (_manualRotation == false)
      {
        if (_input.Rotation() != Quaternion.identity)
        {
          if (_manualRotation == false)
          {
            _controlRotation = _input.Rotation();
          }

         
        }
      }

     
    }

    if (_currentHorizontalSpeed > MIN_MOVEMENT)
    {
      _baseCharacter.Calculator.InvokeMove();
    }

 
    if (IsGrounded) ApplyGravity(true);

    var movementType = _input.MovementType();
    switch (movementType)
    {
       case 0:
         _maxHorizontalSpeed = _movementSettings.WalkSpeed;
         break; 
      case 1:
        _maxHorizontalSpeed = _movementSettings.JogSpeed;
        break; 
      case 2:
        _maxHorizontalSpeed = _movementSettings.SprintSpeed;
        break; 
    }

    if (!_blockedByAbility &&!_blockedByAction && _input.Jump() && IsGrounded ) Jump();
    
    UpdateHorizontalSpeed();
    ApplyMotion();
  }

  #endregion Unity Methods

  #region CALLBACKS

  private void OnStartCasting(CharacterAction data)
  {
    _blockedByAbility = data.CastingData.Option == ExitMode.BlockMovement;
    _currentHorizontalSpeed = 0;
  }

  private void OnCancelCasting(CharacterAction data)
  {
    _blockedByAbility = false;
  }
  
  private void OnCompleteCasting(CharacterAction data)
  {
    _blockedByAbility = false;
  }
  
  private void OnStartInvoking(CharacterAction action)
  {
    if (action.InvokingData == null)
    {
      _blockedByAbility = true;
    }
    else
    {
      _blockedByAbility = action.InvokingData.Option == ExitMode.BlockMovement;
    }

    
  }
  
  private void OnCompleteInvoking(CharacterAction ability)
  {
    _blockedByAbility = false;
  }
  
  private void OnFinishAction()
  {
    _blockedByAction = false;
  }

  private void OnStartAction(CharacterAction obj)
  {
    _blockedByAction = true;
  }

  #endregion

  public bool IsMoving()
  {
    return _currentVerticalSpeed > 0.1f || _currentHorizontalSpeed > 0.1f;
  }

  public void SetInput(BaseInput input)
  {
    if(_input)_input.Desactivate(_baseCharacter);
    
    _input = input;
    
    if(input)_input.Activate(_baseCharacter);
  }

  public Vector3 MoveVector
  {
    get
    {
      if (_baseCharacter.IsDead) return Vector3.zero;
      
      return _moveVector;
    }
    set
    {
      float moveSpeed = value.magnitude * _maxHorizontalSpeed;
      if (moveSpeed < Mathf.Epsilon)
      {
        _targetHorizontalSpeed = 0f;
        return;
      }
      else if (moveSpeed > 0.01f && moveSpeed <= MovementSettings.WalkSpeed)
      {
        _targetHorizontalSpeed = MovementSettings.WalkSpeed;
      }
      else if (moveSpeed > MovementSettings.WalkSpeed && moveSpeed <= MovementSettings.JogSpeed)
      {
        _targetHorizontalSpeed = MovementSettings.JogSpeed;
      }
      else if (moveSpeed > MovementSettings.JogSpeed)
      {
        _targetHorizontalSpeed = MovementSettings.SprintSpeed;
      }

      _moveVector = value;
      if (moveSpeed > 0.01f)
      {
        _moveVector.Normalize();
      }
    }
  }

  public CharacterController Controller
  {
    get { return _controller; }
  }

  public MovementSettings MovementSettings
  {
    get { return _movementSettings; }
    set { _movementSettings = value; }
  }

  public GravitySettings GravitySettings
  {
    get { return _gravitySettings; }
    set { _gravitySettings = value; }
  }

  public RotationSettings RotationSettings
  {
    get { return _rotationSettings; }
    set { _rotationSettings = value; }
  }


  
  public bool IsGrounded
  {
    get { return _controller.isGrounded; }
  }

  public Vector3 Velocity
  {
    get { return _controller.velocity; }
  }

  public Vector3 HorizontalVelocity
  {
    get { return new Vector3(Velocity.x, 0f, Velocity.z); }
  }

  public Vector3 VerticalVelocity
  {
    get { return new Vector3(0f, Velocity.y, 0f); }
  }

  public float HorizontalSpeed
  {
    get { return new Vector3(Velocity.x, 0f, Velocity.z).magnitude; }
  }

  public float VerticalSpeed
  {
    get { return Velocity.y; }
  }

  public void Jump()
  {
    _currentVerticalSpeed = MovementSettings.JumpForce;
  }

  public void ApplyGravity(bool isGrounded = false)
  {
    if (!isGrounded)
    {
      _currentVerticalSpeed =
        MathfExtensions.ApplyGravity(VerticalSpeed, GravitySettings.GravityStrength, GravitySettings.MaxFallSpeed);
    }
    else
    {
      _currentVerticalSpeed = -GravitySettings.GroundedGravityForce;
    }
  }

  public void ResetVerticalSpeed()
  {
    _currentVerticalSpeed = 0f;
  }

  private void UpdateHorizontalSpeed()
  {
    float deltaSpeed = Mathf.Abs(_currentHorizontalSpeed - _targetHorizontalSpeed);
    if (deltaSpeed < 0.1f)
    {
      _currentHorizontalSpeed = _targetHorizontalSpeed;
      return;
    }

    bool shouldAccelerate = (_currentHorizontalSpeed < _targetHorizontalSpeed);
    _currentHorizontalSpeed += MovementSettings.Acceleration *
                               Mathf.Sign(_targetHorizontalSpeed - _currentHorizontalSpeed) * Time.deltaTime;
    if (shouldAccelerate)
    {
      _currentHorizontalSpeed = Mathf.Min(_currentHorizontalSpeed, _targetHorizontalSpeed);
    }
    else
    {
      _currentHorizontalSpeed = Mathf.Max(_currentHorizontalSpeed, _targetHorizontalSpeed);
    }
  }

  private void ApplyMotion()
  {
    OrientRotationToMoveVector(MoveVector);
    Vector3 motion = MoveVector * _currentHorizontalSpeed + Vector3.up * _currentVerticalSpeed;
    _controller.Move(motion * Time.deltaTime);
  }


  #region ROTATION

  private bool _manualRotation = true;
  private void OrientRotationToMoveVector(Vector3 moveVector)
  {
    moveVector.y = 0;
    if (_manualRotation)
    {
      _manualRotation = false;
     
      return;
    }

    if (RotationSettings.FreeMode && moveVector.magnitude > 0f)
    {
      Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
      transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotationSettings.RotationSmoothing * Time.deltaTime);
    }
  }

  private void FaceTarget(Vector3 target)
  {
    Quaternion rotation = Quaternion.LookRotation(target, Vector3.up);
    transform.rotation = rotation;
  }

  public void LerpFaceTarget(Vector3 target)
  {
    var _direction = (target - transform.position).normalized;
    MoveVector = _direction;
    _controlRotation = Quaternion.Euler(_direction);
    var _lookRotation = Quaternion.LookRotation(_direction);
    
    transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, RotationSettings.RotationSmoothing * Time.deltaTime);
    _manualRotation = true;
  }

  public void SetRotation(Vector3 rotation)
  {
    _controlRotation = Quaternion.Euler(0f, rotation.y, 0f);
    transform.rotation = Quaternion.Euler(0f, _controlRotation.eulerAngles.y, 0f);
    _manualRotation = true;
  }

  #endregion
}
  
  #region SETTINGS
  [Serializable]
  public class MovementSettings
  {
    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float _acceleration = 10f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float _walkSpeed = 2f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float _jogSpeed = 4f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float _sprintSpeed = 6f;

    [SerializeField]
    [Tooltip("Force impulse, [0, Infinity)")]
    private float _jumpForce = 10f;
    
    public float Acceleration
    {
      get
      {
        return _acceleration;
      }
      set
      {
        _acceleration = value;
      }
    }

    public float WalkSpeed
    {
      get
      {
        return _walkSpeed;
      }
      set
      {
        _walkSpeed = value;
      }
    }

    public float JogSpeed
    {
      get
      {
        return _jogSpeed;
      }
      set
      {
        _jogSpeed = value;
      }
    }

    public float SprintSpeed
    {
      get
      {
        return _sprintSpeed;
      }
      set
      {
        _sprintSpeed = value;
      }
    }

    public float JumpForce
    {
      get
      {
        return _jumpForce;
      }
      set
      {
        _jumpForce = value;
      }
    }
  }

  [Serializable]
  public class GravitySettings
  {
    [SerializeField]
    [Tooltip("Acceleration, [0, Infinity)")]
    private float gravityStrength = 27f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float maxFallSpeed = 38f;

    [SerializeField]
    [Tooltip("The constant gravity applied to the character when he is grounded, [0, Infinity)")]
    private float groundedGravityForce = 9f;

    public float GravityStrength
    {
      get
      {
        return this.gravityStrength;
      }
      set
      {
        this.gravityStrength = value;
      }
    }

    public float MaxFallSpeed
    {
      get
      {
        return this.maxFallSpeed;
      }
      set
      {
        this.maxFallSpeed = value;
      }
    }

    public float GroundedGravityForce
    {
      get
      {
        return groundedGravityForce;
      }
      set
      {
        this.groundedGravityForce = value;
      }
    }
  }

  [Serializable]
  public class RotationSettings
  {
    [SerializeField]
    [Range(0,25)]
    private float _rotationSmoothing = 15f;

    [SerializeField]
    private bool _freeMode = true;

    /// <summary>
    /// If set to true, this automatically sets UseControlRotation to false
    /// </summary>
    public bool FreeMode
    {
      get
      {
        return _freeMode;
      }
      set
      {
        _freeMode = value;
      }
    }

    public float RotationSmoothing
    {
      get { return _rotationSmoothing; }
    }
  }

  #endregion
}

