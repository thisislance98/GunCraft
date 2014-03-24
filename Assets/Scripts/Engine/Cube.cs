using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {
	
    int numAwake = 0;
	static List<GameObject> awakeCubes = new List<GameObject>();
	Vector3 lastPos;
	
	float timeStopped = 0;

	
	// Update is called once per frame
	void Update () {
		
		if (Vector3.Distance(lastPos,transform.position) < .01f)
		{
			timeStopped += Time.deltaTime;

		}
		else
			timeStopped = 0;
		
		if (timeStopped > .3f)
		{
			Destroy(gameObject);
		}
		
		lastPos = transform.position;
	}
	
	void Damage(float damage)
	{
		Destroy(gameObject);
	}
}
