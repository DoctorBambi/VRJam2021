using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script simply keeps a position certain number of units away from the reference (namely the player) only in the z axis.
/// Used by the asteroid spawner to spawn the next set of asteroids as needed.
/// Added this because when the spawner is placed directly on the player, you could sometimes get asteroids spawned directly on you,
/// which is kinda freaky to experience.
/// I couldn't just parent this point to the player because they moves their head all over causing lots of
/// spawn events, which become compute heavy and led to asteroids spawning on top of each other.
/// This currently only works if the sun is down the z axis from the player.
/// </summary>
public class scriptAsteroidLead : MonoBehaviour
{
	public int distance;
	public Transform reference;

	private void Start()
	{
		if (reference == null)
			Debug.LogError("No reference point assigned.", gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, reference.position.z - distance);
	}
}
