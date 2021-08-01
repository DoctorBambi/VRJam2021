using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of lazer shot that goes a certain distance and then explodes, effecting nearby units.
/// If it connects with a unit, it will explode at that location.
/// </summary>
public class scriptLazerBlast : MonoBehaviour
{
	public float bulletDamage = 5f;
	public float bulletSpeed = 5f;
	//public bool isFiring = false;
	public float lifeTime = 5f;
	public float blastRadius = 1f;

	//Coroutines
	private IEnumerator blastRoutine;

	private void Start()
	{
		
	}

	private void Update()
	{
		//Move bullet forward
		//if (isFiring)
			//transform.position += transform.forward * bulletSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		DoBlast();
	}

	public void BulletFired()
	{
		//Clear out old routine
		StopCoroutine(blastRoutine);
		blastRoutine = null;

		blastRoutine = BlastRoutine();
		StartCoroutine(blastRoutine);
	}

	void DoBlast()
	{
		var enemyShipLayer = 1 << 13;
		var hitColliders = Physics.OverlapSphere(transform.position, blastRadius, enemyShipLayer);

		for (var i = 0; i < hitColliders.Length; i++)
		{
			hitColliders[i].SendMessage("Stun", SendMessageOptions.DontRequireReceiver);
		}

		print("Blast!");
		Destroy(gameObject);
	}

	//Coroutines
	private IEnumerator BlastRoutine()
	{
		yield return new WaitForSeconds(lifeTime);//at the end of life, do an explosion

		blastRoutine = null;

		DoBlast();
	}
}
