using Station.Data;

namespace Station
{
    
    public static partial class GameGlobalEvents 
    {
        /// <summary>
        /// the game is booting and all database are accessible now
        /// </summary>
        public static readonly StationEvent OnDataBaseLoaded = new StationEvent();
        
        /// <summary>
        /// trigger after character creation or when entering first zone
        /// </summary>
        public static readonly StationEvent OnEnterGame = new StationEvent();

        /// <summary>
        /// trigger when changing scene, the previous scene will be removed next
        /// </summary>
        public static readonly StationEvent<SceneType> OnSceneStartLoad = new StationEvent<SceneType>();
        
        /// <summary>
        /// the scene just finished loading, we load all the save for that area
        /// </summary>
        public static readonly StationEvent<SceneType> OnSceneInitialize = new StationEvent<SceneType>();
        
        /// <summary>
        /// trigger when it is time to initialize the npc, items, or other objects in the scene
        /// </summary>
        public static readonly StationEvent<SceneType> OnSceneLoadObjects = new StationEvent<SceneType>();
        
        
        /// <summary>
        /// the scene is ready, can hide loading screen
        /// </summary>
        public static readonly StationEvent<SceneType> OnSceneReady = new StationEvent<SceneType>();
      
        /// <summary>
        /// we are leaving this area, use this to cancel all pending actions, animation...
        /// </summary>
        public static readonly StationEvent OnBeforeLeaveScene = new StationEvent();
        
        #region UI
        public static readonly StationEvent<UiEventData> OnUiEvent = new StationEvent<UiEventData>();
        #endregion
    }
}

