using UnityEngine;
using TMPro;

namespace Station
{
  public class HarvestNode : Interactible
  {
    [SerializeField] private CastingData _casting = null;
    [SerializeField] private TextMeshPro _text = null;
    public object Destination;
    protected override void Setup()
    {
      name += Config.ShowHintMode.ToString();
      _text.gameObject.SetActive(false);
    }

    public override CastingData GetCastingData()
    {
      return _casting;
    }

    public override void ShowVisual()
    {
      _text.gameObject.SetActive(true);
    }

    public override void HideVisual()
    {
      _text.gameObject.SetActive(false);
    }

    public override void Interact(BaseCharacter user)
    {
      
      Debug.Log("harvesting");
    }
  }
}

