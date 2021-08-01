using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyHunter : scriptEnemy
{
	public float shootingRange = 2f; // how close does it need to be to shoot at earth
	public float shootCooldown = 2f;
	public int bulletLifespan = 5;
	public GameObject ammoType;

	public enum states
	{
		Idle,
		Patrolling,
		Chasing,
		Attacking,
		Braking,
		Stunned
	}

	public states currentState;
	private states prevState;

	public enum shotTypes
	{
		Single,
		Burst,
		Charged
	}

	private shotTypes shotType;

	private GameObject lazerShot;
	private Transform bulletChamber; //where bullets are loaded for firing

	//Audio
	public AudioClip[] sounds;

	public enum soundTypes
	{
		Idle,
		HighSpeed
	}

	private AudioSource aSrc;

	//Coroutines
	private IEnumerator patrolRoutine;
	private IEnumerator stunRoutine;
	private IEnumerator shootRoutine;

	// Start is called before the first frame update
	protected override void Start()
	{
		//Initing components
		rb = GetComponent<Rigidbody>();
		if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

		aSrc = GetComponent<AudioSource>();
		if (aSrc == null) Debug.LogError("No audio source found for this enemy.");

		if (ammoType == null) Debug.LogError("No ammo type assigned for this enemy.");

		bulletChamber = transform.Find("BulletChamber");
		if (bulletChamber == null) Debug.LogError("No bullet chamber found for this enemy.");

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
		//Attack Mode
		else if (currentState == states.Attacking)
		{
			HandleAttacking();
		}
		//Braking Mode
		else if (currentState == states.Braking)
		{
			HandleBraking();
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

		//Check if we sense the player.
		var dist = Vector3.Distance(transform.position, pack.earth.transform.position);
		if (dist < sightRadius)
		{
			pack.AlertPackMembers(pack.earth.transform);
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

			//Cast a ray and see what's ahead of us
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRange))
			{
				if (hit.collider.name == "Earth") //then we're in range so start attacking
				{
					SetCurrentState(states.Attacking);
				}
				else //some other obstacle is in the way so try to get around it.
				{
					if (rb != null)
						rb.AddForce(transform.right * chaseSpeed);
				}
			}
			else //Clear skies so carry on
			{
				if (rb != null && rb.velocity.sqrMagnitude < maxVelocity)
					rb.AddForce(transform.forward * chaseSpeed);
			}
		}
	}

	void HandleAttacking()
	{
		if (target != null)
		{
			//Look at target
			transform.LookAt(Vector3.Lerp(transform.position, target.position, lookSpeed));

			//Check that player is still in range
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRange + 1))// + 1 so we have a cushion of space to start shooting if the player is moving the earth quickly.
			{
				if (hit.collider.name == "Earth") //then we're in range so start shooting
				{
					//Slow down to a stop
					rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, breakingSpeed);
					rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, breakingSpeed);

					//Shoot at earth
					if (shootRoutine == null)
					{
						shootRoutine = ShootRoutine(shotType);
						StartCoroutine(shootRoutine);
					}
				}
			}
			else
			{
				SetCurrentState(states.Chasing);
			}
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

	void HandleStun()
	{
		//Just let the ship drift from whatever chaos stunned it.
		target = null;
	}

	private void OnCollisionEnter(Collision collision)
	{
		print(collision.collider.name);

		//general collisions
		if (collision.transform.name.Contains("Hand"))//if we collide with player's hand
		{
			Stun();
		}
	}

	public void AlertUnit(Transform newTarget)
	{
		target = newTarget;
		SetCurrentState(scriptEnemyHunter.states.Chasing);
	}

	//pulverize a thing into dust
	private void Obliterate(GameObject target)
	{
		//Likely will activate a coroutine that does lots of flashy stuff.
		SetCurrentState(states.Braking);
		Destroy(target);
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
			case states.Attacking:
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

		yield return new WaitForSeconds(stunCooldown);

		SetCurrentState(prevState);

		stunRoutine = null;
	}

	private IEnumerator ShootRoutine(shotTypes shot)
	{
		switch (shot)
		{
			case shotTypes.Single:
				//Instantiate lazer and fire at earth
				var inst = Instantiate(ammoType, bulletChamber.position, bulletChamber.rotation);
				//inst.transform.position = lazerShot.transform.position;
				//inst.transform.rotation = lazerShot.transform.rotation;
				//inst.transform.localScale = lazerShot.transform.localScale;
				inst.SetActive(true);
				inst.GetComponent<AudioSource>().Play();
				inst.GetComponent<scriptLazerShot>().isFiring = true;
				Destroy(inst, bulletLifespan);
				break;
			default:
				Debug.LogWarning($"Shot type {shot} is not implemented yet.");
				break;
		}

		yield return new WaitForSeconds(shootCooldown);

		shootRoutine = null;
	}
}
