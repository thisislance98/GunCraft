using UnityEngine;
using System.Collections;

public class ParentToCameraSpell : Spell {
	
	bool wasKinematic;
	float distFromCamera;
	Transform cameraTransform;
	float speed = 10;
	
	Vector3 targetPos;
	
	// Use this for initialization
	void StartSpell () {
		wasKinematic = transform.parent.rigidbody.isKinematic;
		transform.parent.rigidbody.isKinematic = true;
		cameraTransform = Camera.main.transform;
		transform.parent.parent = cameraTransform;
		distFromCamera = Vector3.Distance(transform.parent.position,cameraTransform.position);
		

	}
	

	void Update()
	{
		targetPos = cameraTransform.position + (cameraTransform.forward * distFromCamera);
		Vector3 dir = targetPos - transform.parent.position;
		float delta = speed * Time.deltaTime;
		if (Vector3.Distance(transform.parent.position,targetPos) > delta)
		{
			transform.parent.position += delta * dir;	
		}
		else
			transform.parent.position = targetPos;
		
		
		if (Input.GetKey(KeyCode.I))
		{
			distFromCamera += speed * Time.deltaTime;
		}
		
		if (Input.GetKey(KeyCode.K))
		{
			distFromCamera -= speed * Time.deltaTime;
		}
		
		if (Input.GetKey(KeyCode.L))
		{
			transform.parent.rigidbody.isKinematic = wasKinematic;
			transform.parent.parent = null;
			Destroy(gameObject);
		}
		
	}
	

}
