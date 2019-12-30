using UnityEngine;

namespace Station
{
  public class Button : Interactible
  {
    
    
    protected override void Setup()
    {
      name = Config.ShowHintMode.ToString();
    }
    

    public override void TryInteract(BaseCharacter user)
    {
      
      Debug.Log("harvesting");
    }
  }
}


