using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Game settings")]
    public class GameSettingsDb : SingleFieldDb<GameSettingsModel>
    {
        public override string ObjectName()
        {
            return "GameSettings";
        }
    }
}


