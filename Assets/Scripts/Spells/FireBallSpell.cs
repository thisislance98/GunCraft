using UnityEngine;
using System.Collections;

public class FireBallSpell : Spell {

	public GameObject ExplosionPrefab;
	
	// Use this for initialization
	void StartSpell () {
		HideParent();
		DestroyImmediate( transform.parent.gameObject.collider );
		SphereCollider c = transform.parent.gameObject.AddComponent<SphereCollider>();
		c.radius = transform.GetComponent<SphereCollider>().radius;
		TweenPosition.Begin(transform.parent.gameObject,1,transform.position + Vector3.up);
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);
			other.gameObject.SendMessage("Damage",1,SendMessageOptions.DontRequireReceiver);
			Destroy(transform.parent.gameObject);
		}
	}
}
