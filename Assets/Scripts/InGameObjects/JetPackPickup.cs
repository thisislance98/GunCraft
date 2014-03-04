using UnityEngine;
using System.Collections;

public class JetPackPickup : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;
		
		JetPack.Instance.Activate(true);
		
		Destroy(gameObject);
		
	}
}
