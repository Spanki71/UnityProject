using UnityEngine;

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

        public void OnEnter()
        {
            m_mainMenu.OnQuitGame += MainMenuOnQuitGame;
            m_mainMenu.OnStartGame += MainMenuOnStartGame;
        }

        private void MainMenuOnStartGame()
        {
            m_stateMachine.ChangeState<GamePlayState>();
        }

        private void MainMenuOnQuitGame()
        {
            Debug.Log("MainMenuOnQuitGame");
            Application.Quit();
        }

        public void OnExit()
        {
            m_mainMenu.OnQuitGame -= MainMenuOnQuitGame;
            m_mainMenu.OnStartGame -= MainMenuOnStartGame;
        }
    }
}