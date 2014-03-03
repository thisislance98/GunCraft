using UnityEngine;
using System.Collections;

public class MachineGunPickup : MonoBehaviour {


	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		vp_FPSPlayer player = other.GetComponent<vp_FPSPlayer>();

		player.OnFoundAmmo(3,200);

		Destroy(gameObject);

	}
}
