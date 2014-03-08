using UnityEngine;
using System.Collections;

[System.Serializable]
public class Pickup
{
	public GameObject PickupPrefab;
	public float SpawnLikelyhood;
}

public class PickupManager : MonoBehaviour {

	public Pickup[] Pickups;
	
	public static PickupManager Instance;

	// Use this for initialization
	void Start () {
		Instance = this;
	
	}
	
	public void OnBlockDestoyed(Vector3 blockPos, int density)
	{
		// only gold blocks will spawn rewards
		if (density != 3)
			return;

		foreach(Pickup pickup in Pickups)
		{
			if (Random.Range(0.0f,1.0f) < pickup.SpawnLikelyhood)
			{
				Instantiate(pickup.PickupPrefab,blockPos,Quaternion.identity);
				return;
			}

		}


	}
}
