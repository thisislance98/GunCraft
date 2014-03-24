using UnityEngine;
using System.Collections;


public 	enum ProjectileType
{
	Creator,
	Destroyer,
	Spell
}


public class WandProjectile : MonoBehaviour {
	
	public GameObject ExplosionPrefab;
	
	public float speed;
	Vector3 velocity;
	Vector3 lastPos;
	
	public ProjectileType type;
	
	// Use this for initialization
	void Awake () {
		velocity = Camera.main.transform.forward * speed;
		lastPos = transform.position;
	}
	
	public void SetVelocity(Vector3 vel)
	{
		velocity = 	vel;
	}
	
	public void SetProjectileType(ProjectileType pType)
	{
		type = pType;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += velocity * Time.deltaTime;
		
		RaycastHit hit;
		Physics.Linecast(lastPos,transform.position,out hit);
		
		if (hit.transform != null && hit.transform.tag != "Projectile" && hit.transform.tag != "Player" && !hit.collider.isTrigger)
		{
			
			if (hit.transform.tag == "Chunk")
			{
				Vector3 cubePos = hit.point + (velocity.normalized*.01f);
				int density = TerrainBrain.Instance().getTerrainDensity(cubePos);
		
				if (density != 0)
					OnCollision(hit);	
			}
			else if (type == ProjectileType.Destroyer)
			{
				hit.collider.gameObject.SendMessage("Damage",1,SendMessageOptions.DontRequireReceiver);	
				
			}
			else if (type == ProjectileType.Spell)
			{
				SpellManager.Instance.ApplyCurrentSpell(hit.transform.gameObject);
				
			}
			
			Destroy(gameObject);
			Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);
		}
//		if (TerrainBrain.Instance().getTerrainDensity(transform.position) != 0)
//		{
//			OnCollision();
//			
//		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag != "Projectile" && other.transform.tag != "Player" && !other.collider.isTrigger)
		{
			if (type == ProjectileType.Destroyer)
			{
				other.gameObject.SendMessage("Damage",1,SendMessageOptions.DontRequireReceiver);	
				
			}
		}
	}
	
	void OnCollision(RaycastHit hit)
	{
		
		GameObject chunk = hit.collider.gameObject;
		
		TerrainPrefabBrain brain = chunk.GetComponent<TerrainPrefabBrain>();
	//	brain.GotHit(type,hit.point,velocity);
	}
}
