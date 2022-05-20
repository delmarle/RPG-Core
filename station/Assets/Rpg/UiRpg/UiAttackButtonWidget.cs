
using UnityEngine;
using UnityEngine.EventSystems;


namespace Station
{
    public class UiAttackButtonWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button _button;
        private BaseCharacter _cachedCharacter;
        private RpgActionHandler _action;

        private void Awake()
        {
            GameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);
            //var teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
            //OnLeaderChanged(teamSystem.GetCurrentLeader());
        }

        private void OnDestroy()
        {
            GameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
        }

        private void OnLeaderChanged(BaseCharacter leader)
        {
            _cachedCharacter = leader;
            _action = leader.Action as RpgActionHandler;
        }


        public void OnPointerDown(PointerEventData eventData){

            _action?.WantAttack();
       
        }

        public void OnPointerUp(PointerEventData eventData){
            _action?.SetAttacking(false);
        }

  
    }


}
