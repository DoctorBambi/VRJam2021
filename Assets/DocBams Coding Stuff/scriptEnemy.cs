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
    public Vector3 startPoint;
    public Vector3 patrolRoute;
    public float patrolAreaSize = 5f;

    public enum states
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Embedded,
        Braking,
        BackingUp,
        Stunned
    }

    public states currentState;
    public states prevState;

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

    //Coroutines
    protected IEnumerator stunRoutine;
    protected IEnumerator brakingRoutine;
    protected IEnumerator patrolRoutine;
    #endregion

    #region MonoBehaviour Stuff
    protected virtual void Start()
    {
        //Initing components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

        aSrc = GetComponent<AudioSource>();
        if (aSrc == null) Debug.LogError("No audio source found for this enemy.");

        startPoint = transform.position;

        //Initial state
        SetCurrentState(states.Patrolling);
    }

	protected virtual void Update() { }

	public virtual void OnCollisionEnter(Collision collision) { }
    #endregion

    #region AI Behaviour
    //sets the curent state and caches the previous state
    public virtual void SetCurrentState(states newState)
    {
        if (newState != currentState)
        {
            prevState = currentState; //cache the previous state
            currentState = newState;
        }
    }

    protected virtual void HandleIdle()
    {
        //Check the status of the earth
        EarthCheck();

        if (target != null)
        {
            SetCurrentState(states.Chasing);
        }
    }

    protected virtual void HandlePatrolling()
    {
        //Check the status of the earth
        EarthCheck();

        if (target != null)
        {
            //Stop the patrol routine if need be
            if (patrolRoutine != null)
            {
                StopCoroutine(patrolRoutine);
                patrolRoutine = null;
            }

            SetCurrentState(states.Chasing);
            return;
        }

        //give patrol commands
        if (patrolRoutine == null)
        {
            patrolRoutine = PatrolRoutine();
            StartCoroutine(patrolRoutine);
        }
    }
    #endregion

    #region External Tools
    /// <summary>
    /// Slow the enemy unit.
    /// </summary>
    /// <param name="amount">The sqrmagnitude of the movement velocity we want to slow to. .0001f is essentially fully stopped.</param>
    /// <param name="returnState">What state of behaviour do we want to enter once we are done braking. Defaults to patrolling.</param>
    public virtual void Brake(states returnState = states.Patrolling, float amount = .0001f)
    {
        if (brakingRoutine == null)
        {
            brakingRoutine = BrakingRoutine(returnState, amount);
            StartCoroutine(brakingRoutine);
        }
    }

    public virtual void Stun()
    {
        if (stunRoutine == null)
        {
            stunRoutine = StunRoutine();
            StartCoroutine(stunRoutine);
        }
    }
	#endregion

	#region Internal Tools
	protected virtual void EarthCheck()
	{
        if (scriptEarth.Instance.currentState != scriptEarth.states.Dead && scriptEarth.Instance.currentState != scriptEarth.states.Safe)
        {
            var dist = Vector3.Distance(transform.position, scriptEarth.Instance.transform.position);

            if (pack != null) //we are a pack member
			{
                if (dist < sightRadius && pack.currentAwareness == scriptEnemyPack.awareness.InTerritory)
				{
                    target = scriptEarth.Instance.transform;
                    //pack.AlertPackMembers(target);
                }
                else if (pack.currentAwareness == scriptEnemyPack.awareness.Alerted)
                    target = scriptEarth.Instance.transform;
                else
                    target = null;
            }
            else //we are a lone wolf
			{
                if (dist < sightRadius)
                    target = scriptEarth.Instance.transform;
                else
                    target = null;
            }
        }
        else
            target = null;
    }

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
	#endregion

	#region Coroutines
	protected virtual IEnumerator StunRoutine()
    {
        SetCurrentState(states.Stunned);

        yield return new WaitForSeconds(stunCooldown); //oo we stunned

        SetCurrentState(states.Patrolling); //get your barings

        stunRoutine = null;
    }

    protected virtual IEnumerator BrakingRoutine(states returnState, float amount)
	{
        //Slow down to a stop
        while (rb != null && (rb.velocity != Vector3.zero || rb.angularVelocity != Vector3.zero))
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, breakingSpeed);
            if (rb.velocity.sqrMagnitude < amount) rb.velocity = Vector3.zero;

            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, breakingSpeed);
            if (rb.angularVelocity.sqrMagnitude < amount) rb.angularVelocity = Vector3.zero;

            yield return null;
        }

        SetCurrentState(returnState);

        brakingRoutine = null;
    }

    protected virtual IEnumerator PatrolRoutine()
    {
        patrolRoute = Random.insideUnitSphere * patrolAreaSize + startPoint;
        Vector3 targetDirection = patrolRoute - transform.position;

        //Look at target
        while ((transform.forward - targetDirection.normalized).sqrMagnitude > .0001f)
		{
            targetDirection = patrolRoute - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, lookSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            //transform.LookAt(Vector3.Lerp(transform.position, patrolRoute, lookSpeed));

            yield return null;
        }

        //Move toward the target
        if (rb != null && rb.velocity.sqrMagnitude < maxVelocity)
            rb.AddForce(transform.forward * patrolSpeed);

        yield return new WaitForSeconds(patrolCooldown);

        //Slow to a stop before heading to your next patrol destination.
        Brake();

        patrolRoutine = null;
    }
    #endregion
}
