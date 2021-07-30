using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
	[SerializeField]
	Vector3Int chunk; 
	[SerializeField]
	Transform player;
	public float chunkScale;
	chunk[,,] chunks;
	Vector3Int GetCurrentChunk(){
		Vector3 chunk_rough=player.position/chunkScale;
		return new Vector3Int(Mathf.RoundToInt(chunk_rough.x),Mathf.RoundToInt(chunk_rough.y),Mathf.RoundToInt(chunk_rough.z));
	}
	void Update(){
		bool changedChunkThisFrame = GetCurrentChunk() == chunk;
		chunk = GetCurrentChunk();
		if(changedChunkThisFrame){
			if(chunks[chunk.x,chunk.y,chunk.z] != null){
				Debug.Log("Entered new chunk");
			}
		}
	}

}
public class chunk{
	Vector3[] asteroids;
}
