using UnityEngine;

namespace Main.Scripts.States
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_gameOverCanvas;
        [SerializeField]
        public ObjectsToSpawn MainLevelObjectsToSpawn;
        [SerializeField]
        private GameObject m_mainMenu;
        private IGameState m_currentState;
        private StateMachine m_stateMachine;

        private static GameManager gameManagerInstanse;
        
        
        private void Awake()
        {
            if (gameManagerInstanse != null && gameManagerInstanse != this)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            gameManagerInstanse = this;
            
            m_stateMachine = new StateMachine();

            m_stateMachine.Register(new MainMenuState(m_stateMachine, m_mainMenu));
            m_stateMachine.Register(new GamePlayState(m_stateMachine, MainLevelObjectsToSpawn));
            m_stateMachine.Register(new GameOverState(m_stateMachine, m_gameOverCanvas));

            m_stateMachine.OnLeaveGame += LeaveGame;
            m_stateMachine.ChangeState<MainMenuState>();
            
            
            var menu = GameObject.Instantiate(m_mainMenu);
            var gomenu = GameObject.Instantiate(m_gameOverCanvas);
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(menu);
            DontDestroyOnLoad(gomenu);
        }

        public void LeaveGame()
        {
            gameManagerInstanse = null;
        }
    }
}