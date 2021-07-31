using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{

    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        //rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.position = vrTarget.position - trackingPositionOffset;
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRBodyRig : MonoBehaviour
{
    public float turnSmoothness = 1;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;


    public Transform headConstraint;
    private Vector3 headBodyOffest;

    // Start is called before the first frame update
    void Start()
    {
        headBodyOffest = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = headConstraint.position + headBodyOffest;
        transform.forward = Vector3.Lerp(transform.forward,
         Vector3.ProjectOnPlane(headConstraint.up,Vector3.up).normalized,Time.deltaTime * turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
