using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemy : MonoBehaviour
{
    #region Properties
    public bool debugging = false;

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

    public enum states
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Embedded,
        Braking,
        Stunned
    }

    protected states currentState;
    protected states prevState;

    protected Rigidbody rb;
    public Transform target;

    //Audio
    public AudioClip[] sounds;

    public enum soundTypes
    {
        Idle,
        HighSpeed
    }

    protected AudioSource aSrc;
	#endregion

	#region MonoBehaviour Stuff
	protected virtual void Start()
    {
        //Initing components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

        aSrc = GetComponent<AudioSource>();
        if (aSrc == null) Debug.LogError("No audio source found for this enemy.");

        //Initial state
        SetCurrentState(states.Patrolling);
    }

	protected virtual void Update() { }

	public virtual void OnCollisionEnter(Collision collision) { }
    #endregion

    #region AI Behaviour
    //sets the curent state and caches the previous state
    public void SetCurrentState(states newState)
    {
        if (newState != currentState)
        {
            prevState = currentState; //cache the previous state
            currentState = newState;
        }
    }
    #endregion

    //Reapply the rigidbody
    protected virtual void ReinitializeRidgidBody(float mass = .1f, float drag = .1f, float angularDrag = .05f)
    {
        //rb.isKinematic = false;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
}
