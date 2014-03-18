using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Pickup
{
	public GameObject PickupPrefab;
	public float SpawnLikelyhood;
}

public class PickupManager : MonoBehaviour {

	public Pickup[] Pickups;
	
	public static PickupManager Instance;
	public float PickupLikelyhood = .05f;
	List<Pickup> _pickupBucket = new List<Pickup>();

	// Use this for initialization
	void Start () {
		Instance = this;
	
		foreach (Pickup pickup in Pickups)
		{
			for (int i=0; i < pickup.SpawnLikelyhood; i++)
			{
				_pickupBucket.Add(pickup);

			}

		}

	}
	
	public void OnBlockDestoyed(Vector3 blockPos, int density)
	{
		// only gold blocks will spawn rewards
		if (density != TextureManager.Instance.PickupTextureIndex)
			return;

		GameObject prefab = _pickupBucket[Random.Range(0,_pickupBucket.Count)].PickupPrefab;

		Instantiate(prefab,blockPos,Quaternion.identity);



	}
}
