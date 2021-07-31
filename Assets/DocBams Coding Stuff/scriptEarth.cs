using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEarth : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;

    public enum states
    {
        Nominal,
        Freezing,
        Warming,
        Dead
    }

    private states currentState;
    private states prevState;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        //Initial state
        SetCurrentState(states.Nominal);
    }

    // Update is called once per frame
    void Update()
    {
        //Death mode
        if (currentState == states.Dead)
            HandleDeath();
    }

    public void HandleDamage(float damageAmount)
	{
        health -= damageAmount;

        if (health <= 0)
            SetCurrentState(states.Dead);
	}

    void HandleDeath()
	{
        //fancy coroutine stuff
        Destroy(gameObject);
	}

    //sets the curent state and  caches the previous state
    public void SetCurrentState(states newState)
    {
        if (newState != currentState)
        {
            prevState = currentState; //cache the previous state
            currentState = newState;
        }
    }
}
