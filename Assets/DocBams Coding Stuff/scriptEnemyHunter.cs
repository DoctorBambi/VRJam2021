using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyHunter : scriptEnemy
{
	#region Properties
	public float shootingRangeMax = 2f; // how close does it need to be to shoot at earth
	public float shootingRangeMin = 1f; // how far away does it need to be to shoot at earth
	public float shootCooldown = 2f;
	public int bulletLifespan = 5;
	public GameObject ammoType;

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
	#endregion

	#region MonoBehaviour Stuff
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

	private void OnCollisionEnter(Collision collision)
	{
		//print(collision.collider.name);

		//general collisions
		if (collision.transform.name.Contains("Hand") || collision.transform.name == "prefabStungun")//if we collide with player's hand
		{
			Stun();
		}
	}
	#endregion

	#region AI Behaviours
	//sets the curent state and caches the previous state
	public void SetCurrentState(states newState)
	{
		if (newState != currentState)
		{
			prevState = currentState; //cache the previous state
			currentState = newState;
		}
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

		if (pack.packAlerted) //then get back in the action.
		{
			target = pack.earth.transform;
			SetCurrentState(states.Chasing);
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
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRangeMax))
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
		else
			Debug.Log("I have no target to chase.");
	}

	void HandleAttacking()
	{
		if (target != null)
		{
			//Look at target
			transform.LookAt(Vector3.Lerp(transform.position, target.position, lookSpeed));

			//Check that player is still in range
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRangeMax + 1))// + 1 so we have a cushion of space to start shooting if the player is moving the earth quickly.
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
			SetCurrentState(states.Patrolling); //start patrolling to get your barings.
		}
	}

	void HandleStun()
	{
		//Just let the ship drift from whatever chaos stunned it.
		target = null;
	}
	#endregion

	#region External Inputs
	public void AlertUnit(Transform newTarget)
	{
		target = newTarget;
		SetCurrentState(scriptEnemyHunter.states.Chasing);
	}

	public void RetreatUnit()
	{
		target = null;
		SetCurrentState(scriptEnemyHunter.states.Patrolling);
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
	#endregion

	#region Audio
	void HandleAudio()
	{
		switch (currentState)
		{
			case states.Chasing:
				aSrc.clip = sounds[(int)soundTypes.HighSpeed];
				if (aSrc.isPlaying == false) aSrc.Play();

				//Change engine hum pitch based on current velocity
				scriptAudioManager.Instance.AdjustPitchBasedVelocity(aSrc, rb, maxVelocity, 2, 1);
				break;
			case states.Stunned:
				aSrc.clip = sounds[(int)soundTypes.Idle];
				if (aSrc.isPlaying == false) aSrc.Play();

				aSrc.pitch = Mathf.Lerp(aSrc.pitch, 1, .2f);
				break;
			default:
				aSrc.clip = sounds[(int)soundTypes.Idle];
				if (aSrc.isPlaying == false) aSrc.Play();
				break;
		}
	}
	#endregion

	#region Coroutines
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

		SetCurrentState(states.Patrolling);

		stunRoutine = null;
	}

	private IEnumerator ShootRoutine(shotTypes shot)
	{
		switch (shot)
		{
			case shotTypes.Single:
				//Instantiate lazer and fire at earth
				var inst = Instantiate(ammoType, bulletChamber.position, bulletChamber.rotation);
				
				inst.SetActive(true);
				inst.GetComponent<AudioSource>().Play();

				var sls = inst.GetComponent<scriptLazerShot>();
				if (sls == null)
					Debug.LogError("No scriptLazerShot found on this ammoType.", ammoType);

				sls.isFiring = true;
				Destroy(inst, bulletLifespan);
				break;
			default:
				Debug.LogWarning($"Shot type {shot} is not implemented yet.");
				break;
		}

		yield return new WaitForSeconds(shootCooldown);

		shootRoutine = null;
	}
	#endregion
}
