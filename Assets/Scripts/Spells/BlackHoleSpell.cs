using UnityEngine;
using System.Collections;

public class BlackHoleSpell : Spell {

	void StartSpell () {
		HideParent();

		TweenPosition.Begin(transform.parent.gameObject,1,transform.position + Vector3.up);
	}
	
	void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.tag == "Enemy")
		{
			GameObject enemy = other.gameObject;
			enemy.GetComponent<Enemy>().enabled = false;
			Destroy(enemy.GetComponent<CharacterController>());
			
			float animTime = 5;
			iTween.ScaleTo(enemy,Vector3.zero,animTime);
			iTween.MoveTo(enemy,transform.parent.position,animTime);
			iTween.RotateBy(enemy,Random.onUnitSphere,animTime);
			
			StartCoroutine(DestroyWithDelay(enemy,animTime));
		}
	}
	
	IEnumerator DestroyWithDelay(GameObject obj,float delay)
	{
		yield return new WaitForSeconds(delay);
		
		Destroy(obj);
	//	Destroy(transform.parent.gameObject);
	}
}
