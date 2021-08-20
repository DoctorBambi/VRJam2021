using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script simply passes collision information up to a parent script for processing.
/// </summary>
public class scriptCollisionConveyor : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		transform.parent.SendMessage("OnCollisionEnterChild", collision, SendMessageOptions.DontRequireReceiver);
	}

	private void OnTriggerEnter(Collider other)
	{
		transform.parent.SendMessage("OnTriggerEnterChild", other, SendMessageOptions.DontRequireReceiver);
	}
}
