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
        public bool playMode = false;
        
        private EnemySpawner m_spawner;
        private CancellationTokenSource m_cts =  new CancellationTokenSource();
        private int m_currentWave = 1;
        private float m_waveTime = 5.0f;
        private float m_visibleTime;

        public void SetEnemySpawner(EnemySpawner  enemySpawner)
        {
            m_spawner = enemySpawner;
            m_visibleTime = m_spawner.m_visibleTime;
        }
        
        public async void StartWaveAsync(float visibleTime)
        {
            if (!playMode) return;
            waveText.text = $"Начинается {m_currentWave} волна";
            waveInfo.SetActive(true);
            m_spawner.SetCanSpawn(true);

            await Awaitable.WaitForSecondsAsync(visibleTime, m_cts.Token);
            if (!playMode) return;
            waveInfo.SetActive(false);
            
            await Awaitable.WaitForSecondsAsync(m_waveTime, m_cts.Token);
            if (!playMode) return;
            m_spawner.SetCanSpawn(false);
        }

        public async void StopWaveAsync(float waveTime)
        {
            if (!playMode) return;
            waveText.text = $"Вы продержались {m_currentWave} волну";
            waveInfo.SetActive(true);
            
            await Awaitable.WaitForSecondsAsync(m_visibleTime, m_cts.Token);
            if (!playMode) return;
            waveInfo.SetActive(false);

            m_currentWave++;
            onWaveEnd?.Invoke();
            m_waveTime += 5.0f;
            StartWaveAsync(m_visibleTime);
        }
        
        public void CheckEndOfWave()
        {
            if ( m_spawner.GetActiveEnemyies() <= 0 && !m_spawner.GetCanSpawn())
            {
                m_waveTime += timeUpper;
                StopWaveAsync(m_waveTime);
            }
        }
    }
}