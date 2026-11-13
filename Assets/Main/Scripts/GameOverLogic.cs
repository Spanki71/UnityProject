using System;
using Main.Scripts.States;
using UnityEngine;

namespace Main.Scripts
{
    public class GameOverLogic : MonoBehaviour
    {
        public event Action OnLeave;
        private StateMachine m_stateMachine;

        public void OnButtonClicked()
        {
            OnLeave?.Invoke();
        }
    }
}