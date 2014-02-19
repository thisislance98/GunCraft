using UnityEngine;
using System.Collections;
using MoPhoGames.USpeak.Interface;

public class NetworkPlayer : Photon.MonoBehaviour, ISpeechDataHandler
{
	public Renderer PlayerMeshRenderer;
	public Renderer GunRenderer;
	public Animator CharacterAnim;
	public Transform SpineBone;
	public float LerpPositionSpeed;
	public float LerpRotationSpeed;
	
	Quaternion _lastRotation;
	Vector3 _lastMoveDirection;
	Vector3 _lastPosition;
	Vector3 _targetPosition;
	Vector3 _targetRotation;

	int _team = -1;
	int _idleHash;
	int _walkHash;

	bool _isFirstFrame = true;

	float _lastSpeed;
	float _spineRotation = -90;
	float _lastCameraHeight;

	public static NetworkPlayer Instance;

	IEnumerator Start()
	{
		Instance = this;
	//	_cameraTransform = Camera.main.transform;

		if (photonView.isMine)
		{
			Debug.Log("my view started");
			FlagGameManager.Instance.SetMyPlayer(this);
			//            //We aren't the photonView owner, disable this script
			//            //RPC's and OnPhotonSerializeView will STILL get trough but we prevent Update from running
			//            enabled = false;
			gameObject.tag = "NetworkPlayerOwner";
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<vp_FPSPlayer>().SetNetworkPlayer(this);
			transform.parent = player.transform;
	
			transform.localPosition = new Vector3(0,.70f,0);

			_targetPosition = transform.position;
			PlayerMeshRenderer.enabled = false;
			GunRenderer.enabled = false;

			GetComponent<USpeaker>().SpeakerMode = SpeakerMode.Local;

			yield return new WaitForSeconds(.3f);

			photonView.RPC ("SetTeam",PhotonTargets.AllBuffered,vp_FPSPlayer.Instance.GetTeam());
		}
		else
			GetComponent<USpeaker>().SpeakerMode = SpeakerMode.Remote;

		yield return null;
	}

	public bool IsHoldingFlag()
	{
		return (transform.FindChild("Flag") != null);
	}

	public Transform GetHeldFlag()
	{
		return transform.FindChild("Flag");
	}

	public int GetTeam()
	{
		return _team;
	}

	void FixedUpdate()
	{
		if (photonView.isMine == true)
			UpdateOwner();
		else
			UpdateOthers();

	}    

	// update non-owners of this network player
	void UpdateOthers()
	{

		if (transform.position != _targetPosition)
			transform.position = Vector3.Slerp(transform.position,_targetPosition,Time.deltaTime * LerpPositionSpeed);



		if (transform.forward != _targetRotation && _targetRotation != Vector3.zero)
			transform.forward = Vector3.RotateTowards(transform.forward,_targetRotation,Time.deltaTime * LerpRotationSpeed,1000);

		Vector3 moveDelta = transform.position - _lastPosition;
		float speed = moveDelta.magnitude;

		CharacterAnim.SetFloat("Speed",speed);

		// set character animation direction
		moveDelta.y = 0;
		float forward = Vector3.Dot( moveDelta.normalized , transform.forward);
		float right = Vector3.Dot( moveDelta.normalized , transform.right);
		
		CharacterAnim.SetFloat("DirectionX",right);
		CharacterAnim.SetFloat("DirectionZ",forward);



		_lastPosition = transform.position;
	}

	void UpdateOwner()
	{
		Vector3 moveDelta = transform.parent.position - _lastPosition;
		float speed = moveDelta.magnitude;
		
		// send speed updates
		if (speed != _lastSpeed)
		{
			
			CharacterAnim.SetFloat("Speed",speed);
			//			if (CharacterAnim.IsInTransition(0))
			//			{
			//				photonView.RPC("SetAnimFloat",PhotonTargets.Others,"Speed",speed);
			//			}
		}
		
		// send position updates
		if (speed >= 0.05f || _isFirstFrame)
		{
			moveDelta.y = 0;
			float forward = Vector3.Dot( moveDelta.normalized , transform.forward);
			float right = Vector3.Dot( moveDelta.normalized , transform.right);
			
			CharacterAnim.SetFloat("DirectionX",right);
			CharacterAnim.SetFloat("DirectionZ",forward);
			//Save some network bandwidth; only send a RPC when the position has moved more than 0.05f            
			
			//Send the position Vector3 over to the others; in this case all clients
			photonView.RPC("SetTargetPosition", PhotonTargets.Others, transform.position);
			
			
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
			photonView.RPC ("SetTargetRotation",PhotonTargets.All,forward);
			
			_lastRotation = currentRotation;
		}
		
		_lastSpeed = speed;
		_lastPosition = transform.parent.position;
		_isFirstFrame = false;
	}

	void LateUpdate()
	{

		Vector3 eulers = SpineBone.rotation.eulerAngles;
		eulers.z = _spineRotation;
		SpineBone.rotation = Quaternion.Euler(eulers);

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
		CharacterAnim.SetFloat(paramName,value);

	}

	[RPC]
	void SetCharacterDirection(float x, float z)
	{
		CharacterAnim.SetFloat("DirectionX",x);
		CharacterAnim.SetFloat("DirectionZ",z);
	}

	[RPC]
	void SetTargetPosition(Vector3 newPos)
	{
		//In this case, this function is always ran on the Clients
		//The server requested all clients to run this function (line 25).

		_targetPosition = newPos;

//		if (CharacterAnim.clip.name != "gun_run")
//			CharacterAnim.Play("gun_run");
	}

	[RPC]
	void SetTargetRotation(Vector3 forward)
	{
		_targetRotation = forward;

	}

	#region ISpeechDataHandler Members
	
	/// <summary>
	/// Calls an RPC which passes the data back to USpeaker
	/// </summary>
	public void USpeakOnSerializeAudio( byte[] data )
	{
		photonView.RPC( "vc", PhotonTargets.All, data );
	}
	
	/// <summary>
	/// Calls a buffered RPC which passes the settings data to USpeaker
	/// </summary>
	public void USpeakInitializeSettings( int data )
	{
		photonView.RPC( "init", PhotonTargets.AllBuffered, data );
	}
	
	#endregion

	[RPC]
	void vc( byte[] data )
	{
		GetComponent<USpeaker>().ReceiveAudio( data );
	}
	
	[RPC]
	void init( int data )
	{
		GetComponent<USpeaker>().InitializeSettings( data );
	}

	[RPC]
	void SetTeam(int team)
	{
		Debug.Log("setting team: " + team + " ismine: " + photonView.isMine);
		_team = team;

	}

	public void HitFlag(int teamOfFlag)
	{

		photonView.RPC("OnHitFlag",PhotonTargets.All,teamOfFlag);
	}

	[RPC]
	void OnHitFlag(int teamOfFlag)
	{
	//	Debug.Log("hit flag: " + teamOfFlag + " ismine: " + photonView.isMine + " my team: " + _team + " flag team: " + teamOfFlag);
		if (_team == -1)
			return;

//		if (photonView.isMine)
			
		Flag flag = FlagGameManager.Instance.GetFlag(teamOfFlag);
		flag.OnPlayerTriggerEnter(this);

	}
}
