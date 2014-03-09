using UnityEngine;
using System.Collections;

public class Rocket : Photon.MonoBehaviour {

	public GameObject ExplosionPrefab;
	public float Damage = 12;
	public float BlockDestroyRadius = 3;
	public float DamageRadius = 2;
	float _speed;
	float _timeAlive;
	bool _isMine;
	NetworkPlayer _shootingPlayer;

	// Use this for initialization
	public void Fire(float speed, bool isMine, NetworkPlayer shootingPlayer) {
		_speed = speed;
		_isMine = isMine;
		_shootingPlayer = shootingPlayer;
	}


	// Update is called once per frame
	void Update () {

		transform.position += transform.forward * _speed * Time.deltaTime;
		_timeAlive += Time.deltaTime;

		if (_timeAlive > 10)
			Destroy(gameObject);


		Vector3 checkPos = transform.position + transform.forward * .5f;
		int hitCubeDensity = TerrainBrain.Instance().getTerrainDensity(checkPos);
		if (hitCubeDensity > 0)
		{
			if (_isMine)
			{
				CheckForPlayerHit(checkPos,false);
				OnHitTerrain(checkPos,BlockDestroyRadius,ShotType.Destroy);
			}

			Destroy(gameObject);
		}


	}

	void OnTriggerEnter(Collider other)
	{ 
		CheckForPlayerHit(transform.position,true);
	}

	bool CheckForPlayerHit(Vector3 center, bool destroyIfHit)
	{
		Collider[] colliders = Physics.OverlapSphere(center,DamageRadius);	

		foreach(Collider col in colliders)
		{
			if (col.tag == "NetworkPlayer" && col.GetComponent<NetworkPlayer>() != _shootingPlayer)
			{
	
				float distToPlayer = Vector3.Distance(transform.position,col.transform.position);
				float damage = Damage * (1 - distToPlayer / DamageRadius);
				damage = Mathf.Max(0,damage);

				if (_isMine)
					col.GetComponent<NetworkPlayer>().OnBulletHitPlayer(damage,_shootingPlayer.photonView.viewID);

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
					NetworkPlayer.Instance.photonView.RPC("HitCube",PhotonTargets.All,pos,(int)ShotType.Destroy,0,true);
				}
				

			}
	}

	void OnDestroy()
	{
		Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);

	}
}
