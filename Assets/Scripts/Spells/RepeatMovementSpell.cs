using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepeatMovementSpell : Spell {
	
	public class Keyframe
	{
		public Vector3 position;
		public Quaternion rotation;
		public float time;
	}
	
	List<Keyframe> frames = new List<Keyframe>();
	bool isRecording = false;
	bool isPlaying = false;
	float currentTime = 0;
	int currentFrame = 0;
	
	// Use this for initialization
	void StartSpell () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (Input.GetKeyDown(KeyCode.O))
		{
			
			if (!isRecording)
			{
				Debug.Log("start recoring");
				StartRecording();
			}
			else
			{
				Debug.Log("start playing");
				StopRecording();
				PlayRecording();
			}
		}
		
		if (isRecording)
		{
			RecordFrame();
			currentTime += Time.deltaTime;
		}	
		else if (isPlaying)
		{
			UpdateCurrentFrame();
			PlayCurrentFrame();
			
			currentTime += Time.deltaTime;
		}
	
	}
	
	void RecordFrame()
	{
		Keyframe frame = new Keyframe();
		frame.position = transform.parent.position;
		frame.rotation = transform.parent.rotation;
		frame.time = currentTime;
		
//		Debug.Log("recording frame at pos: " + frame.position);
		
		frames.Add(frame);
	}
	
	void PlayCurrentFrame()
	{
		Keyframe frame = InterpolateKeyframes(frames[currentFrame],frames[currentFrame+1],currentTime);
		
		transform.parent.position = frame.position;
		transform.parent.rotation = frame.rotation;	
		
//		Debug.Log("playing frame: " + currentFrame + " at pos: " + frame.position);
	}
	
	Keyframe InterpolateKeyframes(Keyframe frame1, Keyframe frame2, float time)
	{
		Keyframe frame = new Keyframe();
		float percent = (time - frame1.time) / (frame2.time - frame1.time);
		frame.position = Vector3.Slerp(frame1.position,frame2.position,percent);
		frame.rotation = Quaternion.Slerp(frame1.rotation,frame2.rotation,percent);
		frame.time = time;
		
		return frame;
	}
	
	void UpdateCurrentFrame()
	{
		if (frames.Count < 2)
			return;

		while (currentTime >= frames[currentFrame+1].time)
		{
			currentFrame++;	
			
			if (currentFrame == frames.Count-1)
			{
				ResetLoop();
				break;
			}
		}
			
	}
	
	void ResetLoop()
	{
		currentFrame = 0;
		currentTime = 0;
	}

	
	void StartRecording()
	{
		currentTime = 0;
		frames.Clear();
		isRecording = true;
		isPlaying = false;
	}
	
	void StopRecording()
	{
		
		isRecording = false;	
	}
	
	void PlayRecording()
	{
		if (frames.Count < 2)
			return;
		
		DestroySpellsOfType(SpellType.Movement);
		currentTime = 0;
		isPlaying = true;
		isRecording = false;
	}
}
