namespace Station
{
    
    public static class GameGlobalEvents 
    {
        public static StationEvent OnDataBaseLoaded = new StationEvent();
        public static StationEvent OnBeforeLeaveScene = new StationEvent();
        public static StationEvent OnSceneStartLoad = new StationEvent();
        public static StationEvent OnSceneInitialize = new StationEvent();
        public static StationEvent OnSceneLoadObjects = new StationEvent();
        public static StationEvent OnSceneReady = new StationEvent();
        public static StationEvent OnTriggerSceneSave = new StationEvent();
        public static StationEvent OnEnterGame = new StationEvent();
    }
}

