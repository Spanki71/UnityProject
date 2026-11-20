using System;
using UnityEngine;

namespace Main.Scripts
{
    public class PauseUiBrain : MonoBehaviour
    {
        public event Action onResume;
        public event Action onRestart;
        public event Action onToMenu;
        
        public void Resume()
        {
            onResume?.Invoke();
        }

        public void Restart()
        {
            onRestart?.Invoke();
        }

        public void ToMenu()
        {
            onToMenu?.Invoke();
        }
    }
}
