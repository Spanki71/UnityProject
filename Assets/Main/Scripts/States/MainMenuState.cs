using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts.States
{
    public class MainMenuState : IGameState
    {
        private StateMachine m_stateMachine;
        private GameObject m_mainMenuCanvas;
        private MainMenu m_mainMenu;
        public MainMenuState(StateMachine stateMachine,  GameObject canvas)
        {
            m_stateMachine =  stateMachine;
            m_mainMenuCanvas = canvas;
            m_mainMenu = m_mainMenuCanvas.gameObject.GetComponent<MainMenu>();
        }

        private void MainMenuOnStartGame()
        {
            m_stateMachine.ChangeState<GamePlayState>();
        }

        private void MainMenuOnQuitGame()
        {
            Application.Quit();
        }
        
        public void OnEnter()
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneManager.LoadScene("Main/MainMenu");
            }
            
            m_mainMenu.UpdateScore();
            m_mainMenuCanvas.SetActive(true);
            m_mainMenu.OnQuitGame += MainMenuOnQuitGame;
            m_mainMenu.OnStartGame += MainMenuOnStartGame;

            m_mainMenu.OnQuitGame += m_stateMachine.Leave;
        }

        public void OnExit()
        {
            m_mainMenuCanvas.SetActive(false);
            m_mainMenu.OnQuitGame -= MainMenuOnQuitGame;
            m_mainMenu.OnStartGame -= MainMenuOnStartGame;
            
            m_mainMenu.OnQuitGame -= m_stateMachine.Leave;
        }
    }
}