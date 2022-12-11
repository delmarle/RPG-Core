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
            _playerClassDb = GameInstance.GetDb<PlayerClassDb>();
            _raceDb = GameInstance.GetDb<RaceDb>();
            _factionDb = GameInstance.GetDb<FactionDb>();
        }
        public string GetLocalizedClass()
        {
            var characterType = GetMeta(RpgConst.CHARACTER_TYPE);
            if (characterType.GetType() == typeof(PlayerCharacterType))
            {
                var classId = GetMeta<string>(RpgConst.CLASS_ID);
                var classMeta = _playerClassDb.GetEntry(classId);
                return classMeta.Name.GetValue();
            }
          
            return "";
        }
        
        public string GetLocalizedRace()
        {
            var raceMeta = _raceDb.GetEntry(GetRaceID());
            return raceMeta.Name.GetValue();
        }
        
        public string GetLocalizedFaction()
        {
            var factionMeta = _factionDb.GetEntry(GetFactionID());
            return factionMeta.Name.GetValue();
        }
        
        public string LocalizedGenderName()
        {
            //todo
            return GetGender();
        }
    }
}

