using UnityEngine;
using System.Collections;

public class MagicEyeSpell : Spell {
	
	GameObject camera;
	Vector3 lastPos;
	// Use this for initialization
	void StartSpell () {
		camera = GameObject.Find("SpareCamera");
		camera.GetComponent<Camera>().enabled = true;
		camera.transform.forward = transform.parent.forward;
		camera.transform.parent = transform.parent;
		camera.transform.localPosition = Vector3.zero;
		lastPos = transform.parent.position;
	}
	
	void Update()
	{
		
		Vector3 delta = transform.parent.position - lastPos;
		
		if (delta.magnitude > 0)
		{
			camera.transform.forward = Vector3.Slerp(camera.transform.forward, delta.normalized,Time.deltaTime);
			
		}
		
		lastPos = transform.parent.position;
	}
	

}
