using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBallController : MonoBehaviour
{
    [SerializeField] private bool debugging = false;
    [Header("Components")]
    [SerializeField] private GameObject targetPlayer;
    [Header("Values")]
    [Tooltip("The speed in which the ball needs to travel in order to teleport")]
    [SerializeField] private float minVelocity = 4f;
    [Tooltip("Time it takes to decide to teleport at the balls position")]
    [SerializeField] private float timeTillTele = 10f;

    private Rigidbody rb;
    private CharacterController controller;
    private List<scriptHandSnapper> physicalHands = new List<scriptHandSnapper>();
    private scriptStunGun stunGun;
    private bool beingHeld;
    private bool thrown = false;
    private bool resetTeleport = true;
    private bool collideWithTele = false;
    private float rememberTimer;

	private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found.", gameObject);

        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        if (targetPlayer == null) Debug.LogError("No player found.", gameObject);

        controller = targetPlayer.GetComponentInChildren<CharacterController>();
        if (controller == null) Debug.LogError("No Character controller found on player.", gameObject);

        var potentialHands = GameObject.FindGameObjectsWithTag("PhysicalHand");
        if (potentialHands != null)
		{
            foreach (GameObject hand in potentialHands)
			{
                var script = hand.GetComponent<scriptHandSnapper>();
                if (script == null) Debug.LogError("No scriptHandSnapper found on physical hand.", hand);

                physicalHands.Add(script);
			}
		}

        var potentialStunGun = GameObject.FindGameObjectWithTag("StunGun");
        if (potentialStunGun != null) stunGun = potentialStunGun.GetComponent<scriptStunGun>();
    }

	private void Awake()
    {
        rememberTimer = timeTillTele;
    }

    public void SetBallGrab()
    {
        beingHeld = true;

        //Set the teleport ball to the teleport ball layer so that it doesn't interact with the physics hands.
        //It has to be set after grabbing otherwise you can't initially grab the teleport ball.
        gameObject.layer = 17;
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
        if (debugging) print(collision.gameObject.layer);

        if (collision.gameObject.layer == 12)
        {
            if (debugging) print("Passed Layer Check");
            collideWithTele = true;
            MakeTeleportPoint();
        }
    }

    private void BallBeingThrown()
    {
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > minVelocity)
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
                if (debugging) print("Starting Teleport Process");
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
        if (debugging) print("Testing Ball Tele Called");
        GameObject pointObj = new GameObject();
        pointObj.name = "telePoint";
        pointObj.transform.position = gameObject.transform.position;
        TeleportPlayer(pointObj);
    }

    private void TeleportPlayer(GameObject point)
    {
        if (debugging) print("Teleport Player Called");

        // Apply Teleport to Player
        controller.enabled = false;
        controller.transform.position = point.transform.position;

        // Apply Teleport to Physical hands if we are using them
        foreach (scriptHandSnapper hand in physicalHands)
		{
            hand.SnapPosition(point.transform.position);
		}

        // Apply Teleport to Earth and Stungun
        if (physicalHands.Count > 0)
		{
            scriptEarth.Instance.SnapPosition(point.transform.position);
            if (stunGun != null) stunGun.SnapPosition(point.transform.position);
		}

        // Re-Enable the character controller so we can move again
        controller.enabled = true;

        //targetPlayer.transform.position = point.transform.position;

        //resetTeleport = true;
        //if (debugging) print(point.transform.position);

        collideWithTele = false;
        Destroy(point);
        Destroy(gameObject);
    }
}
