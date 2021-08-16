using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyDriller : scriptEnemy
{
	#region Properties
	public float drillDamage = .1f;
	public float dashCooldown = 4f;
	private float dashRange = 2;

	//Coroutines
	private IEnumerator dislodgeRoutine;
	private IEnumerator dashRoutine;
	#endregion

	#region MonoBehaviour Stuff
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
		//Embedded Mode
		else if (currentState == states.Embedded)
		{
			HandleEmbed();
		}

		//Handle Audio settings
		HandleAudio();
	}

	public override void OnCollisionEnter(Collision collision)
	{
		if (debugging) print($"Driller Collide: {collision.collider.name}");

		//general collisions
		if (collision.transform.name.Contains("Hand") || collision.transform.name == "prefabStungun")//if we collide with player's hand or the stun gun
		{
			if (currentState == states.Embedded)
				DislodgeFrom();
			else
				Stun();
		}

		//behaviour contextual collisions
		if (currentState == states.Chasing || currentState == states.Dashing)
		{
			if (collision.collider.CompareTag("Asteroid"))
			{
				EmbedInto(collision.collider.gameObject);
			}
			else if (collision.collider.CompareTag("Earth"))
			{
				EmbedInto(collision.collider.gameObject);
			}
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (debugging) print($"Driller Trigger: {other.name}");
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

			//Move toward the target
			if (rb.velocity.sqrMagnitude < maxVelocity)
				rb.AddForce(transform.forward * chaseSpeed);
			else
				Brake(states.Chasing, maxVelocity - 1);

			//Check if we're close enough to dash at the target
			if ((transform.position - target.position).sqrMagnitude < dashRange)
			{
				Dash();
			}
		}
		else
			SetCurrentState(states.Patrolling);
	}

	//We've embedded ourselves into an object, drill into it until it is pulverized
	void HandleEmbed()
	{
		transform.parent.SendMessage("HandleDamage", drillDamage, SendMessageOptions.DontRequireReceiver);
	}
	#endregion

	#region External Inputs
	//embed the driller into the target
	private void EmbedInto(GameObject target)
	{
		SetCurrentState(states.Embedded);

		Destroy(rb);

		//TODO: make this a separate function and store routines in a list so we can dynamically kill everything easily.
		StopAllCoroutines();
		dislodgeRoutine = null;
		stunRoutine = null;
		brakingRoutine = null;
		patrolRoutine = null;
		dashRoutine = null;

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

	public void Dash()
	{
		if (dashRoutine == null)
		{
			dashRoutine = DashRoutine();
			StartCoroutine(dashRoutine);
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
	private IEnumerator DislodgeRoutine()
	{
		//Play audio


		SetCurrentState(states.Idle);
		target = null;

		transform.SetParent(null, true);//de-parent the driller
		ReinitializeRidgidBody();

		rb.AddForce(-transform.forward * chaseSpeed);//back the enemy away from embeded target

		yield return new WaitForSeconds(2f);//give the enemy time to back away from asteroid

		SetCurrentState(states.Patrolling);

		dislodgeRoutine = null;
	}

	private IEnumerator DashRoutine()
	{
		//Play audio


		SetCurrentState(states.Dashing);

		if (rb != null) rb.AddForce(transform.forward * (chaseSpeed * 10));//dash toward the target

		yield return new WaitForSeconds(dashCooldown);

		SetCurrentState(states.Chasing);

		dashRoutine = null;
	}
	#endregion
}
