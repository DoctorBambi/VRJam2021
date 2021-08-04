using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scriptEarth : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;
    public float enemyTerritoryDist = 5f;

    public enum states
    {
        Nominal,
        Freezing,
        Warming,
        Dead,
        Safe
    }

    public states currentState;
    public states prevState;

    private List<GameObject> attachedEnemies = new List<GameObject>();

    //Audio
    //Don't think we need these as the audio manager will play the death sound when needed.
    //[SerializeField]
    //AudioClip deathMusic;

    //[SerializeField]
    //AudioSource audioSource;

    //Coroutines
    private IEnumerator dyingRoutine;
    private IEnumerator safeRoutine;

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
        //Safe mode
        else if (currentState == states.Safe)
            HandleSafe();

        //Handle Proximity to enemies
        else
            HandleProximity();
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Sun"))
		{
            SetCurrentState(states.Safe);
		}
	}

	public void HandleDamage(float damageAmount)
	{
        print("Earth has been hit!");

        health -= damageAmount;

        if (health <= 0)
		{
			//SceneManager.LoadScene(2);
            SetCurrentState(states.Dead);
		}
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

    void HandleSafe()
	{

        if (safeRoutine == null)
		{
            safeRoutine = SafeRoutine();
            StartCoroutine(safeRoutine);
		}
    }

    void HandleProximity()
	{

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

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        var aSrc = GetComponent<AudioSource>();

        yield return new WaitForSeconds(aSrc.clip.length); //wait for the death music to finish before reloading.

        //Destroy the planet ;.;
        //Destroy(gameObject);
        //gameObject.GetComponent<Renderer>().enabled = false;

        //Reload the scene
        Invoke("ReloadScene", 1f);

        dyingRoutine = null;
    }

    private IEnumerator SafeRoutine()
    {
        //Handle attached enemies
        foreach (GameObject go in attachedEnemies)
        {
            go.SendMessage("DislodgeFrom", SendMessageOptions.DontRequireReceiver);
        }
        attachedEnemies.Clear();

        var aSrc = GetComponent<AudioSource>();

        yield return new WaitForSeconds(aSrc.clip.length); //wait for the death music to finish before reloading.

        //Destroy the planet ;.;
        //Destroy(gameObject);
        //gameObject.GetComponent<Renderer>().enabled = false;

        //... Now what?
        //Invoke("ReloadScene", 0f);

        safeRoutine = null;
    }
}
