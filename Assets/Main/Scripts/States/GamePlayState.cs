using System.Collections.Generic;
using Main.Scripts.Enemy;
using Main.Scripts.Interfaces;
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
        private GameObject m_enemySpawnerGo;
        private EnemySpawner m_enemySpawner;
        private GameObject m_target;
        private TargetBrain  m_targetBrain;
        private Score m_score;

        private GameObject m_gamePlayUI;
        private GamePlayUIBrain m_gameUIBrain;
        private GameObject m_pauseMenu;
        private PauseUiBrain m_pauseBrain;
        private WavesLogic m_wavesLogic;
        
        public GamePlayState(
            StateMachine stateMachine,
            ObjectsToSpawn mainMenuObjectsToSpawn,
            Score score,
            GameObject gamePlayUI,
            GameObject pauseMenu,
            WavesLogic wavesLogic
            )
        {
            m_stateMachine =  stateMachine;
            m_mainLevelObjectsToSpawn = mainMenuObjectsToSpawn;
            m_score = score;
            m_gamePlayUI = gamePlayUI;
            m_pauseMenu = pauseMenu;
            m_wavesLogic = wavesLogic;
        }

        public void OnEnter()
        {
            m_gameUIBrain = m_gamePlayUI.GetComponent<GamePlayUIBrain>();
            m_pauseBrain = m_pauseMenu.GetComponent<PauseUiBrain>();
            m_wavesLogic.waveInfo = m_gameUIBrain.wavesInfo;
            m_wavesLogic.waveText = m_gameUIBrain.wavesText;
            m_gamePlayUI.SetActive(true);
            m_gameUIBrain.onPause += OnPauseHandler;

            m_pauseBrain.onResume += OnResumeHandler;
            m_pauseBrain.onRestart += OnRestartHandler;
            m_pauseBrain.onToMenu += OnToMenuHandler;
            
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            SceneManager.LoadScene("Main/MainScene");
            m_score.SetStartTime(Time.time);
        }

        public void OnExit()
        {
            m_wavesLogic.waveInfo = null;
            m_wavesLogic.waveText = null;
            
            m_playerController.SetInputEnabled(false);
            
            m_playerController.onDeath -= OnGameOverHandler;
            m_gameUIBrain.onPause -= OnPauseHandler;
            m_pauseBrain.onResume -= OnResumeHandler;
            m_pauseBrain.onRestart -= OnRestartHandler;
            m_pauseBrain.onToMenu -= OnToMenuHandler;
            m_enemySpawner.onEnemyDieUpdateScore -= OnUpdateScoreHandler;
            m_enemySpawner.onWaveEndToUpdateScore -= OnUpdateOnWaveHandler;
            m_targetBrain.onGetDamage -= OnSetGamePlayUIInfoHandler;
            
        }
        
        private void OnSceneLoadedHandler(Scene scene, LoadSceneMode sceneMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedHandler;
            m_spawners.Clear();
            foreach (var obj in m_mainLevelObjectsToSpawn.transforms)
            {
                GameObject temp = Object.Instantiate(obj.gameObject, obj.position, obj.rotation);
                try
                {
                    if (temp.CompareTag("Player"))
                    {
                        m_player = temp;
                        m_playerController = m_player.GetComponent<PlayerController>();
                        m_playerController.onDeath += OnGameOverHandler;
                        m_playerController.SetInputEnabled(false);
                        m_playerController.cam.enabled = false;
                        m_playerController.variableJoystick = m_gameUIBrain.m_variableJoystick;
                    }
                    
                    if (temp.CompareTag("Spawner"))
                    {
                        m_spawners.Add(temp);
                    }

                    if (temp.CompareTag("EnemySpawner"))
                    {
                        m_enemySpawnerGo = temp;
                        m_enemySpawner = temp.GetComponent<EnemySpawner>();
                        m_enemySpawner.SetWaveLogic(m_wavesLogic);
                    }

                    if (temp.CompareTag("Target"))
                    {
                        m_target = temp;
                        m_targetBrain = m_target.GetComponentInChildren<TargetBrain>();
                        m_targetBrain.onGetDamage += OnSetGamePlayUIInfoHandler;
                        m_gameUIBrain.SetMaxHp(m_targetBrain.GetMaxHealth());
                    }
                }
                catch
                {
                }
            }
            
            m_enemySpawnerGo.GetComponent<EnemySpawner>().target = m_target;
            m_target.GetComponentInChildren<TargetBrain>().onGameOver += OnGameOverHandler;
            m_gameUIBrain.SetHitEnemyCount(m_score.GetEnemyCountScore());
            m_enemySpawner.onEnemyDieUpdateScore += OnUpdateScoreHandler;
            m_enemySpawner.onWaveEndToUpdateScore += OnUpdateOnWaveHandler;
        }

        private void OnPauseHandler()
        {
            Time.timeScale = 0;
            m_pauseMenu.SetActive(true);
            
            m_playerController.SetInputEnabled(false);
            m_enemySpawnerGo.SetActive(false);
            m_gamePlayUI.SetActive(false);
            
            foreach (var spawner in m_spawners)
            {
                Debug.Log(spawner);
                spawner.SetActive(false);
            }
        }

        private void OnResumeHandler()
        {
            m_playerController.SetInputEnabled(true);
            m_enemySpawnerGo.SetActive(true);
            m_gamePlayUI.SetActive(true);
            
            foreach (var spawner in m_spawners)
            {
                spawner.SetActive(true);
            }

            m_pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }

        private void OnRestartHandler()
        {
            ResetLevel();
            Time.timeScale = 1;
            m_playerController.SetInputEnabled(false);
            m_pauseMenu.SetActive(false);
            m_stateMachine.ChangeState<GamePlayState>();
        }

        private void OnToMenuHandler()
        {
            ResetLevel();
            Time.timeScale = 1;
            m_pauseMenu.SetActive(false);
            m_playerController.SetInputEnabled(false);
            m_stateMachine.ChangeState<MainMenuState>();
        }

        private void OnGameOverHandler()
        {
            m_target.GetComponentInChildren<TargetBrain>().onGameOver -= OnGameOverHandler;
            m_stateMachine.ChangeState<GameOverState>();
        }
        
        private void OnUpdateScoreHandler()
        {
            m_score.HitEnemy();
            m_gameUIBrain.SetHitEnemyCount(m_score.GetEnemyCountScore());
        }

        private void OnUpdateOnWaveHandler()
        {
            m_score.UpdateWave(m_gameUIBrain);
        }

        private void OnSetGamePlayUIInfoHandler()
        {
            m_gameUIBrain.SetTargetHp(m_targetBrain.health);
        }

        private void ResetLevel()
        {
            m_targetBrain.Reset();
            m_score.Reset();
        }
    }
}