using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
	void Start(){
		for (int i = 0; i < 20; i++)
		{
			SpawnAsteroid(Vector3.zero);	
		}
	}
	[SerializeField]
	float sizeVariation = .25f;
	[SerializeField]
	float baseSize;
	[SerializeField]
	GameObject Asteroid;
	[SerializeField]
	float chunkRadius = 10;
	void SpawnAsteroid(Vector3 basePosition){
		Vector3 scaleA = baseSize * RandomConstVectorThree(1-sizeVariation, 1+sizeVariation); 
		Vector3 positionA = basePosition+RandomConstVectorThree(-chunkRadius,chunkRadius);
		GameObject tempAsteroid = Instantiate(Asteroid);
		tempAsteroid.transform.position = positionA;
		tempAsteroid.transform.localScale = scaleA;
		tempAsteroid.transform.eulerAngles = RandomConstVectorThree(-180, 180);
	}
	public Vector3 RandomConstVectorThree(float min, float max){
		return new Vector3(Random.Range(min, max),Random.Range(min, max),Random.Range(min, max));
	} 
}
