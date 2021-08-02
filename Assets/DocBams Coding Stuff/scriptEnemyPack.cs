using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An enemy pack is a collection of enemies, when one becomes aware of the player, they can transmit that to other units in the pack.
public class scriptEnemyPack : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject earth;

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

    public awareness aware;

    public Transform packStartPoint;
    public int packSize = 5;
    public float patrolAreaSize = 5;
    public float enemyTerritoryRange = 5;
    public bool packAlerted = true;
    public float sleepRange = 20f; //range at which we will disable pack members to save on resources.
    public bool isSleeping = false;

    private List<GameObject> packUnits = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Go find the earth
        var potentialEarths = GameObject.FindGameObjectsWithTag("Earth");
        if (potentialEarths.Length == 0)
            Debug.LogError("There is no earth object in the scene for this enemy pack to look for.");
        else if (potentialEarths.Length > 1)
            Debug.LogError("There is more than 1 earth object in the scene.");
        else
            earth = potentialEarths[0];

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
        if (earth != null)
		{
            var dist = Vector3.Distance(packStartPoint.position, earth.transform.position);

            if (dist > sleepRange && !isSleeping)
			{
                //put pack to sleep
                aware = awareness.Asleep;
                SleepPackMembers();

			}
            else if (dist < sleepRange && isSleeping)
			{
                //wake pack up
                aware = awareness.Unknown;
                WakePackMembers();
			}
            else if (dist < enemyTerritoryRange)
            {
                if (!packAlerted)
                    aware = awareness.InTerritory;
                else
                    aware = awareness.Alerted;
            }
            else
            {
                aware = awareness.Unknown;
                RetreatPackMembers();
            }
		}
	}

    public void SleepPackMembers()
	{
        isSleeping = true;

        foreach (GameObject unit in packUnits)
		{
            unit.SetActive(false);
		}
	}

    public void WakePackMembers()
	{
        isSleeping = false;

        foreach (GameObject unit in packUnits)
		{
            unit.SetActive(true);
		}
	}

    public void AlertPackMembers(Transform target)
	{
        packAlerted = true;
		
        foreach (GameObject unit in packUnits)
        {
            unit.SendMessage("AlertUnit", target, SendMessageOptions.DontRequireReceiver);
        }
	}

    public void RetreatPackMembers()
	{
        packAlerted = false;

        foreach (GameObject unit in packUnits)
        {
            if (unit != null)
                unit.SendMessage("RetreatUnit", SendMessageOptions.DontRequireReceiver);
        }
    }
}
