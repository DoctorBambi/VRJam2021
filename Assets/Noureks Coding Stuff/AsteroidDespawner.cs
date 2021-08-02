using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDespawner : MonoBehaviour
{
	ChunkLoading cl;
	[SerializeField]
	float dist = 10;

	void Start()
	{
		cl = FindObjectOfType<ChunkLoading>();
		if (cl == null) Debug.LogWarning("No chunk loader in scene.");
	}

	void Update()
	{
		if (cl != null && Vector3.Distance(transform.position, cl.chunk*10) > dist)
		{
			Destroy(this.gameObject);
		}
	}
}
