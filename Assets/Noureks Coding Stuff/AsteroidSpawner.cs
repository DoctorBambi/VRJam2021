using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
	void Start(){
		cl=GetComponent<ChunkLoading>();
		/* for (int i = 0; i < 20; i++) */
		/* { */
			/* SpawnAsteroid(Vector3.zero); */	
		/* } */
	}
	[SerializeField]
	GameObject droneSpawnerOne;
	[SerializeField]
	GameObject droneSpawnerTwo;
	[SerializeField]
	ChunkLoading cl;
	[SerializeField]
	float sizeVariation = .25f;
	[SerializeField]
	float baseSize;
	[SerializeField]
	GameObject Asteroid;
	[SerializeField]
	float chunkRadius = 10;
	[SerializeField]
	Transform player;
	[SerializeField]
	Vector3 sunPos;
	public void SpawnAsteroid(Vector3 basePosition,int index){
		Vector3 scaleA = baseSize * RandomConstVectorThree(1-sizeVariation, 1+sizeVariation); 
		Vector3 positionA = basePosition+RandomConstVectorThree(-chunkRadius,chunkRadius);
		GameObject tempAsteroid = Instantiate(Asteroid);
		if(droneSpawnerOne != null && droneSpawnerTwo != null && Random.Range(0, Vector3.Distance(sunPos,player.position)) < 60){
			tempAsteroid = Instantiate(droneSpawnerOne);		
			if(Random.Range(0, 2) == 1){
				tempAsteroid = Instantiate(droneSpawnerTwo);	
			}
		}
		tempAsteroid.transform.position = positionA;
		tempAsteroid.transform.localScale = scaleA;
		tempAsteroid.transform.eulerAngles = RandomConstVectorThree(-180, 180);
		cl.chunks[cl.GetCurrentChunk().x,cl.GetCurrentChunk().y,cl.GetCurrentChunk().z].asteroids[index]=positionA;
	}
	public void spawnAsteroidDirect(Vector3 basesPosition){
		Vector3 scaleA = baseSize * RandomConstVectorThree(1-sizeVariation, 1+sizeVariation); 
		GameObject tempAsteroid = Instantiate(Asteroid);
		tempAsteroid.transform.position = basesPosition;
		tempAsteroid.transform.localScale = scaleA;
		tempAsteroid.transform.eulerAngles = RandomConstVectorThree(-180, 180);
	}
	public Vector3 RandomConstVectorThree(float min, float max){
		return new Vector3(Random.Range(min, max),Random.Range(min, max),Random.Range(min, max));
	}
}
