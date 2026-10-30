using UnityEngine;
using UnityEngine.SceneManagement;

public class InputSystem : MonoBehaviour
{
	[SerializeField] GameObject Canvas;
	public void SetCanvasActive()
	{
		if (Canvas.activeInHierarchy)
		{
			Canvas.SetActive(false);
		}
		else
		{
			Canvas.SetActive(true);
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			SetCanvasActive();
		}
	}
	public void SetPause()
	{
		if (Time.timeScale == 0.0f)
		{
			Time.timeScale = 1.0f;
		}
		else
		{
			Time.timeScale = 0.0f;
		}
	}
	public void Resume()
	{
		Time.timeScale = 1.0f;
		Canvas.SetActive(false);
	}
	public void ToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
