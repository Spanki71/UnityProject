using Main.Interfaces;
using UnityEngine;

namespace Main.Scripts.States
{
    public class GamePlayState : IGameState,  IUpdateable
    {
        private StateMachine  m_stateMachine;
        private PlayerController  m_playerController;
        public GamePlayState(
            StateMachine stateMachine
            )
        {
            m_stateMachine =  stateMachine;
        }

        public void OnEnter()
        {
            // Как сделать лучше?
            m_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            //
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
        }
    }
}