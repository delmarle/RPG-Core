using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public static partial class GameGlobalEvents
    {
        
        #region TEAM
        public static StationEvent<BaseCharacter> OnCharacterAdded = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnCharacterRemoved = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnLeaderChanged = new StationEvent<BaseCharacter>();
        #endregion

    }
}