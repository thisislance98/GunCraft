using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {


	float _speed;

	// Use this for initialization
	void Fire (float speed) {
		_speed = speed;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * _speed * Time.deltaTime;
	}

	void OnCollisionEnter(Collision other)
	{
		Vector3 contactPoint = other.contacts[0].point;
		Collider[] colliders = Physics.OverlapSphere(contactPoint,10);	

		foreach(Collider col in colliders)
		{
			if (col.tag == "NetworkPlayer")
				col.GetComponent<NetworkPlayer>().OnBulletHitPlayer();
		}
		
		Destroy(gameObject);
	}

	void OnHitPlayer(NetworkPlayer player)
	{
		player.OnBulletHitPlayer();
	}
}
