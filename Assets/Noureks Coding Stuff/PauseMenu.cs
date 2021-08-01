using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
	bool toggle = false;
	bool menuButton;
	[SerializeField]
	GameObject UI;
    void Update()
    {
        menuButton = InputBridge.Instance.AButtonDown;
		if(menuButton){
			toggle = !toggle;
			UI.SetActive(toggle);
		}
    }
	public void loadMenu(){
		SceneManager.LoadScene(0);	
	}
}
