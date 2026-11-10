namespace Main.Scripts.States
{
    public class MainMenuState : IGameState
    {
        private StateMachine m_stateMachine;
        public MainMenuState(StateMachine stateMachine)
        {
            m_stateMachine =  stateMachine;
        }

        public void OnEnter()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}