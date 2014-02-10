using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementRecorderSpell : MonoBehaviour {
	
	bool recording;
	
	List<Vector3> positions = new List<Vector3>();

	
	// Update is called once per frame
	void Update () 
	{
		if (!recording && Input.GetKey(KeyCode.C))
			recording = true;
		else if (recording && Input.GetKey(KeyCode.C))
		{
			recording = false;
			PlayRecording();	
		}
		
		if (recording)
		{
			positions.Add(transform.parent.position);
		}
		
		
	}
	
	void PlayRecording()
	{
		
//		iTween.MoveTo(transform.parent.gameObject,iTween.Hash
			
	}
}
