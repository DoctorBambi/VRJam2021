using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
	AsteroidSpawner astSpawner;
	[SerializeField]
	//curently occupied chunk
	Vector3Int chunk; 
	[SerializeField]
	Transform player;
	public float chunkScale;
	chunk[,,] chunks;
	[SerializeField]
	int numAsteroidsPerChunk = 10;
	void Start(){
		chunks = new chunk[20,20,20];
		astSpawner = GetComponent<AsteroidSpawner>();
	}
	Vector3Int GetCurrentChunk(){
		Vector3 chunk_rough=player.position/chunkScale;
		return new Vector3Int(Mathf.RoundToInt(chunk_rough.x),Mathf.RoundToInt(chunk_rough.y),Mathf.RoundToInt(chunk_rough.z));
	}
	void Update(){
		bool changedChunkThisFrame = GetCurrentChunk() != chunk;
		chunk = GetCurrentChunk();
		/* Debug.Log(GetCurrentChunk()); */
		/* Debug.Log(chunk); */
		chunk chunkVar = chunks[GetCurrentChunk().x,GetCurrentChunk().y,GetCurrentChunk().z];
		if(changedChunkThisFrame){
			if(chunkVar != null){
				foreach (Vector3 curCoord in chunkVar.asteroids)
				{
					astSpawner.SpawnAsteroid(curCoord);	
				}	
			}else{
				for (int i = 0; i < numAsteroidsPerChunk; i++)
				{
					astSpawner.SpawnAsteroid(player.position);
				}
			}
			foreach (Vector3 coord in chunkVar.asteroids)
			{
				Debug.Log(Physics.OverlapSphere(coord, .2f)[0].gameObject);	
			}
		}
	}

}
public class chunk{
	public Vector3[] asteroids;
}
