using UnityEngine;
using System.Collections;

public class ParentToCameraSpell : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.parent.rigidbody.isKinematic = true;
		transform.parent.parent = Camera.main.transform;
	}
	

	void Update()
	{
		if (Input.GetKey(KeyCode.K))
		{
			transform.parent.rigidbody.isKinematic = false;
			transform.parent.parent = null;
				
		}
		
	}
	

}
