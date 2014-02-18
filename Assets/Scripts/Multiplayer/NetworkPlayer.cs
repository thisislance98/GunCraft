using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
	public Renderer PlayerMeshRenderer;
	public Animator CharacterAnim;
	public Transform SpineBone;
	
	Quaternion _lastRotation;
	Vector3 _lastMoveDirection;
	Vector3 _lastPosition;


	int _idleHash;
	int _walkHash;

	float _lastSpeed;
	float _spineRotation = -90;
	float _lastCameraHeight;

	public static NetworkPlayer Instance;

	void Awake()
	{
		Instance = this;
	//	_cameraTransform = Camera.main.transform;

		if (photonView.isMine)
		{
			//            //We aren't the photonView owner, disable this script
			//            //RPC's and OnPhotonSerializeView will STILL get trough but we prevent Update from running
			//            enabled = false;
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			transform.parent = player.transform;
	
			transform.localPosition = new Vector3(0,.70f,0);

		//	PlayerMeshRenderer.enabled = false;
		}
	}

	public void SetMoveDirection(Vector3 moveDir)
	{
		if (moveDir != _lastMoveDirection)
			photonView.RPC("SetCharacterDirection",PhotonTargets.All,moveDir.x,moveDir.y);

		_lastMoveDirection = moveDir;
	}
	
	void FixedUpdate()
	{
		if (photonView.isMine == false)
			return;

		float speed = Vector3.Distance(transform.parent.position, _lastPosition);

		if (speed != _lastSpeed)
		{
	
			CharacterAnim.SetFloat("Speed",speed);
			if (CharacterAnim.IsInTransition(0))
			{
				photonView.RPC("SetAnimFloat",PhotonTargets.Others,"Speed",speed);
			}
		}


		if (speed >= 0.05f)
		{

			//Save some network bandwidth; only send a RPC when the position has moved more than 0.05f            

			
			//Send the position Vector3 over to the others; in this case all clients
			photonView.RPC("SetPosition", PhotonTargets.Others, transform.position);


		}

		// set spine rotation 
		float cameraHeight = Camera.main.transform.forward.y;
		if (cameraHeight != _lastCameraHeight)
		{
			photonView.RPC ("SetSpineRotation",PhotonTargets.All,cameraHeight);
			_lastCameraHeight = cameraHeight;
		}


		// Now set rotation around the y axis
		Quaternion currentRotation = Camera.main.transform.rotation;
		if (currentRotation != _lastRotation)
		{
			Vector3 forward = Camera.main.transform.forward;
			forward.y = 0;
			photonView.RPC ("SetRotation",PhotonTargets.All,forward);

			_lastRotation = currentRotation;
		}

		_lastSpeed = speed;
		_lastPosition = transform.parent.position;
	}    


	void LateUpdate()
	{

		Vector3 eulers = SpineBone.rotation.eulerAngles;
		eulers.z = _spineRotation;
		SpineBone.rotation = Quaternion.Euler(eulers);

		Debug.Log("camera forward: " + SpineBone.rotation.eulerAngles);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

	}

	[RPC]
	void SetSpineRotation(float cameraHeight)
	{
		_spineRotation = -90 + (cameraHeight * 90);
	}
	
	[RPC]
	void SetAnimFloat(string paramName, float value)
	{
	
		Debug.Log("setting " + paramName + " to : " + value);
		CharacterAnim.SetFloat(paramName,value);

	}

	[RPC]
	void SetCharacterDirection(float x, float z)
	{
		CharacterAnim.SetFloat("DirectionX",x);
		CharacterAnim.SetFloat("DirectionZ",z);
	}

	[RPC]
	void SetPosition(Vector3 newPos)
	{
		//In this case, this function is always ran on the Clients
		//The server requested all clients to run this function (line 25).
		
		transform.position = newPos;

//		if (CharacterAnim.clip.name != "gun_run")
//			CharacterAnim.Play("gun_run");
	}

	[RPC]
	void SetRotation(Vector3 forward)
	{
		transform.forward = forward;

	}

}
