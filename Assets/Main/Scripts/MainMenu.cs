using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();
        }
        public void StartGame()
        {
            SceneManager.LoadScene("Main/MainScene");
        }
    }
}
