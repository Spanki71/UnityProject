using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts.States
{
    public class GameOverState : IGameState
    {
        StateMachine m_stateMachine;

        private Canvas m_gameOverCanvas;
        private GameOverLogic m_gameOverLogic;
        private GameObject m_canvas;
        
        public GameOverState(StateMachine stateMachine, GameObject canvas)
        {
            m_stateMachine =  stateMachine;
            m_canvas =  canvas;
            m_gameOverLogic = m_canvas.GetComponent<GameOverLogic>();
        }

        public void OnEnter()
        {
            m_canvas.SetActive(true);
            m_gameOverLogic.OnLeave += Leave;
            Debug.Log("Game Over Logic");
        }

        public void OnExit()
        {
            m_canvas.SetActive(false);
            m_gameOverLogic.OnLeave -= Leave;
        }

        public void Leave()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

// TODO: расчитать Score