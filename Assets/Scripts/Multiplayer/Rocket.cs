using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	public float Damage = 12;
	public float BlockDestroyRadius = 3;
	public float DamageRadius = 6;
	float _speed;
	float _timeAlive;
	// Use this for initialization
	void Fire (float speed) {
		_speed = speed;
	}
	
	// Update is called once per frame
	void Update () {

		transform.position += transform.forward * _speed * Time.deltaTime;
		_timeAlive += Time.deltaTime;

		int hitCubeDensity = TerrainBrain.Instance().getTerrainDensity(transform.position);
		if (hitCubeDensity > 0)
		{
			CheckForPlayerHit(transform.position,false);
			OnHitTerrain(transform.position,BlockDestroyRadius,ShotType.Destroy);
		}

		if (_timeAlive > 10)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{ 
		Debug.Log("trigger entered: " + other.name);
		CheckForPlayerHit(transform.position,true);

	}

	bool CheckForPlayerHit(Vector3 center, bool destroyIfHit)
	{
		Collider[] colliders = Physics.OverlapSphere(center,BlockDestroyRadius);	
		

		foreach(Collider col in colliders)
		{
	//		Debug.Log("hit collider: " + col.transform.name);
			if (col.tag == "NetworkPlayer")
			{
				Debug.Log("got player");
				float distToPlayer = Vector3.Distance(transform.position,col.transform.position);
				float damage = Damage * (1 - distToPlayer / DamageRadius);
				damage = Mathf.Max(0,damage);

				col.GetComponent<NetworkPlayer>().OnBulletHitPlayer(damage);

				if (destroyIfHit)
					Destroy(gameObject);

				return true;
			}
		}

		return false;
	}
	

	void OnHitTerrain(Vector3 hitPos, float radius, ShotType shotType)
	{
	
		for (float x = hitPos.x - radius; x <= hitPos.x + radius; x++)
			for (float y = hitPos.y - radius; y <= hitPos.y + radius; y++)
				for (float z = hitPos.z - radius; z <= hitPos.z + radius; z++)
			{
				Vector3 pos = new Vector3 (x,y,z);


				if (shotType == ShotType.Destroy)
				{
					NetworkPlayer.Instance.photonView.RPC("HitCube",PhotonTargets.All,pos,(int)ShotType.Destroy,0);
				}
				

			}
		Destroy(gameObject);
	}
}
