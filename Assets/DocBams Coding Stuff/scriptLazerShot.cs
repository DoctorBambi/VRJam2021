using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptLazerShot : MonoBehaviour
{
	public float bulletDamage = 5f;
	public float bulletSpeed = 5f;

	private void Update()
	{
		//Move bullet forward
		transform.position += transform.forward * bulletSpeed * Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.name == "Earth")
		{
			collision.collider.SendMessage("HandleDamage", bulletDamage, SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
