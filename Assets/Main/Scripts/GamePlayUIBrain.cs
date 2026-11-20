using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts
{
    public class GamePlayUIBrain : MonoBehaviour
    {
        public event Action onPause;
        
        [SerializeField] public GameObject wavesInfo;
        [SerializeField] public TMP_Text wavesText;
        [SerializeField] public VariableJoystick m_variableJoystick;
        
        [SerializeField] private TMP_Text m_EnemyCount;
        [SerializeField] private Image m_hpBar;
        [SerializeField] private TMP_Text m_waveScore;
    
        private float m_targetMaxHp;
        
        public void Pause()
        {
            onPause?.Invoke();
        }

        public void SetHitEnemyCount(int count)
        {
            m_EnemyCount.text = $"Сбито противников: {count}";
        }

        public void SetTargetHp(float hp)
        {
            float ratio = hp / m_targetMaxHp;
            m_hpBar.fillAmount = ratio;
            if (ratio <= 0.5f)
            {
                m_hpBar.color = Color.yellow;
            }
        }
    
        public void SetMaxHp(float hp)
        {
            m_targetMaxHp = hp;
        }

        public void UpdateWave(int wave)
        {
            m_waveScore.text = $"Волн прожито: {wave}";
        }
    }
}
