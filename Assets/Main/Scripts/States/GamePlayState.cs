using Main.Interfaces;
using UnityEngine;

namespace Main.Scripts.States
{
    public class GamePlayState : IGameState,  IUpdateable
    {
        private StateMachine m_stateMachine;
        private PlayerController m_playerController;
        public GamePlayState(
            StateMachine stateMachine,
            PlayerController playerController
            )
        {
            m_stateMachine =  stateMachine;
            m_playerController = playerController;
        }

        public void OnEnter()
        {
            m_playerController.OnDeath += GameOver;
            m_playerController.SetInputEnabled(true);
        }
        
        public void OnExit()
        {
            m_playerController.OnDeath -= GameOver;
            m_playerController.SetInputEnabled(false);
        }
        
        public void Update()
        {
            
        }
        
        private void PlayerLogic()
        {
            
        }

        private void GameOver()
        {
            m_stateMachine.ChangeState<GameOverState>();
            Debug.Log($"Game Over ");
        }
    }
}