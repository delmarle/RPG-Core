using UnityEngine;

namespace Station
{
    public class DefaultFactionHandler : IFactionHandler
    {
        private FactionDb _db;
        public int ResolveStance(string sourceID, string targetID)
        {
            if (_db == null)
            {
                _db = RpgStation.GetDb<FactionDb>();
            }
            
            var sourceFaction = _db.GetEntry(sourceID);
            if (string.Equals(sourceID, targetID))
            {
                return sourceFaction.StanceToSelf;
            }

            if (sourceFaction == null || string.IsNullOrEmpty(sourceID))
            {
                Debug.LogError("missing faction");
                return 0;
            }

            var result = sourceFaction.GetStance(targetID);
            return result;
        }
    }

}

