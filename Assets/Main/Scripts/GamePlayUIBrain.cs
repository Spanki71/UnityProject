using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIBrain : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_EnemyCount;
    [SerializeField]
    private Image m_hpBar;
    
    public event Action OnPause;
    private float m_targetMaxHp;
    public void Pause()
    {
        OnPause?.Invoke();
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
}
