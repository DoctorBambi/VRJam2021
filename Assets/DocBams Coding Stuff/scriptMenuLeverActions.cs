using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scriptMenuLeverActions : MonoBehaviour
{
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void CloseApplication()
	{
		Application.Quit();
	}

	public void OnLeverChange(Transform leverHandle)
	{
		print(leverHandle.localPosition);
	}
}
