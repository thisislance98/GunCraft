using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	// Controls how the player moves, originally based on FPSWalker
	
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	
	private Vector3 moveDir = Vector3.zero;
	private bool grounded = false;
	
	void FixedUpdate()
	{
		if (grounded)
		{
			moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDir = Quaternion.AngleAxis(transform.localEulerAngles.y, Vector3.up) * moveDir;
			//moveDir = transform.TransformDirection(moveDir);
			moveDir *= speed;
			
			if (Input.GetButton("Jump"))
			{
				moveDir.y = jumpSpeed;
			}
		}
		
		moveDir.y -= gravity*Time.deltaTime;
		
		CharacterController controller = GetComponent<CharacterController>();
		CollisionFlags flags = controller.Move(moveDir*Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	}
}
