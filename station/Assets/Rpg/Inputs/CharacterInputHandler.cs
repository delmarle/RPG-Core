using UnityEngine;

namespace Station
{
  public class CharacterInputHandler : MonoBehaviour
  {
    #region [[ FIELDS ]]
    [SerializeField] private CharacterControl characterControl = null;
    [SerializeField] private AiCharacterInput _aiInput = null;
  
    private PlayerInput _playerInput;
    //private BaseInput _previousInput;
    public bool UseAi = false;
    #endregion
  
    public void InitializePlayerInput(PlayerInput player)
    {
      var bc = characterControl.GetComponent<BaseCharacter>();
      if (_aiInput)
      {
        _aiInput.Desactivate(bc);
      }

      _playerInput = player;
      _playerInput.Desactivate(bc);
      
    }

    public void SetAiInput(Transform target)
    {
      _aiInput.SetTarget(target);
      SwitchInput(_aiInput);
      UseAi = true;
    }
  
    public void SetPlayerInput()
    {
      SwitchInput(_playerInput);
      UseAi = false;
    }
  
    private void SwitchInput(BaseInput input)
    {
      characterControl.SetInput(input);
    }
  
  }
}

