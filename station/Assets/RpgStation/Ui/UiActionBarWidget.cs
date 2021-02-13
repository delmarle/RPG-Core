using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class UiActionBarWidget : MonoBehaviour
{
    #region FIELDS

    private BaseCharacter _target;
    #endregion
    public void Setup(BaseCharacter target)
    {
        _target = target;
    }

    private void FollowTarget()
    {
        if (_target)
        {
           // _target.Action.OnStartCasting
        }
    }
}
