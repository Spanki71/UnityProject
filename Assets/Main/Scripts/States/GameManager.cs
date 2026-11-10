using System;
using UnityEngine;

namespace Main.Scripts.States
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerController m_playerController;
        private event Action smt;
        private IGameState m_currentState;
        private StateMachine m_stateMachine;
        
        private void Awake()
        {
            m_stateMachine = new StateMachine();
            m_stateMachine.Register(new MainMenuState(m_stateMachine));
            m_stateMachine.Register(new GamePlayState(m_stateMachine, m_playerController));
            m_stateMachine.Register(new GameOverState(m_stateMachine));
            
            m_stateMachine.ChangeState<GamePlayState>();
            
        }

        private void Update()
        {
            m_stateMachine.Update();
        }


    }
}