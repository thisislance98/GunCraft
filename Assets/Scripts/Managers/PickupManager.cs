using UnityEngine;
using System.Collections;

public class PickupManager : MonoBehaviour {

	public GameObject[] PickupPrefabs;

	public static PickupManager Instance;

	// Use this for initialization
	void Start () {
		Instance = this;
	
	}
	
	public void OnBlockDestoyed(Vector3 blockPos, int density)
	{
		if (density == 3 && Random.Range(0,10) == 0)
			Instantiate(PickupPrefabs[0],blockPos,Quaternion.identity);

	}
}
