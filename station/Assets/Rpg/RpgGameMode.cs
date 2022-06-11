namespace Station
{
    public class RpgGameMode : GameMode
    {
        public bool CreatePlayerTeam;

        protected override void OnEnterScene()
        {
            if (CreatePlayerTeam)
            {
                var teamSystem = GameInstance.GetSystem<TeamSystem>();
                teamSystem.InitializeTeam();
            }
        }

        protected override void OnExitScene()
        {}
    }
}

