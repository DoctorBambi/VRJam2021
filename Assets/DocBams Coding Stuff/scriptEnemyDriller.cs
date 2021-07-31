using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyDriller : MonoBehaviour
{
    public GameObject earth;
    public float sightRadius = 50f;
    public float movementSpeed = 10f;
    public float maxVelocity = 10f;
    public float lookSpeed = .1f;
    public float breakingSpeed = .01f;
    public float drillDamage = .1f;

    public enum states
    {
        Idle,
        Chasing,
        Attacking,
        Embedded,
        Braking,
        Stunned
    }

    private states currentState;
    private states prevState;

    private Rigidbody rb;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        //Initing components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

        //Initial state
        SetCurrentState(states.Chasing);
    }

    // Update is called once per frame
    void Update()
    {
        //Idle Mode
        if (currentState == states.Idle)
        {
            HandleIdle();
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
    }

    void HandleIdle()
	{
        //For now we're just chillin
        target = null;
	}

    void HandleChasing()
	{
        //Target the earth
        if (earth != null)
            target = earth.transform;

        //Look at target
        transform.LookAt(Vector3.Lerp(transform.position, target.position, lookSpeed));

        //Move toward the target
        if (rb.velocity.sqrMagnitude < maxVelocity)
            rb.AddForce(transform.forward * movementSpeed);
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

    //Weve embedded ourselves into an object, drill into it until its pulverized
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
        print(collision.collider.name);

        if (collision.collider.CompareTag("Asteroid"))
        {
            Obliterate(collision.collider.gameObject);
		}
        else if (collision.collider.CompareTag("Earth"))
		{
            EmbedInto(collision.collider.gameObject);
		}
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
	}

    //unparents the driller and allows it to move again
    public void DislodgeFrom(GameObject target)
	{
        currentState = states.Idle;
        //rb.isKinematic = false;
        gameObject.AddComponent<Rigidbody>();
        rb.AddForce(-transform.forward * movementSpeed);//back the enemy away from embeded target

        transform.SetParent(null, true);//need to set to the scene
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
}
