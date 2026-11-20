using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Main.Scripts.Enemy
{
    public class WavesLogic
    {
        public event Action onWaveEnd;
        
        public GameObject waveInfo;
        public TMP_Text waveText;
        public float timeUpper = 5.0f;
        
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
            m_interval = m_spawner.waveTime;
            m_visibleTime = m_spawner.m_visibleTime;
            m_waveTime = m_spawner.m_startWaveTime;
        }
        
        public async void StartWaveAsync(float timeWaitStart, float visibleTime, float waveTime)
        {
            float time = Time.time;
            await Awaitable.WaitForSecondsAsync(timeWaitStart, m_cts.Token);
            Debug.Log(Time.time - time);
            waveText.text = $"Начинается {m_currentWave} волна";
            waveInfo.SetActive(true);
            m_spawner.canSpawn = true;

            await Awaitable.WaitForSecondsAsync(visibleTime, m_cts.Token);
            waveInfo.SetActive(false);
            
            await Awaitable.WaitForSecondsAsync(waveTime, m_cts.Token);
            m_spawner.canSpawn = false;
        }

        public async void StopWaveAsync(float waveTime)
        {
            waveText.text = $"Вы продержались {m_currentWave} волну";
            waveInfo.SetActive(true);
            
            await Awaitable.WaitForSecondsAsync(m_visibleTime, m_cts.Token);
            waveInfo.SetActive(false);

            m_currentWave++;
            onWaveEnd?.Invoke();
            m_endTime = Time.time + waveTime;
            StartWaveAsync(m_interval, m_visibleTime, m_waveTime);
        }
        
        public void CheckEndOfWave()
        {
            if ( m_spawner.GetActiveEnemyies() <= 0 && !m_spawner.canSpawn)
            {
                m_waveTime += timeUpper;
                StopWaveAsync(m_waveTime);
            }
        }
    }
}