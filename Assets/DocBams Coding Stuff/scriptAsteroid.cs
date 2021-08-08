using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scriptAsteroid : scriptPlanetoid
{
    #region Properties

    protected Renderer rend;

    #endregion

    #region MonoBehaviour Stuff
    protected override void Start()
	{
		base.Start();

        rend = GetComponent<Renderer>();
    }

	// Update is called once per frame
	protected override void Update()
    {
        //Death mode
        if (currentState == states.Dead)
            HandleDeath();
    }
    #endregion

    #region AI Behaviour
    
    #endregion

    #region External Inputs
    public override void HandleDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            SetCurrentState(states.Dead);
        }
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
        rend.enabled = false;
        col.enabled = false;

        //Play Audio


        //Spawn destroy effect
        var dustCloud = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(dustCloud, 5f);

        yield return new WaitForSeconds(0);

        hasDied = true;
        dyingRoutine = null;
    }
    #endregion
}
