using UnityEngine;
using System.Collections;

public class XRayPickup : ObjectPickup {

	override protected void OnTriggerEnter(Collider other)
	{

		if (other.tag != "Player")
			return;

		base.OnTriggerEnter(other);
		
		XRay.Instance.Activate(true);
		
		Destroy(gameObject);
		
	}
}
