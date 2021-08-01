using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class PlayerJetPackController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] public float fallVelocity = -5f;
    [SerializeField] private float liftVelocity = 10f;
    [SerializeField] private float forwardVelocity = 6f;

    private CharacterController character;

    //private Vector3 moveDirection = Vector3.zero;

    private bool slowFall = false;

    private float leftHandTrig;
    private float rightHandTrig;
    private Vector2 leftHandJoy;

    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateInputs();

        BlastUp();

        //character.Move(moveDirection * Time.deltaTime);

    }

    private void UpdateInputs()
    {
        leftHandTrig = InputBridge.Instance.LeftTrigger;
        rightHandTrig = InputBridge.Instance.RightTrigger;
        leftHandJoy = InputBridge.Instance.LeftThumbstickAxis;
    }

    public void BlastUp()
    {
        //moveDirection = Vector3.zero;
        if (leftHandTrig > 0.9 && rightHandTrig > 0.9)
        {
            slowFall = false;
            GetComponent<PlayerGravity>().GravityEnabled = false;
            transform.position += Vector3.up * Time.deltaTime * liftVelocity;
            //transform.position += Vector3.forward * Time.deltaTime * forwardVelocity;
        }
        else { slowFall = true; }

        CheckForSlowFall();

    }

    private void CheckForSlowFall()
    {
        if (character.isGrounded) { slowFall = false; }

        if (slowFall) { GetComponent<PlayerGravity>().GravityEnabled = true; }
    }
}
