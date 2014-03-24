using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour {

	public GameObject ChunkPrefab;
	
	const float kChunkSize = 16;
	const float kHeight = 5;
	const float kRadius = 10;
	
	// Use this for initialization
	IEnumerator Start () {
	
		for (float x = -kRadius; x <= kRadius; x++)
			for (float z = -kRadius; z <= kRadius; z++)
				for (float y = 0; y < kHeight; y++)
			{	
				Vector3 pos = new Vector3(x*kChunkSize, y*kChunkSize, z*kChunkSize);
				GameObject chunk = (GameObject)Instantiate(ChunkPrefab,pos,Quaternion.identity);
				
				
				
				yield return new WaitForSeconds(.1f);
			}
	}
	
}
