using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {

	public float DestroyInSeconds = 30;

	virtual protected void OnTriggerEnter(Collider other)
	{

	}
	
	// Update is called once per frame
	void Update () {
		DestroyInSeconds -= Time.deltaTime;

		if (DestroyInSeconds < 0)
			Destroy(gameObject);
	}
}
