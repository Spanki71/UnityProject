using System;
using TMPro;
using UnityEngine;

namespace Main.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        public event Action onQuitGame;
        public event Action onStartGame;
        
        [SerializeField] private TMP_Text m_scoreText;
        [SerializeField] private GameObject m_clearButton;
        [SerializeField] private GameObject m_scorePanel;

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
                m_scorePanel.SetActive(false);
                m_clearButton.SetActive(false);
            }
        }

        public void Quit()
        {
            onQuitGame?.Invoke();
        }
        public void StartGame()
        {
            onStartGame?.Invoke();
        }

        public void ClearScore()
        {
            PlayerPrefs.SetFloat("TimeScore", 0);
            PlayerPrefs.SetInt("EnemyCountScore", 0);
            PlayerPrefs.SetInt("WavesScore", 0);
            m_clearButton.SetActive(false);
            
        }

        public void Start()
        {
            UpdateScore();
        }
    }
}
