using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class EquipmentHandler : MonoBehaviour
{
    #region FIELDS

    public StationAction OnEquipmentChanged;
    private Dictionary<string, float> _statsBonusesMap = new Dictionary<string, float>();
    private Dictionary<string, int> _attributesBonusesMap = new Dictionary<string, int>();
    private Dictionary<string, int> _vitalsBonusesMap = new Dictionary<string, int>();
    
    private BaseItemContainer _equipmentContainer;

    private BaseCharacter _owner;
    #endregion

    public void Setup(BaseCharacter source)
    {
        _owner = source;
      
        var playerInventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
        var id = PlayerInventorySystem.PLAYER_EQUIPMENT_KEY + source.GetCharacterId();
        var container = playerInventorySystem.GetContainer(id);
        if (container != null)
        {
            _equipmentContainer = container;
            _equipmentContainer.OnContentChanged -= OnContentChanged;
            _equipmentContainer.OnContentChanged += OnContentChanged;
        }
    }

    void OnDestroy()
    {
        if (_equipmentContainer != null)
        {
            _equipmentContainer.OnContentChanged -= OnContentChanged;
        }
        
    }

    private void OnContentChanged()
    {
        ResetBonus();
        CacheBonus();
        OnEquipmentChanged?.Invoke();
        _owner.OnAttributesUpdated?.Invoke(_owner);
        _owner.OnStatisticUpdated?.Invoke(_owner);
        _owner.OnVitalsUpdated?.Invoke(_owner);
    }

    private void ResetBonus()
    {
        _statsBonusesMap.Clear();
        _attributesBonusesMap.Clear();
        _vitalsBonusesMap.Clear();
    }
    private void CacheBonus()
    {
        var itemDb = GameInstance.GetDb<ItemsDb>();
        var items = _equipmentContainer.GetItems();
        foreach (var itemPair in items)
        {
            var item = itemDb.GetEntry(itemPair.Value);
            if (item != null)
            {
                if (item is EquipmentItemModel == false)
                {
                    Debug.LogError($"{item.GetType()} / {item}");
                }
                var equipItem = (EquipmentItemModel)item;

                foreach (var attributesBonus in equipItem.AttributesBonuses)
                {
                    if (_attributesBonusesMap.ContainsKey(attributesBonus.Id))
                    {
                        _attributesBonusesMap[attributesBonus.Id] += attributesBonus.Value;
                    }
                    else
                    {
                        _attributesBonusesMap.Add(attributesBonus.Id, attributesBonus.Value);
                    }
                }
            
                foreach (var statsBonus in equipItem.StatisticsBonuses)
                {
                    if (_statsBonusesMap.ContainsKey(statsBonus.Id))
                    {
                        _statsBonusesMap[statsBonus.Id] += statsBonus.Value;
                    }
                    else
                    {
                        _statsBonusesMap.Add(statsBonus.Id, statsBonus.Value);
                    }
                }
            
                foreach (var vitalBonus in equipItem.VitalBonuses)
                {
                    if (_vitalsBonusesMap.ContainsKey(vitalBonus.Id))
                    {
                        _vitalsBonusesMap[vitalBonus.Id] += vitalBonus.Value;
                    }
                    else
                    {
                        _vitalsBonusesMap.Add(vitalBonus.Id, vitalBonus.Value);
                    }
                }
            }
           
        }
    }

    private void UpdateContent()
    {
        
    }
    #region GET STATS

    public int GetVitalBonus(string vitalId)
    {
        return 0;
    }

    public int GetStatsBonus(string vitalId)
    {
        return 0;
    }

    public int GetAttributeBonus(string vitalId)
    {
        return 0;
    }

    
    #endregion
}
