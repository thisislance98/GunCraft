using UnityEngine;
using System.Collections;

public class GoldCoin : MonoBehaviour {
	
	Transform player;
	
	bool moving;
	
	IEnumerator Start()
	{
		
		player = GameObject.FindGameObjectWithTag("Player").transform;
		
		yield return new WaitForSeconds(60);
		
		Destroy(gameObject);
			
	}
	
	void Update()
	{
		if (Vector3.Distance(player.position,transform.position) < 2 && !moving)
		{
			moving = true;
			MoveToPlayer();	
			
			
		}
		
	}
	
	void MoveToPlayer()
	{
		
		float moveTime = .5f;
		Debug.Log("coin at pos " + transform.position + " moving to player: " + player.position);
		
		iTween.MoveTo(gameObject,iTween.Hash("position",player.position + Vector3.up*1.0f,"time",moveTime,"islocal",false,"oncomplete","OnMoveComplete"));
		
		
	}
	
	void OnMoveComplete()
	{
		Purse.Instance.AddGold(1);
		Destroy(gameObject);
	}
}
