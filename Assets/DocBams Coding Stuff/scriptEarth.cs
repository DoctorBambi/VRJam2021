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

    private List<GameObject> attachedEnemies = new List<GameObject>();

    //Coroutines
    private IEnumerator dyingRoutine;

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

    public void EmbedInto(GameObject thingToEmbed)
	{
        attachedEnemies.Add(thingToEmbed);
	}

    void HandleDeath()
	{
        if (dyingRoutine == null)
		{
            dyingRoutine = DyingRoutine();
            StartCoroutine(dyingRoutine);
		}
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

    //Coroutines
    private IEnumerator DyingRoutine()
    {
        //Handle attached enemies
        foreach (GameObject go in attachedEnemies)
        {
            go.SendMessage("DislodgeFrom", SendMessageOptions.DontRequireReceiver);
        }
        attachedEnemies.Clear();

        yield return new WaitForSeconds(.25f); //give the enemies time to run their dislodge scripts

        //Destroy the planet ;.;
        Destroy(gameObject);

        dyingRoutine = null;
    }
}
