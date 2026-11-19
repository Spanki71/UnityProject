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
        private Score m_score;
        
        public GameOverState(StateMachine stateMachine, GameObject canvas, Score score)
        {
            m_stateMachine =  stateMachine;
            m_canvas =  canvas;
            m_score = score;
            m_gameOverLogic = m_canvas.GetComponent<GameOverLogic>();
        }

        public void OnEnter()
        {
            m_canvas.SetActive(true);
            m_gameOverLogic.OnLeave += Leave;
            m_gameOverLogic.SetScoreOnUI(m_score);
            
            float time = PlayerPrefs.GetFloat("TimeScore", 0);
            int count = PlayerPrefs.GetInt("EnemyCountScore", 0);
            float newTime = m_score.GetTimeScore();
            int newCount = m_score.GetEnemyCountScore();
            
            if (newTime > time)
            {
                PlayerPrefs.SetFloat("TimeScore", newTime);
            }

            if (newCount > count)
            {
                PlayerPrefs.SetInt("EnemyCountScore", newCount);
            }
            
        }

        public void OnExit()
        {
            
            m_canvas.SetActive(false);
            m_gameOverLogic.OnLeave -= Leave;
        }

        public void Leave()
        {
            m_stateMachine.ChangeState<MainMenuState>();
        }
    }
}