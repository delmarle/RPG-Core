using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
  public class HarvestNode : Interactible
  {
    public string NodeId;

    //Cache
    private ResourcesNodeDb _nodeDb;

    
    protected override void Setup()
    {
      var _nodeDb = DbSystem.GetDb<ResourcesNodeDb>();
      var model = _nodeDb.GetEntry(NodeId);
      SetUiName(model.Name.GetValue());
      SetUiIcon( model.Icon);
name += $" | {model.Name.GetValue()}";
    }

    public override void Interact(BaseCharacter user)
    {
      Debug.Log("harvesting");
    }
  }
}

