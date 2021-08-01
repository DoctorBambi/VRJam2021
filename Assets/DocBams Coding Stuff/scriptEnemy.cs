using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemy : MonoBehaviour
{
    public scriptEnemyPack pack;
    public float sightRadius = 3f;
    public float patrolSpeed = 2f;
    public float patrolCooldown = 5f;//time before finding a new spot to patrol.
    public float chaseSpeed = 10f;
    public float maxVelocity = 10f;
    public float lookSpeed = .1f;
    public float breakingSpeed = .01f;
    public float stunCooldown = 5f;
    public Vector3 patrolLocation;

    protected Rigidbody rb;
    public Transform target;

    protected virtual void Start() { }

	protected virtual void Update() { }

	//Reapply the rigidbody with driller settings, this is super not ideal.
	protected void ReinitializeRidgidBody()
    {
        //rb.isKinematic = false;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = .1f;
        rb.drag = .1f;
        rb.angularDrag = .05f;
    }
}
