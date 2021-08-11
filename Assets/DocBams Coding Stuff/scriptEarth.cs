using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scriptEarth : scriptPlanetoid
{
    static public scriptEarth Instance;

    #region Properties

    public TextMeshProUGUI deathTextUI;
    public string deathText;

    private GameObject model;

    #endregion

    #region MonoBehaviour Stuff
    protected override void Start()
    {
        base.Start();

        model = transform.Find("EarthContainer").gameObject;

        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Earth already exists in the scene!", Instance);
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Death mode
        if (currentState == states.Dead)
            HandleDeath();
        //Safe mode
        else if (currentState == states.Safe)
            HandleSafe();
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (debugging) print($"Earth collide: {collision.collider.name}");

        //If we hit a driller that is currently embedded in the earth, knock it free.
        var driller = collision.contacts[0].thisCollider.GetComponentInParent<scriptEnemyDriller>();

        if (driller != null) driller.DislodgeFrom();
    }

	protected override void OnTriggerEnter(Collider other)
	{
        if (debugging) print($"Earth trigger: {other.name}");
        
        if (other.CompareTag("Sun"))
		{
            SetCurrentState(states.Safe);
		}
	}
    #endregion

    #region AI Behaviours
    protected override void HandleDeath()
    {
        if (dyingRoutine == null && hasDied == false)
        {
            dyingRoutine = DyingRoutine();
            StartCoroutine(dyingRoutine);
        }
    }
    #endregion

    #region External Inputs
    public override void HandleDamage(float damageAmount)
	{
        if (debugging) print("Earth has been hit!");

        health -= damageAmount;

        if (health <= 0)
		{
            //SceneManager.LoadScene(2);
            deathText = "EARTH HAS DIED";
            SetCurrentState(states.Dead);
		}
	}

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Coroutines
    protected IEnumerator DyingRoutine()
    {
        //Handle attached enemies
        foreach (GameObject go in attachedEntities)
        {
            if (go != null) go.SendMessage("DislodgeFrom", SendMessageOptions.DontRequireReceiver);
        }
        attachedEntities.Clear();

        //Disable renderer and collider
        model.SetActive(false);
        col.enabled = false;

        //Play Audio


        //Spawn explosion effect
        var dustCloud = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(dustCloud, 10f);

        //Activate the death text
        if (deathTextUI != null)
		{
            deathTextUI.text = deathText;
            deathTextUI.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(scriptAudioManager.Instance.musicLoops[(int)scriptAudioManager.vibes.Dead].length); //wait for the death music to finish before reloading.

        //Reload the scene
        Invoke("ReloadScene", 1f);

        hasDied = true;
        dyingRoutine = null;
    }

    protected override IEnumerator SafeRoutine()
    {
        //Handle attached enemies
        foreach (GameObject go in attachedEntities)
        {
            go.SendMessage("DislodgeFrom", SendMessageOptions.DontRequireReceiver);
        }
        attachedEntities.Clear();

        yield return new WaitForSeconds(scriptAudioManager.Instance.musicLoops[(int)scriptAudioManager.vibes.Victory].length); //wait for the victory music to finish because reasons?

        //... Now what do we do?

        safeRoutine = null;
    }
	#endregion
}
