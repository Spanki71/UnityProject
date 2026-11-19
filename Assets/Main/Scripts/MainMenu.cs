using System;
using TMPro;
using UnityEngine;

namespace Main.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_scoreText;
        [SerializeField]
        private GameObject m_clearButton;
        public event Action OnQuitGame;
        public event Action OnStartGame;

        public void UpdateScore()
        {
            float time = PlayerPrefs.GetFloat("TimeScore", 0);
            int count = PlayerPrefs.GetInt("EnemyCountScore", 0);
            int waves = PlayerPrefs.GetInt("WavesScore", 0);
            if (time != 0 && count != 0)
            {
                m_scoreText.text = $"Лучшее время выживания: {time}\nНаибольшее количество сбитых противников: {count}" +
                                   $"\nМаксимум волн прожито: {waves}";
                m_clearButton.SetActive(true);
            }
            else
            {
                m_scoreText.text = "";
                m_clearButton.SetActive(false);
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

        public void ClearScore()
        {
            PlayerPrefs.SetFloat("TimeScore", 0);
            PlayerPrefs.SetInt("EnemyCountScore", 0);
            PlayerPrefs.SetInt("WavesScore", 0);
            m_clearButton.SetActive(false);
            
        }
    }
}
