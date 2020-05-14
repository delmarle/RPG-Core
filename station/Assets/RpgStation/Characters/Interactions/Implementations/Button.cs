﻿using UnityEngine;

namespace Station
{
  public class Button : Interactible
  {
    
    
    protected override void Setup()
    {
      name = Config.ShowHintMode.ToString();
    }
    

    public override void Interact(BaseCharacter user)
    {
      Debug.Log("use button");
    }
  }
}


