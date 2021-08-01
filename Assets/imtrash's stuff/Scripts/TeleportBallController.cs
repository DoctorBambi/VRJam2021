using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBallController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject targetPlayer;
    [Header("Values")]
    [Tooltip("The speed in which the ball needs to travel in order to teleport")]
    [SerializeField] private float minVelocity = 4f;
    [Tooltip("Time it takes to decide to teleport at the balls position")]
    [SerializeField] private float timeTillTele = 10f;

    private bool beingHeld;
    private bool thrown = false;
    private bool resetTeleport = true;
    private bool collideWithTele = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.layer);
        if (collision.gameObject.layer == 12)
        {
            print("Passed Layer Check");
            collideWithTele = true;
            MakeTeleportPoint();
        }
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
        if (!beingHeld && thrown == true && !collideWithTele)
        {
            if (resetTeleport)
            {
                print("Starting Teleport Process");
                Invoke("MakeTeleportPoint", timeTillTele);
                //timeTillTele -= Time.deltaTime;
                //if (timeTillTele <= 0)
                //{
                //    timeTillTele = rememberTimer;
                //    GameObject pointObj = new GameObject();
                //    pointObj.name = "telePoint";
                //    pointObj.transform.position = gameObject.transform.position;
                //    TeleportPlayer(pointObj);
                //    resetTeleport = false;
                //}
            }
        }
    }

    private void MakeTeleportPoint()
    {
        print("Testing Ball Tele Called");
        GameObject pointObj = new GameObject();
        pointObj.name = "telePoint";
        pointObj.transform.position = gameObject.transform.position;
        TeleportPlayer(pointObj);
    }

    private void TeleportPlayer(GameObject point)
    {
        print("Teleport Player Called");
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        targetPlayer.transform.position = point.transform.position;
        //resetTeleport = true;
        //print(point.transform.position);
        collideWithTele = false;
        Destroy(point);
        Destroy(gameObject);
    }
}
