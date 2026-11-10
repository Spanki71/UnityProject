namespace Main.Scripts.States
{
    public class GameOverState : IGameState
    {
        StateMachine m_stateMachine;
        public GameOverState(StateMachine stateMachine)
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