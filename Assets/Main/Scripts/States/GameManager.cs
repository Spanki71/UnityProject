using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        
        private void Awake()
        {
            m_stateMachine = new StateMachine();
            
            m_stateMachine.Register(new MainMenuState(m_stateMachine, m_mainMenu));
            m_stateMachine.Register(new GamePlayState(m_stateMachine, MainLevelObjectsToSpawn));
            m_stateMachine.Register(new GameOverState(m_stateMachine, m_gameOverCanvas));
            
            m_stateMachine.ChangeState<MainMenuState>();
            
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_mainMenu);
        }
    }
}