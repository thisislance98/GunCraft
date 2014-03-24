using UnityEngine;
using System.Collections;

public class ExplosiveSpell : Spell {
	
	public GameObject ExplosionPrefab;
	
	void StartSpell()
	{
	//	Physics.IgnoreCollision(collider,transform.parent.collider);
		Debug.Log("spell started");
	}
	
	void OnTriggerEnter(Collider other)
	{
	
		if (other.gameObject.tag == "Enemy" )
		{
			
			Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);
			other.gameObject.SendMessage("Damage",1,SendMessageOptions.DontRequireReceiver);
			Destroy(transform.parent.gameObject);
		}
	}
}
