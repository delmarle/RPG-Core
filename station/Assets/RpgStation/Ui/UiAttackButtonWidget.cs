
using UnityEngine;
using UnityEngine.EventSystems;


namespace Station
{
    public class UiAttackButtonWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button _button;
        private BaseCharacter _cachedCharacter;
        private PlayerActionHandler _action;

        private void Awake()
        {
            TeamSystem.OnLeaderChanged.AddListener(OnLeaderChanged);
            //var teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            //OnLeaderChanged(teamSystem.GetCurrentLeader());
        }

        private void OnDestroy()
        {
            TeamSystem.OnLeaderChanged.RemoveListener(OnLeaderChanged);
        }

        private void OnLeaderChanged(BaseCharacter leader)
        {
            _cachedCharacter = leader;
            _action = leader.Action as PlayerActionHandler;
        }


        public void OnPointerDown(PointerEventData eventData){

            _action?.WantAttack();
       
        }

        public void OnPointerUp(PointerEventData eventData){
            _action?.SetAttacking(false);
        }

  
    }


}
