using System.Collections;
using System.Collections.Generic;
using Station;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class UiButton : MonoBehaviour
{
  
   #region [[ FIELDS ]]

   private const string STATE_SELECTED = "selected";
   private const string STATE_UNSELECTED = "unselected";
   private const string STATE_LOCKED = "locked";
   private const string STATE_UNLOCKED = "unlocked";
    [SerializeField] private BaseAnimation _animation;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _secondText;
    [SerializeField] private TextMeshProUGUI _thirdText;
    private string _key;
    private int _index;



    private StationAction _mainCallback;
    private StationAction<int> _mainCallbackIndex;

    private StationAction _secondCallback;
    private StationAction<int> _secondCallbackIndex;
    #endregion

  #region SET VALUES

  public void SetIndex(int index)
  {
    _index = index;
  }
  public void SetName(string value, bool localized = false)
  {
    if (_name == null) return;
    if (localized)
    {
      
    }
    else
    {
      _name.text = value;
    }
  }
  
  public void SetSecondText(string value, bool localized = false)
  {
    if (_secondText == null) return;
    if (localized)
    {
      
    }
    else
    {
      _secondText.text = value;
    }
  }
  
  public void SetThirdText(string value, bool localized = false)
  {
    if (_thirdText == null) return;
    
    if (localized)
    {
      
    }
    else
    {
      _thirdText.text = value;
    }
  }

  public void SetIcon(Sprite sprite)
  {
    if (_icon)
    {
      _icon.sprite = sprite;
    }
  }

  public void SetCallBack(StationAction callback)
  {
    _mainCallback = callback;
  }
  
  public void SetCallBack(StationAction<int> callback)
  {
    _mainCallbackIndex = callback;
  }
  
  public void SetSecondCallBack(StationAction callback)
  {
    _secondCallback = callback;
  }
  
  public void SetSecondCallBack(StationAction<int> callback)
  {
    _secondCallbackIndex = callback;
  }
    #endregion
    public void PressButton()
    {
      _mainCallback?.Invoke();
      _mainCallbackIndex?.Invoke(_index);
    }

    public void PressSecondButton()
    {
      _secondCallback?.Invoke();
      _secondCallbackIndex?.Invoke(_index);
    }


    public void SetLocked(bool locked)
    {
      if (locked)
      {
        _animation.PlayState(STATE_LOCKED);
      }
      else
      {
        _animation.PlayState(STATE_UNLOCKED);
      }
      
      _button.interactable = !locked;
    }

    public void SetStateSelected()
    {
      _animation.PlayState(STATE_SELECTED);
    }
    
    public void SetStateUnSelected()
    {
      _animation.PlayState(STATE_UNSELECTED);
    }
    
    public void SetState(string state)
    {
      _animation.PlayState(state);
    }
}
