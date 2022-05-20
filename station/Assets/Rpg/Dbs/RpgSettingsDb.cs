using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Rpg settings")]
    public class RpgSettingsDb : SingleFieldDb<RpgGameSettingsModel>
    {
        public override string ObjectName()
        {
            return "RpgSettings";
        }
    }
}


