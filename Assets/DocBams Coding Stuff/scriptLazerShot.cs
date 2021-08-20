using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptLazerShot : MonoBehaviour
{
	#region Properties
	public bool debugging = false;
	public float bulletDamage = 5f;
	public float bulletSpeed = 5f;
	public bool isFiring = false;
	public GameObject explosionEffect;
	#endregion

	#region Monobehaviour Stuff
	private void Start()
	{
		if (explosionEffect == null) Debug.LogError("No explosion effect found on lazer shot.", gameObject);
	}

	private void Update()
	{
		//Move bullet forward
		if (isFiring)
			transform.position += transform.forward * bulletSpeed * Time.deltaTime;
	}

	public void OnTriggerEnterChild(Collider other)
	{
		if (debugging) print(other);
		
		other.SendMessage("HandleDamage", bulletDamage, SendMessageOptions.DontRequireReceiver);

		//Spawn explosion effect
		var explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
		Destroy(explosion, 5f);

		//Destroy bullet
		Destroy(gameObject);
	}
	#endregion
}
