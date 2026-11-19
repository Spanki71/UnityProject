using System;
using UnityEngine;

namespace Main.Scripts
{
    public class PauseUiBrain : MonoBehaviour
    {
        public event Action OnResume;
        public event Action OnRestart;
        public event Action OnToMenu;
        
        public void Resume()
        {
            Debug.Log("Resuming");
            OnResume?.Invoke();
        }

        public void Restart()
        {
            OnRestart?.Invoke();
        }

        public void ToMenu()
        {
            OnToMenu?.Invoke();
        }
    }
}
