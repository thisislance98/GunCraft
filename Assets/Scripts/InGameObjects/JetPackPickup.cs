using UnityEngine;
using System.Collections;

public class JetPackPickup : ObjectPickup {

	override protected void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		base.OnTriggerEnter(other);
		
		JetPack.Instance.Activate(true);
		
		Destroy(gameObject);
		
	}
}
