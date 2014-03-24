using UnityEngine;
using System.Collections;

public class FireTowerSpell : Spell {
	
	public GameObject FireballPrefab;
	public float fireballSpeed = 10;
	public float FireTime = 1;
	
	float currentTime = 0;
	
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		

	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Enemy" && currentTime > FireTime)
		{
			GameObject fireball = (GameObject)Instantiate(FireballPrefab,transform.position,Quaternion.identity);
			
			Vector3 dir = (other.transform.position-transform.position).normalized;
		
			fireball.SendMessage("SetTarget",other.transform);
			currentTime = 0;
			Destroy(transform.parent.gameObject);
		}
	}
}
