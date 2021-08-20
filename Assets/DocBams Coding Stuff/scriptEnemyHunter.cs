using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyHunter : scriptEnemy
{
	#region Properties
	public float shootingRangeMax = 2f; // furthest away it can shoot at earth
	public float shootingRangeMin = 1f; // how close it can get before backing up
	public float shootCooldown = 2f;
	public float burstCooldown = .2f;
	public int bulletLifespan = 5;
	public GameObject ammoType;

	public enum shotTypes
	{
		Single,
		Burst3,
		Burst5,
		Charged
	}

	public shotTypes shotType;

	private Transform bulletChamber; //where bullets are loaded for firing

	//Coroutines
	private IEnumerator shootRoutine;
	#endregion

	#region MonoBehaviour Stuff
	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		if (ammoType == null) Debug.LogError("No ammo type assigned for this enemy.");

		bulletChamber = transform.Find("BulletChamber");
		if (bulletChamber == null) Debug.LogError("No bullet chamber found for this enemy.");
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
		//Backing Up Mode
		else if (currentState == states.BackingUp)
		{
			HandleBackingUp();
		}

		//Handle Audio settings
		HandleAudio();
	}

	public override void OnCollisionEnter(Collision collision)
	{
		//print(collision.collider.name);

		//general collisions
		if (collision.transform.name.Contains("Hand") || collision.transform.name == "prefabStungun")//if we collide with player's hand or stun gun
		{
			Stun();
		}
	}
	#endregion

	#region AI Behaviours
	void HandleChasing()
	{
		//Check the status of the earth
		EarthCheck();

		if (target != null && rb != null)
		{
			//Look at target
			Vector3 targetDirection = target.position - transform.position;
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, chaseLookSpeed * Time.deltaTime, 0.0f);
			rb.MoveRotation(Quaternion.LookRotation(newDirection));

			//Cast a ray and see what's ahead of us
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRangeMax))
			{
				if (hit.collider.CompareTag("Earth")) //then we're getting ready to attack
				{
					if (hit.distance < shootingRangeMin)
					{ //then we're too close so back up
						SetCurrentState(states.BackingUp);
					}
					else //we're just right, so start attacking
						SetCurrentState(states.Attacking);
				}
				else //some other obstacle is in the way so try to get around it.
				{
					rb.AddForce(transform.right * strafingSpeed);
				}
			}
			else //Clear skies so carry on
			{
				if (rb.velocity.sqrMagnitude < maxVelocity)
					rb.AddForce(transform.forward * chaseSpeed);
				else
					Brake(states.Chasing, maxVelocity - 1);
			}
		}
		else
			SetCurrentState(states.Patrolling);
	}

	void HandleAttacking()
	{
		//Check the status of the earth
		EarthCheck();

		if (target != null && rb != null)
		{
			//Look at target
			Vector3 targetDirection = target.position - transform.position;
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, chaseLookSpeed * Time.deltaTime, 0.0f);
			rb.MoveRotation(Quaternion.LookRotation(newDirection));


			//Check that player is still in range
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRangeMax))
			{
				if (hit.collider.CompareTag("Earth")) //then we're in range so start shooting
				{
					//Slow down to a stop
					rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, brakingSpeed);
					rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, brakingSpeed);

					//Shoot at earth
					if (shootRoutine == null)
					{
						shootRoutine = ShootRoutine(shotType);
						StartCoroutine(shootRoutine);
					}
				}
				else //navigate around the object occluding the earth
					SetCurrentState(states.Chasing);
			}
			else
				SetCurrentState(states.Chasing);
		}
		else
			SetCurrentState(states.Patrolling);
	}

	void HandleBackingUp()
	{
		//Check the status of the earth
		EarthCheck();

		if (target != null && rb != null)
		{
			if (shootRoutine == null) //If we're in the middle of a shoot pattern, let that play out. This will allow the bullets to flow in a nice line.
			{
				//Look at target
				Vector3 targetDirection = target.position - transform.position;
				Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, chaseLookSpeed * Time.deltaTime, 0.0f);
				rb.MoveRotation(Quaternion.LookRotation(newDirection));

				//Check that target is still in range
				if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, shootingRangeMax))
				{
					if (hit.collider.CompareTag("Earth")) //then we're in range so start shooting
					{
						if (hit.distance < shootingRangeMin) //then we're too close so back up
						{
							if (rb.velocity.sqrMagnitude < maxVelocity) rb.AddForce(-transform.forward * backUpSpeed);

							if (rb.velocity.sqrMagnitude > maxVelocity) Brake(states.Attacking);
						}
						else //we're just right, so attack
							SetCurrentState(states.Attacking);

						//We can still shoot while backing up, so shoot at target
						if (shootRoutine == null)
						{
							shootRoutine = ShootRoutine(shotType);
							StartCoroutine(shootRoutine);
						}
					}
					else //navigate around the object occluding the earth
						SetCurrentState(states.Chasing);
				}
				else
					SetCurrentState(states.Chasing);
			}
		}
		else
			SetCurrentState(states.Patrolling);
	}
	#endregion

	#region External Inputs
	public void SpawnBullet()
	{
		//Instantiate lazer and fire at earth
		var inst = Instantiate(ammoType, bulletChamber.position, bulletChamber.rotation);

		inst.SetActive(true);
		inst.GetComponent<AudioSource>().Play();

		var sls = inst.GetComponent<scriptLazerShot>();
		if (sls == null)
			Debug.LogError("No scriptLazerShot found on this ammoType.", ammoType);

		sls.isFiring = true;
		Destroy(inst, bulletLifespan);
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

	private IEnumerator ShootRoutine(shotTypes shot)
	{
		switch (shot)
		{
			case shotTypes.Single:
				SpawnBullet();
				break;
			case shotTypes.Burst3:
				for (var i = 0; i < 3; i++)
				{
					SpawnBullet();

					yield return new WaitForSeconds(burstCooldown);
				}
				break;
			case shotTypes.Burst5:
				for (var i = 0; i < 5; i++)
				{
					SpawnBullet();

					yield return new WaitForSeconds(burstCooldown);
				}
				break;
			default:
				Debug.LogWarning($"Shot type {shot} is not implemented yet. Defaulting shot type to Single.");

				SpawnBullet();

				shotType = shotTypes.Single;
				break;
		}

		yield return new WaitForSeconds(shootCooldown);

		shootRoutine = null;
	}
	#endregion
}
