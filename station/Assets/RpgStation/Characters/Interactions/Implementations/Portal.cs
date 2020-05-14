
using UnityEngine.Serialization;

namespace Station
{
  public class Portal : Interactible
  {
    [FormerlySerializedAs("scene")] [FormerlySerializedAs("Destination")] [Destination] public DestinationModel destination;
    private SceneSystem _sceneSystem;
    private ScenesDb _sceneDb;
    
    protected override void Setup()
    {
      
      _sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
      var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
      if (dbSystem == null)
      {
        return;
      }

      _sceneDb = dbSystem.GetDb<ScenesDb>();
      name += Config.ShowHintMode.ToString();
    }

    public override void Interact(BaseCharacter user)
    {
      GameGlobalEvents.OnBeforeLeaveScene?.Invoke();
      var sceneData = _sceneDb.GetEntry(destination.SceneId);
      var model = new TravelModel {SceneName = sceneData.VisualName};
      _sceneSystem.InjectDestinationInSave(destination);
 
      _sceneSystem.TravelToZone(model);
     
      switch (Config.TryInteractMode)
      {
        case InteractType.None:
          break;
        case InteractType.Tap:
          break;
        case InteractType.EnterDistance:
          break;
        case InteractType.Collide:
          break;
        case InteractType.HoverAndInput:
          break;
        case InteractType.UiInput:
          break;
      }
    }

    public override bool CanUse(BaseCharacter character)
    {
      if (_sceneSystem.IsTraveling)
      {
        return false;
      }

      return base.CanUse(character);
    }
    
  }
}

