using UnityEngine;

namespace Station
{
  public class Portal : Interactible
  {
    [SerializeField] private bool _saveDestination = true;
    [Destination] public DestinationModel Destination;
    private SceneSystem _sceneSystem;
    private SavingSystem _saveSystem;
    private ScenesDb _sceneDb;
    
    protected override void Setup()
    {
      
      _sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
      _saveSystem =  RpgStation.GetSystemStatic<SavingSystem>();
      var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
      if (dbSystem == null)
      {
        return;
      }

      _sceneDb = dbSystem.GetDb<ScenesDb>();
      name += Config.ShowHintMode.ToString();
    }

    public override void TryInteract(BaseCharacter user)
    {
      GameGlobalEvents.OnBeforeLeaveScene?.Invoke();
      var sceneData = _sceneDb.GetEntry(Destination.SceneId);
      var model = new TravelModel {SceneName = sceneData.VisualName};
      
      GameGlobalEvents.OnTriggerSceneSave?.Invoke();
      _sceneSystem.InjectDestinationInSave(Destination);
 
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

    public override bool CanUse()
    {
      if (_sceneSystem.IsTraveling)
      {
        return false;
      }

      return base.CanUse();
    }
    
  }
}

