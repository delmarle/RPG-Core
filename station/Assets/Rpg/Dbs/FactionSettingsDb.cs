using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Faction settings")]
    public class FactionSettingsDb : SingleFieldDb<FactionSettingModel>
    {
        public override string ObjectName()
        {
            return "Faction Settings";
        }
        
        private void OnEnable()
        {
            if (Database.Ranks.Any() == false)
            {
                Database.Ranks.Add(new FactionRank { Name = "Neutral" });
            }
        }
    }
}

