using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEarthDetector : MonoBehaviour
{
	public Transform player;
	public float maxDistance = 20;
	void Update(){
		if(Vector3.Distance(transform.position, player.position)>maxDistance){

		}
	}
}
