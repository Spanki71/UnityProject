using System;
using System.Threading;
using TMPro;
using UnityEngine;


namespace Main.Scripts
{
    public class WavesLogic
    {
        public GameObject waveInfo;
        public TMP_Text waveText;

        public float timeUpper = 5.0f;
        public event Action OnWaveEnd;
        
        private EnemySpawner m_spawner;

        private CancellationTokenSource m_cts =  new CancellationTokenSource();
        
        private int m_currentWave = 1;
        
        private float m_interval;
        private float m_waveTime;
        private float m_visibleTime;
        private float m_endTime;

        public void SetEnemySpawner(EnemySpawner  enemySpawner)
        {
            m_spawner = enemySpawner;
            m_interval = m_spawner.wateInterval;
            m_visibleTime = m_spawner.m_visibleTime;
            m_waveTime = m_spawner.m_startWaveTime;
        }
        
        public async void StartWave(float timeWaitStart, float visibleTime, float waveTime)
        {
            await Awaitable.WaitForSecondsAsync(timeWaitStart, m_cts.Token);
            waveText.text = $"Начинается {m_currentWave} волна";
            waveInfo.SetActive(true);
            m_spawner.canSpawn = true;

            await Awaitable.WaitForSecondsAsync(visibleTime, m_cts.Token);
            waveInfo.SetActive(false);
            
            await Awaitable.WaitForSecondsAsync(waveTime, m_cts.Token);
            m_spawner.canSpawn = false;
        }

        public async void StopWave(float waveTime)
        {
            waveText.text = $"Вы продержались {m_currentWave} волну";
            waveInfo.SetActive(true);
            
            await Awaitable.WaitForSecondsAsync(m_visibleTime, m_cts.Token);
            waveInfo.SetActive(false);

            m_currentWave++;
            OnWaveEnd?.Invoke();
            m_endTime = Time.time + waveTime;
            StartWave(m_interval, m_visibleTime, m_waveTime);
        }

        

        public void CheckEndOfWave()
        {
            if ( m_spawner.GetActiveEnemyies() <= 0 && !m_spawner.canSpawn)
            {
                m_waveTime += timeUpper;
                StopWave(m_waveTime);
            }
        }

    }
}