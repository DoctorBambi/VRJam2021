using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptHandSnapper : MonoBehaviour
{
	[SerializeField] private GameObject targetPlayer;

	private Rigidbody rb;

	private void Start()
	{
		targetPlayer = GameObject.FindGameObjectWithTag("Player");
		if (targetPlayer == null) Debug.LogError("No player found.", gameObject);

		rb = GetComponent<Rigidbody>();
		if (rb == null) Debug.LogError("No rigidbody found.", gameObject);
	}

	public void SnapPosition(Vector3 newPos)
	{
		rb.MovePosition(newPos);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}
