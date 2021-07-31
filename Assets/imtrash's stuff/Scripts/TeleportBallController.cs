using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBallController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform targetPlayer;
    [Header("Values")]
    [Tooltip("The speed in which the ball needs to travel in order to teleport")]
    [SerializeField] private float minVelocity = 4f;
    [Tooltip("Time it takes to decide to teleport at the balls position")]
    [SerializeField] private float timeTillTele = 10f;

    private bool beingHeld;
    private bool thrown = false;
    private bool resetTeleport = true;
    private float rememberTimer;

    private void Awake()
    {
        rememberTimer = timeTillTele;
    }

    public void SetBallGrab()
    {
        beingHeld = true;
    }
    public void ResetBallGrab()
    {
        beingHeld = false;
    }

    private void Update()
    {
        StartTeleportProcess();
        BallBeingThrown();
    }

    private void BallBeingThrown()
    {
       if(gameObject.GetComponent<Rigidbody>().velocity.magnitude > minVelocity)
        {
            thrown = true;
        }
        else
        {
            thrown = false;
        }
    }

    /// <summary>
    /// This function checks if the ball is thrown at the required velocity.
    /// It will then start a timer that will tell the teleport function where the balls position ended up.
    /// </summary>
    private void StartTeleportProcess()
    {
        if (!beingHeld && thrown == true)
        {
            if (resetTeleport)
            {
                timeTillTele -= Time.deltaTime;
                if (timeTillTele <= 0)
                {
                    timeTillTele = rememberTimer;
                    TeleportPlayer(gameObject.transform.position);
                    //resetTeleport = false;
                }
            }
        }
    }

    private void TeleportPlayer(Vector3 pos)
    {
        targetPlayer.position = pos;
        //resetTeleport = true;
        Destroy(gameObject);
    }
}
