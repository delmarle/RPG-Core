using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
  public class HarvestNode : Interactible
  {
   

    //Cache
    public string NodeId;
    private ResourcesNodeDb _nodeDb;
    private ResourceNodeModel _model;
    
    protected override void Setup()
    {
      var _nodeDb = RpgStation.GetDb<ResourcesNodeDb>();
      ResourceNodeModel model = _nodeDb.GetEntry(NodeId);
      SetUiName(model.Name.GetValue());
      SetUiIcon( model.Icon);
name += $" | {model.Name.GetValue()}";
    }

    public override void Interact(BaseCharacter user)
    {
      user.Action.Interaction.TryInteract(this);
      Debug.Log("harvesting");
    }
    
    
  }
}

