using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DropEarthDetector : MonoBehaviour
{
	public Transform player;
	public float maxDistance = 20;

	void Update()
	{
		if (Vector3.Distance(transform.position, player.position) > maxDistance)
		{
			scriptEarth.Instance.deathText = "EARTH IS LOST";
			scriptEarth.Instance.SetCurrentState(scriptPlanetoid.states.Dead); //previously: SceneManager.LoadScene(2);
		}
	}
}
