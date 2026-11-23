using Main.Scripts.Enemy;
using Main.Scripts.Interfaces;
using UnityEngine;

namespace Main.Scripts.States
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        public ObjectsToSpawn MainLevelObjectsToSpawn;
        
        [Header("UI")]
        [SerializeField] private GameObject m_mainMenu;
        [SerializeField] private GameObject m_gamePlayUI;
        [SerializeField] private GameObject m_pauseMenu;
        [SerializeField] private GameObject m_gameOverCanvas;
        
        [Header("Cutscene")]
        [SerializeField] private GameObject m_director;
        
        private IGameState m_currentState;
        private StateMachine m_stateMachine;

        private static GameManager gameManagerInstanse;
        private Score m_score;
        private WavesLogic m_wavesLogic;
        
        public void OnLeaveGameHandler()
        {
            m_stateMachine.onLeaveGame -= OnLeaveGameHandler;
            gameManagerInstanse = null;
        }
        
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
            m_wavesLogic =  new WavesLogic();
            
            m_mainMenu = GameObject.Instantiate(m_mainMenu);
            m_gameOverCanvas = GameObject.Instantiate(m_gameOverCanvas);
            m_gamePlayUI = GameObject.Instantiate(m_gamePlayUI);
            m_pauseMenu = GameObject.Instantiate(m_pauseMenu);

            
            m_mainMenu.SetActive(false);
            m_gameOverCanvas.SetActive(false);
            m_gamePlayUI.SetActive(false);
            m_pauseMenu.SetActive(false);
            
            m_stateMachine.Register(new MainMenuState(m_stateMachine, m_mainMenu));
            m_stateMachine.Register(new GamePlayState(
                m_stateMachine,
                MainLevelObjectsToSpawn,
                m_score,
                m_gamePlayUI,
                m_pauseMenu,
                m_wavesLogic,
                m_director));
            m_stateMachine.Register(new GameOverState(m_stateMachine, m_gameOverCanvas, m_score));

            m_stateMachine.onLeaveGame += OnLeaveGameHandler;
            m_stateMachine.ChangeState<MainMenuState>();
            
            
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_mainMenu);
            DontDestroyOnLoad(m_gameOverCanvas);
            DontDestroyOnLoad(m_gamePlayUI);
            DontDestroyOnLoad(m_pauseMenu);
        }
    }
}