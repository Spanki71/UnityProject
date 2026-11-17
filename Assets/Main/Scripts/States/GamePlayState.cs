using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Main.Scripts.States
{
    public class GamePlayState : IGameState
    {
        private StateMachine m_stateMachine;
        
        private GameObject m_player;
        private PlayerController m_playerController;
        private ObjectsToSpawn m_mainLevelObjectsToSpawn;
        private List<GameObject> m_spawners = new List<GameObject>(4);
        private GameObject m_enemySpawner;
        private GameObject m_target;
        private Score m_score;
        
        public GamePlayState(
            StateMachine stateMachine,
            ObjectsToSpawn mainMenuObjectsToSpawn,
            Score score
            )
        {
            m_stateMachine =  stateMachine;
            m_mainLevelObjectsToSpawn = mainMenuObjectsToSpawn;
            m_score = score;
        }

        public void OnEnter()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Main/MainScene");
            m_score.SetStartTime(Time.time);
        }
        
        public void OnExit()
        {
            m_playerController.OnDeath -= GameOver;
            m_playerController.SetInputEnabled(false);
            m_enemySpawner.SetActive(false);
            foreach (var spawner in m_spawners)
            {
                spawner.SetActive(false);
            }
        }

        private void GameOver()
        {
            m_target.GetComponentInChildren<TargetBrain>().OnGameOver -= GameOver;
            m_stateMachine.ChangeState<GameOverState>();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            foreach (var obj in m_mainLevelObjectsToSpawn.transforms)
            {
                
                GameObject temp = Object.Instantiate(obj.gameObject, obj.position, obj.rotation);

                try
                {
                    if (temp.CompareTag("Player"))
                    {
                        m_player = temp;
                        m_playerController = m_player.GetComponent<PlayerController>();
                        m_playerController.OnDeath += GameOver;
                        m_playerController.SetInputEnabled(true);
                    }
                    
                    if (temp.CompareTag("Spawner"))
                    {
                        m_spawners.Add(temp);
                    }

                    if (temp.CompareTag("EnemySpawner"))
                    {
                        m_enemySpawner =  temp;
                        temp.GetComponent<EnemySpawner>().OnHitEnemySpawner += OnHitEnemy;
                    }

                    if (temp.CompareTag("Target"))
                    {
                        m_target = temp;
                    }
                }
                catch
                {
                }
            }
            
            m_enemySpawner.GetComponent<EnemySpawner>().target = m_target;
            m_target.GetComponentInChildren<TargetBrain>().OnGameOver += GameOver;
        }

        private void OnHitEnemy()
        {
            m_score.HitEnemy();
        }
    }
}