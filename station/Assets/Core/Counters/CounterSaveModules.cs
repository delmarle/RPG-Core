using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class CounterSaveModules : SaveModule<CounterData>
{

    public override void FetchData()
    {
        var counterSystem = GameInstance.GetSystem<CounterSystem>();
        Value = counterSystem.GetData;
    }
}

[Serializable]
public class CounterData
{
    public Dictionary<string, int> Counters = new Dictionary<string, int>();
    public Dictionary<string, Dictionary<string,int>> CountersWithIds = new Dictionary<string, Dictionary<string, int>>();

}
