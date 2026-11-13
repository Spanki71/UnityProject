using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace Main.Scripts.States
{
    public class GamePlayState : IGameState
    {
        private StateMachine m_stateMachine;
        private GameObject m_player;
        private PlayerController m_playerController;
        private ObjectsToSpawn m_mainLevelObjectsToSpawn;
        private List<GameObject> m_spawners = new List<GameObject>(4);
        
        public GamePlayState(
            StateMachine stateMachine,
            ObjectsToSpawn mainMenuObjectsToSpawn
            )
        {
            m_stateMachine =  stateMachine;
            m_mainLevelObjectsToSpawn = mainMenuObjectsToSpawn;
        }

        public void OnEnter()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Main/MainScene");
        }
        
        public void OnExit()
        {
            m_playerController.OnDeath -= GameOver;
            m_playerController.SetInputEnabled(false);
            Debug.Log("Exit Playmode");

        }

        private void GameOver()
        {
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
                    if (temp.tag == "Player")
                    {
                        m_player = temp;
                        m_playerController = m_player.GetComponent<PlayerController>();
                        m_playerController.OnDeath += GameOver;
                        m_playerController.SetInputEnabled(true);
                    }
                    else if (temp.tag == "Spawner")
                    {
                        m_spawners.Add(temp);
                    }
                }
                catch
                {
                }
            }
            
        }
    }
}