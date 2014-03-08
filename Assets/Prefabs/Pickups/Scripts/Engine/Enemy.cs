using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	
	public GameObject ExplosionPrefab;
	Transform bouy;
	float speed = 2;
	float life = 1;
	bool alive = true;
	bool landed = false;
	List<Vector3> path;
	float damage = 0;
	PathCreator pathCreator = new PathCreator();
	Transform player;
	float moveDist;
	float moveTime;
	Vector3 lastPos;
	
	
	// Use this for initialization
	void Start () {
		bouy = GameObject.FindGameObjectWithTag("Bouy").transform;
		animation.CrossFade("walk",.3f);
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	public void Damage(float damage)
	{
		life -= damage;
		if (life <= 0)
			Kill();
		else
		{
			animation.CrossFade("getting_hit",.3f);
			animation.CrossFadeQueued("walk",.3f,QueueMode.CompleteOthers,PlayMode.StopAll);
		}
	}
	
	public void Kill()
	{
		animation.CrossFade("gets_killed",.1f);
	
		
		rigidbody.constraints = RigidbodyConstraints.None;
		
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2)); //Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, 1000.0f))
		{
			DestroyImmediate(GetComponent<CharacterController>());
			
			BoxCollider collider = gameObject.AddComponent<BoxCollider>();
			if (collider)
				collider.size = new Vector3(1,1,1);
			//collider.bounds = new Bounds(transform.position,new Vector3(.5f,.5f,.5f));
			Vector3 flyDir = transform.position - bouy.position;
			flyDir.y = 0;
			flyDir = (flyDir.normalized + Vector3.up) * 5;
			rigidbody.velocity = flyDir;
			rigidbody.useGravity = true;
			rigidbody.angularVelocity = Random.insideUnitSphere * 30;// -transform.right * 20;
		}
		StartCoroutine("Die");
		
		alive = false;
	//	Destroy(gameObject);	
	}
	
	void OnLanded()
	{
	//	animation.CrossFade("flight_landing",.3f);
	//	animation.CrossFadeQueued("walk",.3f,QueueMode.CompleteOthers,PlayMode.StopAll);
		landed = true;
		
		Debug.Log("on landed");

		path = pathCreator.CreatePath(transform.position,bouy.position,false);
		
		Debug.Log("pos: " + transform.position + " path: " + path.Count);
		transform.position = path[0];
		
		DrawPath();

	}
	
	void DrawPath()
	{
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();

		if (lineRenderer == null)
			lineRenderer = gameObject.AddComponent<LineRenderer>();
		
		lineRenderer.SetVertexCount(path.Count);
		lineRenderer.SetWidth(.1f,.1f);
		
		for(int i=0; i < path.Count; i++)
		{
			lineRenderer.SetPosition(i,path[i]);	
		}
			
	}
	
	IEnumerator Die()
	{
		yield return new WaitForSeconds(1);
		Instantiate(ExplosionPrefab,transform.position,Quaternion.identity);
		Destroy(gameObject);
	}
	// Update is called once per frame
	void Update () 
	{
		if (!alive)
			return;
		
		CharacterController controller = GetComponent<CharacterController>();
		if (!landed && controller.isGrounded)
			OnLanded();
		
		
		Vector3 moveDirection = GetMoveDirection();
		Vector3 delta = moveDirection*Time.deltaTime * speed;
		
		
		controller.Move(delta);

		moveDirection.y = 0;
		if (moveDirection != Vector3.zero)
		{
			transform.forward = Vector3.Slerp(transform.forward, moveDirection,.1f);
		}
		
		HandleBeingStuck();
		
		lastPos = transform.position;
	}
	
	void HandleBeingStuck()
	{
		// reset the path if something is blocking them and they don't move much in a certain amount of time
		moveDist += Vector3.Distance(lastPos,transform.position);
		if (landed)
			moveTime += Time.deltaTime;
		
		if (moveTime >= 1)
		{
			Debug.Log("move dist: " + moveDist);
			if (moveDist < 1)
			{
				ResetPath();
			}
			
			moveTime = 0;
			moveDist = 0;
		}		
	}
	
	Vector3 GetMoveDirection()
	{
		// go down if there is nothing under their feet
		CharacterController controller = GetComponent<CharacterController>();
		RaycastHit hit;
		Ray downRay = new Ray(transform.position,Vector3.down);
		Physics.Raycast(downRay,out hit, 1.0f);
		if (!landed)// hit.transform == null && controller.velocity.y <= 0)
		{
			return Vector3.down*10;
		}
		else if (path != null && path.Count > 0)
		{
	
			Vector3 dir = (path[0] - transform.position).normalized;
			
			if (Vector3.Distance(transform.position,path[0]) <= .1f)
			{
				transform.position = path[0];
				path.RemoveAt(0);
				
				// get more path if we run out and we are still not at the target
				if (path.Count == 0 && Vector3.Distance(transform.position,bouy.position) > 1)
				{
					ResetPath();
				}
			}
			
			return dir;
		}
		
		return Vector3.zero;
	}
	
	void ResetPath()
	{
		// let the enemy climb walls if their horizontal distance to the bouy is small
		bool allowCliming = (Vector3.Distance(new Vector3(transform.position.x,0,transform.position.z),
			new Vector3(bouy.position.x,0,bouy.position.z)) < 10);
		
		pathCreator.Clear();
		
		
		path = pathCreator.CreatePath(transform.position,bouy.position,allowCliming);
		DrawPath();
//		transform.position = path[0];				
	}
	
	
}
