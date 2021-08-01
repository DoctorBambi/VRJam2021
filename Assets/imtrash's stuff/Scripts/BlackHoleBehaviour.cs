using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBehaviour : MonoBehaviour
{
    [SerializeField] private float pullAmount;
    [SerializeField] private GameObject center;
    private List<GameObject> victims = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        victims.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        victims.Remove(other.gameObject);
    }

    private void Update()
    {
        PullObjects();
    }

    private void PullObjects()
    {
        if(victims.Count > 0)
        {
            foreach (GameObject victim in victims)
            {
                Vector3 diff = center.transform.position - victim.transform.position;
                victim.transform.position += diff / diff.magnitude * pullAmount;
            }
        }
    }
}
