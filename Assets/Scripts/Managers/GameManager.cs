using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject BouyPrefab;
	

	
	// Update is called once per frame
	void Update () 
	{
		
		if (Input.GetKeyDown(KeyCode.H) && GameObject.FindGameObjectWithTag("Bouy") == null)
		{
			Instantiate(BouyPrefab,GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.up*50,
				Quaternion.Euler(-90,0,0));
			GameObject.Find("EnemyCreator").GetComponent<EnemyCreator>().StartTimer();
		}
	
	}
}
