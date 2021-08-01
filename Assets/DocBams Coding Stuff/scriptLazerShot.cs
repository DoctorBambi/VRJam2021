using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptLazerShot : MonoBehaviour
{
	public float bulletDamage = 5f;
	public float bulletSpeed = 5f;
	public bool isFiring = false;

	private void Update()
	{
		//Move bullet forward
		if (isFiring)
			transform.position += transform.forward * bulletSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Earth")
		{
			other.SendMessage("HandleDamage", bulletDamage, SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
