using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An enemy pack is a collection of enemies, when one becomes aware of the player, they can transmit that to other units in the pack.
public class scriptEnemyPack : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    public enum types
    {
        AllDriller,
        AllHunter,
        RandomMix
    }

    private types type;

    public Transform packStartPoint;
    public int packSize;

    // Start is called before the first frame update
    void Start()
    {
        type = types.AllDriller;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
