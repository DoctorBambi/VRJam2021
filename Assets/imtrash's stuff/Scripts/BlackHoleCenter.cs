using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCenter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            GetComponentInParent<BlackHoleBehaviour>().RemoveObjectOnList(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
