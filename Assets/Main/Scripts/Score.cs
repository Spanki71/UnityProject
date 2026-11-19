using UnityEngine;

namespace Main.Scripts
{
    public class Score
    {
        private int m_enemyScore;
        private int m_waveScore;
        private float m_startTime;

        public void SetStartTime(float startTime)
        {
            m_startTime = startTime;
        }

        public void HitEnemy()
        {
            m_enemyScore++;
        }

        public void UpdateWave(GamePlayUIBrain gamePlayUI)
        {
            m_waveScore++;
            Debug.Log($"Волн прожито {m_waveScore}");
            gamePlayUI.UpdateWave(m_waveScore);
        }

        public int GetWaves()
        {
            return m_waveScore;
        }

        public string GetScoreString()
        {
            float lifetime = Time.time - m_startTime;
            return $"Набрано очков выживания: {lifetime}\nСбито противников: {m_enemyScore}";
        }

        public float GetTimeScore()
        {
            return Time.time - m_startTime;
        }

        public int GetEnemyCountScore()
        {
            return m_enemyScore;
        }

        public void Reset()
        {
            m_enemyScore = 0;
            m_startTime = Time.time;
        }
    }
}