
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Condition/Team count")]
    public class ConditionTeamCount : SimpleCondition
    {
        public int TeamCount = 1;
        
        public override bool EvaluateCondition()
        {
            var teamSystem = GameInstance.GetSystem<TeamSystem>();
            if (teamSystem == null) return false;
            
            return teamSystem.GetTeamMembers().Count >= TeamCount;
        }
    }
}