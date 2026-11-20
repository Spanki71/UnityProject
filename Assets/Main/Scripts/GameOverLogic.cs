using System;
using Main.Scripts.States;
using TMPro;
using UnityEngine;

namespace Main.Scripts
{
    public class GameOverLogic : MonoBehaviour
    {
        public event Action onLeave;
        
        [SerializeField]
        private TMP_Text m_scoreText;
        private StateMachine m_stateMachine;

        public void OnButtonClicked()
        {
            onLeave?.Invoke();
        }

        public void SetScoreOnUI(Score score)
        {
            m_scoreText.text = score.GetScoreString();
        }
    }
}