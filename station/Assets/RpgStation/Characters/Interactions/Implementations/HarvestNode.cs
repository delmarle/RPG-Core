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
        if (CanAddStack(entry, user))
        {
          _resourceStack.RemoveAt(_resourceStack.Count -1);
          Debug.Log($"harvesting {entry[0].ItemId} count = {entry.Count}");
          CollectOnce(entry, user);
          if (_resourceStack.Any() == false)
          {
            //destroy
            DeSpawn();
          }
        }
        else
        {

          if (Config.FailNotificationChannels.Any())
          {
            var dict = new Dictionary<string, object> {{UiConstants.TEXT_MESSAGE, $"Your inventory is too full to collect the item {GetObjectName()}"}};
            UiNotificationSystem.ShowNotification(Config.FailNotificationChannels, dict);
          }
        }
       
      }
      else
      {
        //destroy
        DeSpawn();
      }
    }


    private bool CanAddStack(List<ItemStack> listStack, BaseCharacter receiver)
    {
      var playerInventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
      var playerContainer = playerInventorySystem.GetContainer(receiver.GetCharacterId());
      foreach (var stack in listStack)
      {
        if (playerContainer.CanAddItem(stack) == false)
        {
          return false;
        }
      }
      return true;
    }
    private void CollectOnce(List<ItemStack> listStack, BaseCharacter receiver)
    {
      var playerInventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
      var playerContainer = playerInventorySystem.GetContainer(receiver.GetCharacterId());
      foreach (var stack in listStack)
      {
        if (Config.ResultNotificationChannels.Any())
        {
          var dict = new Dictionary<string, object> {{UiConstants.ITEM_STACK, stack}};
          UiNotificationSystem.ShowNotification(Config.ResultNotificationChannels, dict);
        }
        playerContainer.AddItem(stack);
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
            ItemStack stack = new ItemStack
            {
              ItemCount = Random.Range(lootEntry.QuantityMin, lootEntry.QuantityMax), 
              ItemId = lootEntry.ItemId
            };
            listStack.Add(stack);
          }
        }
        _resourceStack.Add(listStack);
      }
    }

    private void DeSpawn()
    {
      Destroy(gameObject);
    }

    public override ActionFxData GetActionData()
    {
      return _model.FxData;
    }
  }
}

