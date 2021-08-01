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

    private types type;

    public enum awareness
    {
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

    private List<GameObject> packUnits = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        type = types.AllDriller;

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
                GameObject pf;
                foreach (GameObject go in enemyPrefabs)
				{
                    if (go.name.Contains("Driller"))
					{
                        pf = go;
                        for (int i = 0; i < packSize; i++)
                        {
                            var unit = Instantiate(pf);

                            unit.transform.position = Random.insideUnitSphere * patrolAreaSize + position;

                            var script = unit.GetComponent<scriptEnemyDriller>();
                            script.pack = this;

                            packUnits.Add(unit);
                        }
                        break;
					}
				}
                break;
		}
        
	}

    void CheckProximity()
	{
        if (earth != null)
		{
            if (!packAlerted)
			{
                var dist = Vector3.Distance(packStartPoint.position, earth.transform.position);

                if (dist < enemyTerritoryRange)
                {
                    print("In enemy territory.");
                    aware = awareness.InTerritory;
                    //audioManager.SetCurrentVibe(scriptAudioManager.vibes.EnemyTerritory);
                }
                else
                {
                    print("Out of enemy territory.");
                    aware = awareness.Unknown;
                    //audioManager.SetCurrentVibe(scriptAudioManager.vibes.Chill);
                }
            }
			else
			{
                print("Alerted.");
                aware = awareness.Alerted;
                //audioManager.SetCurrentVibe(scriptAudioManager.vibes.Alerted);
			}
		}
	}

    public void AlertPackMembers(Transform target)
	{
        packAlerted = true;

		switch (type)
		{
            case types.AllDriller:
                foreach (GameObject unit in packUnits)
                {
                    var script = unit.GetComponent<scriptEnemyDriller>();
                    script.target = target;
                    script.SetCurrentState(scriptEnemyDriller.states.Chasing);
                }
                break;
		}
	}
}
