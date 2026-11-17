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
        private Score m_score;
        
        
        private void Awake()
        {
            if (gameManagerInstanse != null && gameManagerInstanse != this)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            gameManagerInstanse = this;
            
            m_stateMachine = new StateMachine();
            m_score = new Score();
            m_mainMenu = GameObject.Instantiate(m_mainMenu);
            m_gameOverCanvas = GameObject.Instantiate(m_gameOverCanvas);
            m_mainMenu.SetActive(false);
            m_gameOverCanvas.SetActive(false);
            
            m_stateMachine.Register(new MainMenuState(m_stateMachine, m_mainMenu));
            m_stateMachine.Register(new GamePlayState(m_stateMachine, MainLevelObjectsToSpawn, m_score));
            m_stateMachine.Register(new GameOverState(m_stateMachine, m_gameOverCanvas, m_score));

            m_stateMachine.OnLeaveGame += LeaveGame;
            m_stateMachine.ChangeState<MainMenuState>();
            
            
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_mainMenu);
            DontDestroyOnLoad(m_gameOverCanvas);
        }

        public void LeaveGame()
        {
            gameManagerInstanse = null;
        }
    }
}