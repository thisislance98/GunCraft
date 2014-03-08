using UnityEngine;
using System.Collections;

public class EnemyCreator : MonoBehaviour {
	
	public GameObject EnemyPrefab;
	public Transform Player;
	public float NumberSpawned = 5;
	public float SpawnTime = 120;
	public float Radius = 20;
	public float DropHeight = 50;
	
	bool timerStarted = false;
	float currentTime;
	
	// Use this for initialization
	void Start () {
		currentTime = 0;// SpawnTime;
	}
	
	public void StartTimer()
	{
		timerStarted = true;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!timerStarted)
			return;
		
		currentTime -= Time.deltaTime;
		
		if (currentTime <= 0)
		{

			for (int i=0; i < NumberSpawned; i++)
				Spawn();
			
			currentTime = SpawnTime;
			
//			Destroy(gameObject);
		}
		
	
	}
	
	void Spawn()
	{
		Vector2 groundPos = (Random.insideUnitCircle.normalized * Radius);
		Vector3 startPos = Player.position + new Vector3(groundPos.x,DropHeight,groundPos.y);	
		
		Vector3 lookDir = Player.position - startPos;
		lookDir.y = 0;
		Quaternion rot = Quaternion.LookRotation(lookDir);
			
		Instantiate(EnemyPrefab,startPos,rot);
					
		
	}
}
