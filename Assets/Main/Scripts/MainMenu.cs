using System;
using TMPro;
using UnityEngine;

namespace Main.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_scoreText;
        public event Action OnQuitGame;
        public event Action OnStartGame;

        public void UpdateScore()
        {
            float time = PlayerPrefs.GetFloat("TimeScore", 0);
            int count = PlayerPrefs.GetInt("EnemyCountScore", 0);
            if (time != 0 && count != 0)
            {
                m_scoreText.text = $"Лучшее время выживания: {time}\nНаибольшее количество сбитых противников: {count}";
            }
            else
            {
                m_scoreText.text = "";
            }
        }

        public void Quit()
        {
            OnQuitGame?.Invoke();
        }
        public void StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}
