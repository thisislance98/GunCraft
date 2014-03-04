using UnityEngine;
using System.Collections;

public class XRayPickup : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;
		
		XRay.Instance.Activate(true);
		
		Destroy(gameObject);
		
	}
}
