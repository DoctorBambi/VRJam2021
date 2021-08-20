using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptStunGun : MonoBehaviour
{
	#region Properties
	public float slashDamage = 25f;
	public bool isGrabbed = false;

	private Rigidbody rb;
	#endregion

	#region MonoBehaviour Stuff
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		if (rb == null) Debug.LogError("No rigidbody found.", gameObject);
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.collider.CompareTag("Earth"))
			collision.collider.SendMessage("HandleDamage", slashDamage, SendMessageOptions.DontRequireReceiver);
	}
	#endregion

	#region External Inputs
	public void SnapPosition(Vector3 newPos)
	{
		if (isGrabbed)
		{
			rb.MovePosition(newPos);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	public void SetIsGrabbed(bool newState)
	{
		isGrabbed = newState;
	}
	#endregion
}
