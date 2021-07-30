using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoading : MonoBehaviour
{
	void Update(){
		Vector3Int chunk = GetCurrentChunk();
	}
	[SerializeField]
	Transform player;
	public float chunkScale;
	chunk[,,] chunks;
	Vector3Int GetCurrentChunk(){
		Vector3 chunk_rough=player.position/chunkScale;
		return new Vector3Int(Mathf.RoundToInt(chunk_rough.x),Mathf.RoundToInt(chunk_rough.y),Mathf.RoundToInt(chunk_rough.z));
	}	
}
public class chunk{
	Vector3[] asteroids;
}
