using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBehaviour : MonoBehaviour
{
    [SerializeField] private float pullAmount;
    [SerializeField] private GameObject center;

    private void Update()
    {
        PullObjects();
    }

    private void PullObjects()
    {
        
    }
}
