using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeatSeekerSpell : Spell {
	
	public float Speed = 10;

	bool seek = false;
	Transform target = null;
	PathCreator creater = new PathCreator();
	List<Vector3> path = null;
	bool initialMoveComplete = false;
	
//	void StartSpell()
//	{
//		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
//			
//		float minDist = float.MaxValue;
//		
//		foreach (GameObject enemy in enemies)
//		{
//			float dist = Vector3.Distance(transform.position,enemy.transform.position);
//			if (dist < minDist)
//			{
//				target = enemy.transform;
//				minDist = dist;
//			}
//			
//		}
//			
//	}
	
//	void Update()
//	{
//		if (target == null)
//		{
//			path = null;
//			initialMoveComplete = false;
//			return;
//		}
//		
//		if (false) //path == null)
//		{
//			path = creater.UpdatePath(transform.position,target.position,null);
//			
//			if (path != null)
//				iTween.MoveTo(transform.parent.gameObject,iTween.Hash("movetopath",true,"path",path.ToArray(),"time",1,"oncomplete","OnMoveComplete"));
//		}
//		
//		if (true) //initialMoveComplete)
//		{
//			Vector3 dir = (target.position-transform.position).normalized;
//			
//			Vector3 delta = dir * Speed * Time.deltaTime;
//			Vector3 newPos = transform.position + delta;
//			if (TerrainBrain.Instance().getTerrainDensity(newPos) == 0)
//				transform.parent.position = newPos;
//			
//		}
//	}
	
	void OnMoveComplete()
	{
		initialMoveComplete = true;
	}
	
	void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.tag == "Enemy")
		{
//			if (transform.parent.rigidbody != null)
//				transform.parent.rigidbody.isKinematic = true;
			
			target = other.transform;

		}
	}
	
}
