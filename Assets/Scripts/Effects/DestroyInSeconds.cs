using UnityEngine;
using System.Collections;

public class DestroyInSeconds : MonoBehaviour {
	
	public float DestroyTime = 10;
	
	float currentTime = 0;
	
	
	// Update is called once per frame
	void Update () 
	{
		currentTime += Time.deltaTime;
		
		if (currentTime >= DestroyTime)
			Destroy(gameObject);
	
	}
}
