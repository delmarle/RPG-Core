using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
  public class HarvestNode : Interactible
  {
   private List<List<ItemStack>> _resourceStack = new List<List<ItemStack>>();

    //Cache
    public string NodeId;
    private ResourcesNodeDb _nodeDb;
    private ResourceNodeModel _model;
    
    protected override void Setup()
    {
      var _nodeDb = RpgStation.GetDb<ResourcesNodeDb>();
      _model = _nodeDb.GetEntry(NodeId);
      SetUiName(_model.Name.GetValue());
      SetUiIcon( _model.Icon);
name += $" | {_model.Name.GetValue()}";
    }

    public override void Interact(BaseCharacter user)
    {
      user.Action.Interaction.TryInteract(this);
      GenerateResourceStack();
      
      if (_resourceStack.Any())
      {
        var entry = _resourceStack.GetLast();
        _resourceStack.RemoveAt(_resourceStack.Count -1);
        Debug.Log($"harvesting {entry[0].ItemId} count = {entry.Count}");
        if (_resourceStack.Any() == false)
        {
          //destroy
          Destroy(gameObject);
        }
      }
      else
      {
        //destroy
        Destroy(gameObject);
      }
    }

    private void GenerateResourceStack()
    {
      _resourceStack.Clear();
      var possibleLoot = _model.Loots;
      var lootsAmount = _model.CycleLength;
      for (int i = 0; i < lootsAmount; i++)
      {
        var listStack = new List<ItemStack>();
        foreach (var lootEntry in possibleLoot)
        {
          if (lootEntry.Chance > Random.value * 100)
          {
            ItemStack stack = new ItemStack();
            stack.ItemCount = Random.Range(lootEntry.QuantityMin, lootEntry.QuantityMax);
            stack.ItemId = lootEntry.ItemId;
            listStack.Add(stack);
          }
        }
        _resourceStack.Add(listStack);
      }
    }

    public override ActionFxData GetActionData()
    {
      return _model.FxData;
    }
  }
}

