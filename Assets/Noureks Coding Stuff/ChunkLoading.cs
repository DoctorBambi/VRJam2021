using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
	[SerializeField]
	AsteroidSpawner astSpawner;
	[SerializeField]
	//curently occupied chunk
	public Vector3Int chunk; 
	[SerializeField]
	Transform player;
	public float chunkScale;
	public chunk[,,] chunks;
	[SerializeField]
	int numAsteroidsPerChunk = 10;
	void Start(){
		chunkVar = new chunk();
		chunk = Vector3Int.one;
		chunks = new chunk[20,20,20];
		astSpawner = GetComponent<AsteroidSpawner>();
		for (int i = 0; i < numAsteroidsPerChunk; i++)
		{
		}
	}
	public Vector3Int GetCurrentChunk(){
		Vector3 chunk_rough=player.position/chunkScale;
		return new Vector3Int(Mathf.RoundToInt(chunk_rough.x),Mathf.RoundToInt(chunk_rough.y),Mathf.RoundToInt(chunk_rough.z));
	}
	void OnDrawGizmos(){
		Gizmos.DrawCube(chunk*10, new Vector3(10,10,10));
	}
	chunk chunkVar;
	void Update(){
		bool changedChunkThisFrame = GetCurrentChunk() != chunk;
		Debug.Log(chunks[chunk.x,chunk.y,chunk.z]);
		if(changedChunkThisFrame){
			/* foreach (GameObject currentProcAsteroid in chunkVar.asteroids) */
			/* { */
				/* Destroy(currentProcAsteroid); */
			/* } */
		}
		chunk = GetCurrentChunk();
		chunkVar = chunks[GetCurrentChunk().x,GetCurrentChunk().y,GetCurrentChunk().z];
		/* Debug.Log(chunkVar); */
		if(changedChunkThisFrame){
			Debug.Log("entered new chunk");
			if(chunkVar != null){
				Debug.Log("new chunk");
				foreach (Vector3 curCoord in chunkVar.asteroids)
				{
					astSpawner.spawnAsteroidDirect(curCoord);	
				}	
			}else{
				chunks[GetCurrentChunk().x,GetCurrentChunk().y,GetCurrentChunk().z] = new chunk();
				for (int i = 0; i < numAsteroidsPerChunk; i++)
				{
					astSpawner.SpawnAsteroid(player.position, i);
				}
			}
		}
	}

}
public class chunk{
	public Vector3[] asteroids = new Vector3[10];
}
