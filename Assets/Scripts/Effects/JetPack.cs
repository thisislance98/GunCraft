using UnityEngine;
using System.Collections;

public class JetPack : MonoBehaviour {

	vp_FPSController controller;
	float upAcceleration = 10;
	Vector3 velocity;
	
	// Use this for initialization
	void Start () {
		controller = GetComponent<vp_FPSController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	

		if (Input.GetKey(KeyCode.Space))
		{
			AddJumpForce();

		}
	
		Vector3 moveDir = controller.GetMoveDir();
		if (moveDir.magnitude > 0)
		{
			Ray ray = new Ray(collider.bounds.center,moveDir);
			if (Physics.Raycast(ray, 1.5f))
			{
		//		AddJumpForce();
			}
		}
		
	}

	void AddJumpForce()
	{
	
		gameObject.SendMessage("AddForce",new Vector3(0,1.5f,0) * Time.deltaTime,SendMessageOptions.DontRequireReceiver);	

	}
}
