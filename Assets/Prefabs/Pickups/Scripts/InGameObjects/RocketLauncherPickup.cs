using UnityEngine;
using System.Collections;

public class RocketLauncherPickup : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;
		
		vp_FPSPlayer player = other.GetComponent<vp_FPSPlayer>();
		
		player.OnFoundAmmo(4,20);
		
		Destroy(gameObject);
		
	}
}
