using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public static  class RpgGameGlobalEvents
    {
        
        #region TEAM
        public static StationEvent<BaseCharacter> OnCharacterAdded = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnCharacterRemoved = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnLeaderChanged = new StationEvent<BaseCharacter>();
        #endregion

    }
}