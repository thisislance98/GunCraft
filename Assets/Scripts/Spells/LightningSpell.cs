using UnityEngine;
using System.Collections;

public class LightningSpell : Spell {
	
	public GameObject ExplosionPrefab;
	
	Vector3 startPos;
	float timeUntilLightning = .5f;
	float radius = 5;
	float height = 2.6f;
	float currentTime = 0;

	
	void StartSpell () {
		HideParent();
	//	TweenPosition.Begin(transform.parent.gameObject,6,transform.position + Vector3.up);
		transform.position += Vector3.up * height;
		startPos = transform.position;
	}
	
	void Update()
	{
		currentTime += Time.deltaTime;
		
		if (currentTime >= timeUntilLightning)
		{
			transform.position = startPos + new Vector3(Random.Range(-radius,radius),0,Random.Range(-radius,radius));
			RaycastHit hit;
			Physics.Raycast(transform.position,Vector3.down, out hit, 8);
			
			if (hit.transform != null)
			{
				Vector3 pos = transform.position;
				pos.y = hit.point.y + height - .5f;
				transform.position = pos;
				transform.GetChild(0).GetComponent<ParticleSystem>().Play(true);
				
			}
			timeUntilLightning = Random.Range(.1f, 4);
			currentTime = 0;	
		}
		
	}

	void OnTriggerEnter(Collider other)
	{

		
		if (other.gameObject.tag == "Enemy")
		{
			Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);
			other.gameObject.SendMessage("Damage",1,SendMessageOptions.DontRequireReceiver);
		}
	}
}
