using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DropEarthDetector : MonoBehaviour
{
	public Transform player;
	public float maxDistance = 20;
	public float teleportWaitTime = 2f;//give the earth time to catch up to the player after they teleport.

	//Coroutines
	private IEnumerator teleportWaitRoutine;

	void Update()
	{
		if (teleportWaitRoutine == null && Vector3.Distance(transform.position, player.position) > maxDistance)
		{
			teleportWaitRoutine = TeleportWaitRoutine();
			StartCoroutine(teleportWaitRoutine);
		}
	}

	private IEnumerator TeleportWaitRoutine()
	{
		yield return new WaitForSeconds(teleportWaitTime);

		if (Vector3.Distance(transform.position, player.position) > maxDistance)//are we still too far away from the player?
		{
			scriptEarth.Instance.deathText = "EARTH IS LOST";
			scriptEarth.Instance.SetCurrentState(scriptPlanetoid.states.Dead); //previously: SceneManager.LoadScene(2);
		}

		teleportWaitRoutine = null;
	}
}
