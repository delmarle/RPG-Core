using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
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

