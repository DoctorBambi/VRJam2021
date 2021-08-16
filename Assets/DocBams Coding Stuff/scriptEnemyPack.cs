using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Transform packStartPoint;
    public int packSize = 5;
    public float patrolAreaSize = 5;
    public float enemyTerritoryRange = 5;
    public float sleepRange = 20f; //range at which we will disable pack members to save on resources.
    public bool isSleeping = true;

    private List<GameObject> packUnits = new List<GameObject>();
	#endregion

	// Start is called before the first frame update
	void Start()
    {
        SpawnPack(type, packStartPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Check earth proximity
        CheckProximity();
    }

    public void SpawnPack(types type, Vector3 position)
	{
		switch (type)
		{
            case types.AllDriller:
                GameObject drillerPf = enemyPrefabs[(int)types.AllDriller];
                
                for (int i = 0; i < packSize; i++)
                {
                    var unit = Instantiate(drillerPf);

                    unit.transform.position = Random.insideUnitSphere * patrolAreaSize + position;

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

                    unit.transform.position = Random.insideUnitSphere * patrolAreaSize + position;

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
                currentAwareness = awareness.Asleep;
                SleepPackMembers();
			}
            else
			{
                //wake pack up
                currentAwareness = awareness.Unknown;
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
                    currentAwareness = awareness.InTerritory;
                else
                    currentAwareness = awareness.Alerted;
            }
		}
		else //earth is not in an attackable state, so pull everyone back home.
		{
            currentAwareness = awareness.Unknown;
            //RetreatPackMembers();
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

 //   public void AlertPackMembers(Transform target)
	//{
 //       foreach (GameObject unit in packUnits)
 //       {
 //           unit.SendMessage("AlertUnit", target, SendMessageOptions.DontRequireReceiver);
 //       }
	//}

 //   public void RetreatPackMembers()
	//{
 //       if (packAlerted)
	//	{
 //           packAlerted = false;

 //           foreach (GameObject unit in packUnits)
 //           {
 //               if (unit != null)
 //                   unit.SendMessage("RetreatUnit", SendMessageOptions.DontRequireReceiver);
 //           }
 //       }
 //   }
}
