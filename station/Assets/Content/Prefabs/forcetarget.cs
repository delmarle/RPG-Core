using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class forcetarget : MonoBehaviour
{
    public AiCharacterInput input;
    private void OnEnable()
    {
        input.SetTarget(RpgStation.GetSystemStatic<TeamSystem>().GetCurrentLeader().transform);
    }
}
