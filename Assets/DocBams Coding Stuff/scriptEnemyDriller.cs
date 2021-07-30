using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEnemyDriller : MonoBehaviour
{
    public GameObject earth;
    public float sightRadius = 50f;
    public float movementSpeed = 0f;

    private Rigidbody rb;
    private Transform target;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        //Initing components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No rigidbody found for this enemy.");

        //Initial state
        isAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Attack Mode
        if (isAttacking == true)
            HandleAttacking();
    }

    void HandleAttacking()
	{
        //Target the earth
        target = earth.transform;

        //Look at target
        transform.LookAt(target);

        //Move toward the target
        rb.MovePosition(target.position);
	}
}
