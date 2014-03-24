using UnityEngine;
using System.Collections;

public class MoveWithForceSpell : Spell {
	
	public float force = 30;
	
	// Use this for initialization
	void StartSpell () {
		transform.parent.rigidbody.isKinematic = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (Camera.main == null)
			return;
		
		Vector3 forward = Camera.main.transform.forward;
		Vector3 right = Camera.main.transform.right;
		forward.y = 0;
		right.y = 0;
		
		if (transform.parent == null || transform.parent.rigidbody == null)
			return;
		
		Rigidbody body = transform.parent.rigidbody;
		
		if (Input.GetKey(KeyCode.I))
			body.AddForce(forward*force);
		
		if (Input.GetKey(KeyCode.K))
			body.AddForce(-forward*force);
	
		if (Input.GetKey(KeyCode.L))
			body.AddForce(right*force);
		
		if (Input.GetKey(KeyCode.J))
			body.AddForce(-right*force);
		
		if (Input.GetKey(KeyCode.O))
			body.AddForce(Vector3.up*force);
	}
}
