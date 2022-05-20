using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class RuntimePassiveAbility
    {
        private PassiveAbility _abilityCache;
        private int _rank;
        private BaseCharacter _user;
        
        
        public void Initialize(PassiveAbility data, int rank, BaseCharacter user)
        {
            _abilityCache = data;
            _rank = rank;
            _user = user;
        }
    }
  

}
