using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public partial class BaseCharacter
    {
        private static PlayerClassDb _playerClassDb;
        private static RaceDb _raceDb;
        private static FactionDb _factionDb;
   
        public static void CacheAllDb()
        {
            _playerClassDb = RpgStation.GetDb<PlayerClassDb>();
            _raceDb = RpgStation.GetDb<RaceDb>();
            _factionDb = RpgStation.GetDb<FactionDb>();
        }
        public string GetLocalizedClass()
        {
            var characterType = GetMeta(StationConst.CHARACTER_TYPE);
            if (characterType.GetType() == typeof(PlayerCharacterType))
            {
                var classId = GetMeta<string>(StationConst.CLASS_ID);
                var classMeta = _playerClassDb.GetEntry(classId);
                return classMeta.Name;
            }
          
            return "";
        }
        
        public string GetLocalizedRace()
        {
            var raceMeta = _raceDb.GetEntry(GetRaceID());
            return raceMeta.Name;
        }
        
        public string GetLocalizedFaction()
        {
            var factionMeta = _factionDb.GetEntry(GetFactionID());
            return factionMeta.Name;
        }
        
        public string LocalizedGenderName()
        {
            //todo
            return GetGender();
        }
    }
}

