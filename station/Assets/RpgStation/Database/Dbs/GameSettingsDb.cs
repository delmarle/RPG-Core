using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class GameSettingsDb : SingleFieldDb<GameSettingsModel>
    {
        public override string ObjectName()
        {
            return "GameSettings";
        }
    }
}


