using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
  public class HarvestNode : Interactible
  {
    public string NodeId;
    [SerializeField] private TextMeshProUGUI _hintText;

    [SerializeField] private Image _icon;
    //Cache
    private ResourcesNodeDb _nodeDb;

    
    protected override void Setup()
    {
      var _nodeDb = DbSystem.GetDb<ResourcesNodeDb>();
      var model = _nodeDb.GetEntry(NodeId);
      _hintText.text = model.Name.GetValue();
      _icon.sprite = model.Icon;
      name += Config.ShowHintMode.ToString();
    }

    public override void Interact(BaseCharacter user)
    {
      Debug.Log("harvesting");
    }
  }
}

