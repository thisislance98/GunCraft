using UnityEngine;
using System.Collections;

public class MoveWithTranslationSpell : Spell {
	
	public float speed = 30;
	
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
		
		body.isKinematic = true;
		
		if (Input.GetKey(KeyCode.I))
			body.MovePosition(body.position + forward*speed);
		
		if (Input.GetKey(KeyCode.K))
			body.MovePosition(body.position + -forward*speed);
	
		if (Input.GetKey(KeyCode.L))
			body.MovePosition(body.position + right*speed);
		
		if (Input.GetKey(KeyCode.J))
			body.MovePosition(body.position + -right*speed);
		
		if (Input.GetKey(KeyCode.O))
			body.MovePosition(body.position + Vector3.up*speed);
		
		if (Input.GetKey(KeyCode.Period))
			body.MovePosition(body.position + -Vector3.up*speed);
	}
}
