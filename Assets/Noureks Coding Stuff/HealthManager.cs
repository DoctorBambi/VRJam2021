using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HealthManager : MonoBehaviour
{
	[SerializeField]
	GameObject deathScreen;
	public int health;
	public bool isDead;
	[SerializeField]
	int maxHealth = 100;
	void Start(){
		Time.timeScale = 1;
		RestoreHealth();
	}
	void RestoreHealth(){
		health = maxHealth;
	}
	void Update(){
		if(health < 1){
			isDead = true;
		}
		if(isDead){
			deathScreen.SetActive(true);
			Time.timeScale=0;
		}
	}
	void takeDamage(int damage){
		health -= damage;
	}
	public void ReloadScene(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);	
	}
}
