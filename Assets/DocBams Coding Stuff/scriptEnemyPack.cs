using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//An enemy pack is a collection of enemies, when one becomes aware of the player, they can transmit that to other units in the pack.
public class scriptEnemyPack : MonoBehaviour
{
	#region Properties
	public GameObject[] enemyPrefabs;

    public enum types
    {
        AllDriller,
        AllHunter,
        RandomMix
    }

    public types type;

    public enum awareness
    {
        Asleep,
        Unknown,
        InTerritory,
        Alerted
    }

    public awareness currentAwareness;
    public awareness prevAwareness;

    public Transform packStartPoint;
    public int packSize = 5;
    public float patrolAreaSize = 5;
    public float enemyTerritoryRange = 5;
    public float sleepRange = 20f; //range at which we will disable pack members to save on resources.
    public bool isSleeping = true;

    //Events
    public event Action<scriptEnemyPack> OnPackAlertedAction;
    public UnityEvent onPackAlerted;

    private List<GameObject> packUnits = new List<GameObject>();
	#endregion

	#region Monobehaviour Stuff
	// Start is called before the first frame update
	void Start()
    {
        OnPackAlertedAction += HandlePackAlerted;
        SpawnPack(type, packStartPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Check earth proximity
        CheckProximity();
    }
    #endregion

    #region AI Behaviour
    public virtual void SetCurrentAwareness(awareness newState)
    {
        if (newState != currentAwareness)
        {
            prevAwareness = currentAwareness; //cache the previous state
            currentAwareness = newState;
        }
    }
    #endregion

    #region Internal Inputs
    public void SpawnPack(types type, Vector3 position)
	{
		switch (type)
		{
            case types.AllDriller:
                GameObject drillerPf = enemyPrefabs[(int)types.AllDriller];
                
                for (int i = 0; i < packSize; i++)
                {
                    var unit = Instantiate(drillerPf);

                    unit.transform.position = UnityEngine.Random.insideUnitSphere * patrolAreaSize + position;

                    var script = unit.GetComponent<scriptEnemyDriller>();
                    script.pack = this;
                    script.startPoint = packStartPoint.position;
                    script.patrolAreaSize = patrolAreaSize;

                    packUnits.Add(unit);
                }
                break;
            case types.AllHunter:
                GameObject hunterPf = enemyPrefabs[(int)types.AllHunter];

                for (int i = 0; i < packSize; i++)
                {
                    var unit = Instantiate(hunterPf);

                    unit.transform.position = UnityEngine.Random.insideUnitSphere * patrolAreaSize + position;

                    var script = unit.GetComponent<scriptEnemyHunter>();
                    script.pack = this;
                    script.startPoint = packStartPoint.position;
                    script.patrolAreaSize = patrolAreaSize;

                    packUnits.Add(unit);
                }
                break;
            default:
                Debug.LogWarning($"Have not implemented pack type {type}.");
                break;
        }
	}

    void CheckProximity()
	{
        if (scriptEarth.Instance != null && scriptEarth.Instance.currentState != scriptPlanetoid.states.Dead && scriptEarth.Instance.currentState != scriptPlanetoid.states.Safe)
		{
            var dist = Vector3.Distance(packStartPoint.position, scriptEarth.Instance.transform.position);

            //Sleep state
            if (dist > sleepRange)
			{
                //put pack to sleep
                SetCurrentAwareness(awareness.Asleep);
                SleepPackMembers();
			}
            else
			{
                //wake pack up
                SetCurrentAwareness(awareness.Unknown);
                WakePackMembers();
			}

            //Territory state
            if (dist < enemyTerritoryRange)
            {
                bool enemySighted = false;
                foreach (GameObject unit in packUnits)
				{
                    if (enemySighted == true) break; //a unit found a target so we're alerted

                    if (unit.GetComponent<scriptEnemy>().target != null)
                        enemySighted = true;
				}

                if (!enemySighted)
                    SetCurrentAwareness(awareness.InTerritory);
				else
				{
                    OnPackAlertedAction?.Invoke(this); //A little hacky, but call this first so that, when future frames come through, we can check on the Alerted awareness to prevent the event from firing every frame.
                    SetCurrentAwareness(awareness.Alerted);
				}
            }
		}
		else //earth is not in an attackable state, so pull everyone back home.
		{
            SetCurrentAwareness(awareness.Unknown);
        }
	}

    public void SleepPackMembers()
	{
        if (!isSleeping)
		{
            isSleeping = true;

            foreach (GameObject unit in packUnits)
		    {
                unit.SetActive(false);
		    }
		}
	}

    public void WakePackMembers()
	{
        if (isSleeping)
		{
            isSleeping = false;

            foreach (GameObject unit in packUnits)
            {
                unit.SetActive(true);
            }
        }
	}
    #endregion

    #region Events
    /// <summary>
    /// This pack has been altered this frame
    /// </summary>
    public void HandlePackAlerted(scriptEnemyPack pack)
    {
        if (onPackAlerted != null && currentAwareness != awareness.Alerted)
        {
            onPackAlerted.Invoke();
        }
    }
    #endregion
}
