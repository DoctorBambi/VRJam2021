using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionHelp : MonoBehaviour
{
    [SerializeField] private Transform jetHand0;
    [SerializeField] private Transform jetHand1;

    private void Start()
    {
        Physics.IgnoreCollision(jetHand0.GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(jetHand1.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
