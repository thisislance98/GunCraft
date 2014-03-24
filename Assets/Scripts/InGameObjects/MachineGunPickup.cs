using UnityEngine;
using System.Collections;

public class MachineGunPickup : ObjectPickup {


	override protected void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		base.OnTriggerEnter(other);

		vp_FPSPlayer player = other.GetComponent<vp_FPSPlayer>();

		player.OnFoundAmmo(3,200);

		Destroy(gameObject);

	}
}
