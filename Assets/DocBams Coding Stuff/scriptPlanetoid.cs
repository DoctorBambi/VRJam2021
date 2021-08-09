using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scriptPlanetoid : MonoBehaviour
{
    #region Properties
    public bool debugging = false;
	public float maxHealth = 100;
    public float health;
    public bool hasDied = false;
    public GameObject explosionEffect;

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

    protected List<GameObject> attachedEntities = new List<GameObject>();
    protected Collider col;

    //Audio
    protected AudioSource aSrc;

    //Coroutines
    protected IEnumerator dyingRoutine;
    protected IEnumerator safeRoutine;
    #endregion

    #region MonoBehaviour Stuff
    // Start is called before the first frame update
    protected virtual void Start()
    {
        aSrc = GetComponent<AudioSource>();
        if (aSrc == null) Debug.LogError("No audio source found on planetoid.", gameObject);

        col = GetComponent<Collider>();
        if (col == null) Debug.LogError("No collider found on planetoid.", gameObject);

        if (explosionEffect == null) Debug.LogError("No explosion effect found on planetoid.", gameObject);

        //Initial state
        health = maxHealth;
        SetCurrentState(states.Nominal);
    }

    // Update is called once per frame
    protected virtual void Update() { }

    protected virtual void OnTriggerEnter(Collider other) { }
    #endregion

    #region AI Behaviours
    //sets the curent state and caches the previous state
    public virtual void SetCurrentState(states newState)
    {
        if (newState != currentState)
        {
            prevState = currentState; //cache the previous state
            currentState = newState;
        }
    }

    protected virtual void HandleDeath()
    {
        if (dyingRoutine == null && hasDied == false)
        {
            dyingRoutine = DyingRoutine();
            StartCoroutine(dyingRoutine);
        }
    }

    protected virtual void HandleSafe()
    {

        if (safeRoutine == null)
        {
            safeRoutine = SafeRoutine();
            StartCoroutine(safeRoutine);
        }
    }
    #endregion

    #region External Inputs
    public virtual void HandleDamage(float damageAmount)
    {
        Debug.LogWarning("The planetoid has not set up a damage handler.");
    }

    public virtual void EmbedInto(GameObject thingToEmbed)
    {
        attachedEntities.Add(thingToEmbed);
    }
    #endregion

    #region Coroutines
    protected virtual IEnumerator DyingRoutine()
    {
        Debug.LogWarning("The planetoid has not set up a dying routine.");

        yield return new WaitForSeconds(0);

        dyingRoutine = null;
    }

    protected virtual IEnumerator SafeRoutine()
    {
        Debug.LogWarning("The planetoid has not set up a safe routine.");

        yield return new WaitForSeconds(0);

        safeRoutine = null;
    }
	#endregion
}
