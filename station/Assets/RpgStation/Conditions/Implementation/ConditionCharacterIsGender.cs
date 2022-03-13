using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Condition/gender")]
    public class ConditionCharacterIsGender : ConditionCharacter
    {
        public string GenderId;
        
        public override bool EvaluateCondition(BaseCharacter character)
        {
            if (character == null) return false;
            
            return character.GetGender().Equals(GenderId);
        }
    }
}

