using System;
using Main.Scripts.States;
using TMPro;
using UnityEngine;

namespace Main.Scripts
{
    public class GameOverLogic : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_scoreText;
        public event Action OnLeave;
        private StateMachine m_stateMachine;

        public void OnButtonClicked()
        {
            OnLeave?.Invoke();
        }

        public void SetScoreOnUI(Score score)
        {
            m_scoreText.text = score.GetScoreString();
        }
    }
}