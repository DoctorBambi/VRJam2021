using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
	bool toggle = true;
	bool menuButton;
	[SerializeField]
	GameObject UI;
    void Update()
    {
        menuButton = InputBridge.Instance.XButtonDown;
		if(menuButton){
			UI.SetActive(toggle);
			toggle = !toggle;
		}
    }
	public void loadMenu(){
		SceneManager.LoadScene(0);	
	}
}
