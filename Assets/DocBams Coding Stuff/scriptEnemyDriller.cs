using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyDriller : scriptEnemy
{

    public float drillDamage = .1f;

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

    public states currentState;
    private states prevState;

    //Audio
    public AudioClip[] sounds;

    public enum soundTypes
    {
        Idle,
        HighSpeed
    }

    private AudioSource aSrc;

    //Coroutines
    private IEnumerator dislodgeRoutine;
    private IEnumerator patrolRoutine;
    private IEnumerator stunRoutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        //Initing components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

        aSrc = GetComponent<AudioSource>();
        if (aSrc == null) Debug.LogError("No audio source found for this enemy.");

        //Initial state
        SetCurrentState(states.Patrolling);
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Idle Mode
        if (currentState == states.Idle)
        {
            HandleIdle();
        }
        //Patrol mode
        else if (currentState == states.Patrolling)
		{
            HandlePatrolling();
		}
        //Chase Mode
        else if (currentState == states.Chasing)
		{
            HandleChasing();
        }
        //Braking Mode
        else if (currentState == states.Braking)
		{
            HandleBraking();
		}
        //Embedded Mode
        else if (currentState == states.Embedded)
		{
            HandleEmbed();
		}
        //Stun Mode
        else if (currentState == states.Stunned)
		{
            HandleStun();
		}

        //Handle Audio settings
        HandleAudio();
    }

    void HandleIdle()
	{
        //For now we're just chillin
        target = null;
	}

    void HandlePatrolling()
	{
        //give patrol commands
        if (patrolRoutine == null)
        {
            patrolRoutine = PatrolRoutine();
            StartCoroutine(patrolRoutine);
        }

        //Check if we sense the player and they're still alive.
        if (pack.earth.GetComponent<scriptEarth>().currentState != scriptEarth.states.Dead && pack.earth.GetComponent<scriptEarth>().currentState != scriptEarth.states.Safe)
		{
            var dist = Vector3.Distance(transform.position, pack.earth.transform.position);
            if (dist < sightRadius)
            {
                pack.AlertPackMembers(pack.earth.transform);
            }
        }
    }

    void HandleChasing()
	{
        //Target the earth
        //if (earth != null)
        //    target = earth.transform;

        if (target != null)
		{
            //Look at target
            transform.LookAt(Vector3.Lerp(transform.position, target.position, lookSpeed));

            //Move toward the target
            if (rb != null && rb.velocity.sqrMagnitude < maxVelocity)
                rb.AddForce(transform.forward * chaseSpeed);
		}
    }

    void HandleBraking()
	{
        //Slow down to a stop
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, breakingSpeed);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, breakingSpeed);

        if (rb.velocity == Vector3.zero) //then we've finished braking so go back to whatever we were doing
        {
            SetCurrentState(prevState); //return to task being performed before braking.
        }
    }

    //We've embedded ourselves into an object, drill into it until it is pulverized
    void HandleEmbed()
	{
        transform.parent.SendMessage("HandleDamage", drillDamage, SendMessageOptions.DontRequireReceiver);
    }

    void HandleStun()
	{
        //Just let the ship drift from whatever chaos stunned it.
        target = null;
    }

	private void OnCollisionEnter(Collision collision)
	{
        //print(collision.collider.name);

        //general collisions
        if (collision.transform.name.Contains("Hand"))//if we collide with player's hand
		{
            Stun();
		}

        //chase contextual collisions
        if (currentState == states.Chasing)
		{
            if (collision.collider.CompareTag("Asteroid"))
            {
                Obliterate(collision.collider.gameObject);
            }
            else if (collision.collider.CompareTag("Earth"))
            {
                EmbedInto(collision.collider.gameObject);
            }
        }
	}

    public void AlertUnit(Transform newTarget)
    {
        target = newTarget;
        SetCurrentState(scriptEnemyDriller.states.Chasing);
    }

    public void RetreatUnit()
    {
        target = null;
        SetCurrentState(scriptEnemyDriller.states.Patrolling);
    }

    //pulverize a thing into dust
    private void Obliterate(GameObject target)
	{
        //Likely will activate a coroutine that does lots of flashy stuff.
        SetCurrentState(states.Braking);
        Destroy(target);
	}

    //embed the driller into the target
    private void EmbedInto(GameObject target)
	{
        currentState = states.Embedded;

        Destroy(rb);
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //rb.isKinematic = true;

        transform.SetParent(target.transform, true);
        target.SendMessage("EmbedInto", gameObject, SendMessageOptions.DontRequireReceiver);
    }

    //unparents the driller and allows it to move again
    public void DislodgeFrom()
	{
        if (dislodgeRoutine == null)
        {
            dislodgeRoutine = DislodgeRoutine();
            StartCoroutine(dislodgeRoutine);
        }
    }

    public void Stun()
	{
        if (stunRoutine == null)
        {
            stunRoutine = StunRoutine();
            StartCoroutine(stunRoutine);
        }
	}

    //sets the curent state and caches the previous state
    public void SetCurrentState(states newState)
	{
        if (newState != currentState)
		{
            prevState = currentState; //cache the previous state
            currentState = newState;
        }
	}

    //Audio
    void HandleAudio()
	{
		switch (currentState)
		{
            case states.Chasing:
                aSrc.clip = sounds[(int)soundTypes.HighSpeed];
                if (aSrc.isPlaying == false) aSrc.Play();
                break;
            default:
                aSrc.clip = sounds[(int)soundTypes.Idle];
                if (aSrc.isPlaying == false) aSrc.Play();
                break;
		}

        if (rb != null)
        {
            var curVel = Mathf.Clamp(rb.velocity.sqrMagnitude, 0, maxVelocity);
            var ratio = ((curVel / maxVelocity) * 2) + 1; //* 2 + 1 because pitch can go from 1 to 3.

            aSrc.pitch = Mathf.Lerp(aSrc.pitch, ratio, .1f);
        }
        else
            aSrc.pitch = Mathf.Lerp(aSrc.pitch, 1, .1f);
        
	}

    //Coroutines
    private IEnumerator DislodgeRoutine()
    {
        SetCurrentState(states.Idle);
        target = null;
        transform.SetParent(null, true);//set parent to the scene

        ReinitializeRidgidBody();

        rb.AddForce(-transform.forward * chaseSpeed);//back the enemy away from embeded target

        yield return new WaitForSeconds(2f); //give the enemy time to back away from asteroid

        SetCurrentState(states.Patrolling);

        dislodgeRoutine = null;
    }

    private IEnumerator PatrolRoutine()
    {
        patrolLocation = Random.insideUnitSphere * pack.patrolAreaSize + pack.packStartPoint.position;

        //Look at target
        transform.LookAt(Vector3.Lerp(transform.position, patrolLocation, lookSpeed));

        //Move toward the target
        if (rb != null && rb.velocity.sqrMagnitude < maxVelocity)
            rb.AddForce(transform.forward * patrolSpeed);

        yield return new WaitForSeconds(patrolCooldown);

        patrolRoutine = null;
    }

    private IEnumerator StunRoutine()
    {
        SetCurrentState(states.Stunned);

        yield return new WaitForSeconds(stunCooldown); //oo we stunned

        SetCurrentState(prevState);

        stunRoutine = null;
    }
}
