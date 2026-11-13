using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        public event Action OnQuitGame;
        public event Action OnStartGame;
        public void Quit()
        {
            OnQuitGame?.Invoke();
        }
        public void StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}
