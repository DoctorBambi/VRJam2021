using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scriptEarth : scriptPlanetoid
{
    #region Properties

    GameObject model;

    #endregion

    #region MonoBehaviour Stuff
    protected override void Start()
    {
        base.Start();

        model = transform.Find("EarthContainer").gameObject;
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

    protected override void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Sun"))
		{
            SetCurrentState(states.Safe);
		}
	}
	#endregion

	#region External Inputs
	public override void HandleDamage(float damageAmount)
	{
        print("Earth has been hit!");

        health -= damageAmount;

        if (health <= 0)
		{
			//SceneManager.LoadScene(2);
            SetCurrentState(states.Dead);
		}
	}

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Coroutines
    protected override IEnumerator DyingRoutine()
    {
        //Handle attached enemies
        foreach (GameObject go in attachedEntities)
        {
            go.SendMessage("DislodgeFrom", SendMessageOptions.DontRequireReceiver);
        }
        attachedEntities.Clear();

        //Disable renderer and collider
        model.SetActive(false);
        col.enabled = false;

        //Play Audio


        //Spawn explosion effect
        var dustCloud = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(dustCloud, 10f);

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

        yield return new WaitForSeconds(aSrc.clip.length); //wait for the death music to finish before reloading.

        //... Now what do we load?

        safeRoutine = null;
    }
	#endregion
}
