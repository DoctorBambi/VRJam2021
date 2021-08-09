using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyDriller : scriptEnemy
{
	#region Properties
	public float drillDamage = .1f;

	//Coroutines
	private IEnumerator dislodgeRoutine;
	private IEnumerator patrolRoutine;
	private IEnumerator stunRoutine;
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
		if (currentState == states.Chasing)
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

		if (pack.packAlerted)
		{
			target = pack.earth.transform;
			SetCurrentState(states.Chasing);
		}
	}

	void HandleChasing()
	{
		if (target != null)
		{
			//Look at target
			transform.LookAt(Vector3.Lerp(transform.position, target.position, lookSpeed));

			//Move toward the target
			if (rb != null && rb.velocity.sqrMagnitude < maxVelocity)
				rb.AddForce(transform.forward * chaseSpeed);
		}
		else
			Debug.Log("I have no target to chase.", gameObject);
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
	#endregion

	#region External Inputs
	/// <summary>
	/// Used by the pack to alert this unit to an target's location.
	/// </summary>
	/// <param name="newTarget"></param>
	public void AlertUnit(Transform newTarget)
	{
		target = newTarget;
		SetCurrentState(scriptEnemyDriller.states.Chasing);
	}

	/// <summary>
	/// When the player has left a pack's territory, the pack will use this to call off their attack mode.
	/// </summary>
	public void RetreatUnit()
	{
		target = null;
		SetCurrentState(scriptEnemyDriller.states.Patrolling);
	}

	//embed the driller into the target
	private void EmbedInto(GameObject target)
	{
		currentState = states.Embedded;

		Destroy(rb);

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

		SetCurrentState(states.Patrolling); //get your barings

		stunRoutine = null;
	}
	#endregion
}
