using UnityEngine;

namespace Station
{
    public class RpgGameMode : GameMode
    {
        public bool CreatePlayerTeam;
        [SerializeField] private UiEventData _showUiEvent;
        [SerializeField] private float _showUiDelay = 3;

        protected override void OnEnterScene()
        {
            if (CreatePlayerTeam)
            {
                var teamSystem = GameInstance.GetSystem<TeamSystem>();
                teamSystem.InitializeTeam();
            }

            if (_showUiEvent)
            {
                Timer.Register(_showUiDelay, ShowDefaultUi);
            }
        }

        private void ShowDefaultUi()
        {
            GameGlobalEvents.OnUiEvent.Invoke(_showUiEvent);
        }

        protected override void OnExitScene()
        {}
    }
}

