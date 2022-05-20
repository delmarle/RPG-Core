using Station;
using UnityEngine;

public abstract class BaseSystem : MonoBehaviour
{
   private bool _isInitialized = false;
   protected GameInstance _station;

   public void Init(GameInstance station)
   {
      GameGlobalEvents.OnDataBaseLoaded.AddListener(OnDataBaseReady);
      _station = station;
      if (!_isInitialized)
      {
         _isInitialized = true;
         OnInit();
      }
      else
      {
         Debug.LogError( "System Already Initialized : "+GetType().Name);
      }
   }
   
   public void Dispose()
   {
      GameGlobalEvents.OnDataBaseLoaded.RemoveListener(OnDataBaseReady);
      OnDispose();
   }
   
   protected abstract void OnInit();

   protected abstract void OnDispose();

   protected abstract void OnDataBaseReady();

}
