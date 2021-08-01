using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDespawner : MonoBehaviour
{
	ChunkLoading cl;
	[SerializeField]
	float dist = 10;
	void Start(){
		cl = FindObjectOfType<ChunkLoading>();
	}
	void Update(){
		if(Vector3.Distance(transform.position, cl.chunk*10)>dist){
			Destroy(this.gameObject);
		}
	}
}
