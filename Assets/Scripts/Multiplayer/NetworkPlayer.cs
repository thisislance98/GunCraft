using UnityEngine;
using System.Collections;
using MoPhoGames.USpeak.Interface;
using System.Collections.Generic;

public class NetworkPlayer : Photon.MonoBehaviour, ISpeechDataHandler
{
	public GameObject BloodParticlePrefab;
	public GameObject ProjectilePrefab;
	public GameObject RocketPrefab;
	public Renderer PlayerMeshRenderer;
	public Renderer GunRenderer;
	public Animator CharacterAnim;
	public Transform SpineBone;
	public float LerpPositionSpeed;
	public float LerpRotationSpeed;
	public AudioClip GrownClip;
	public AudioClip DeathClip;

	public int KillScore = 10;
	public int DeathScore = -10;
	public int CaptureScore = 50;

	static List<GameObject> _playerObservers = new List<GameObject>();
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
		CharacterAnim.SetFloat("Health",1);
	//	_cameraTransform = Camera.main.transform;


		if (photonView.isMine)
		{
			PhotonNetwork.networkingPeer.NetworkSimulationSettings.IncomingLag = 500;
			PhotonNetwork.networkingPeer.NetworkSimulationSettings.OutgoingLag = 500;
			PhotonNetwork.networkingPeer.NetworkSimulationSettings.IncomingJitter = 50;
			PhotonNetwork.networkingPeer.NetworkSimulationSettings.OutgoingJitter = 50;
//			PhotonNetwork.networkingPeer.NetworkSimulationSettings.IncomingLossPercentage = 5;
//			PhotonNetwork.networkingPeer.NetworkSimulationSettings.OutgoingLossPercentage = 5;
			PhotonNetwork.networkingPeer.IsSimulationEnabled = true;

			Instance = this;

			collider.enabled = false;
			FlagGameManager.Instance.OnConnected(this);
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
		{
			GetComponent<USpeaker>().SpeakerMode = SpeakerMode.Remote;
			photonView.RPC("GetTargetPositionFromOwner",photonView.owner,PhotonNetwork.player);
		}

		
		yield return null;
	}

	public static void AddPlayerObserver(GameObject obj)
	{
		_playerObservers.Add(obj);
	}
	
	void SendObserversMessage(string message, bool isMine)
	{
		foreach (GameObject obj in _playerObservers)
			obj.SendMessage(message,isMine,SendMessageOptions.DontRequireReceiver);

	}

	[RPC]
	void DropFlag()
	{
		GetHeldFlag().SendMessage("OnDroppedFlag",this);
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
	void GetTargetPositionFromOwner(PhotonPlayer requestingPlayer)
	{
		photonView.RPC("SetTargetPosition",requestingPlayer,transform.position);
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
		_team = team;
		Color teamColor;

		if (team == vp_FPSPlayer.Instance.GetTeam())
		{
			SendObserversMessage("OnPlayerJoined",true);
			teamColor = Color.green;
		}
		else
		{
			SendObserversMessage("OnPlayerJoined",false);
			teamColor = Color.red;
		}

		PlayerMeshRenderer.material.SetColor("_Color",teamColor);

		for (int i=0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SendMessage("SetTeamColor",teamColor,SendMessageOptions.DontRequireReceiver);
		}
	}

	public void HitFlag(int teamOfFlag)
	{
		Flag flag = FlagGameManager.Instance.GetFlag(teamOfFlag);
		flag.OnPlayerTriggerEnter(this);

	//	photonView.RPC("OnHitFlag",PhotonTargets.AllBuffered,teamOfFlag);
	}

//	[RPC]
//	IEnumerator OnHitFlag(int teamOfFlag)
//	{
//
//		while (FlagGameManager.Instance == null || FlagGameManager.Instance.GetMyPlayer() == null)
//		{
//			yield return new WaitForSeconds(1);
//		}
//	//	Debug.Log("hit flag: " + teamOfFlag + " ismine: " + photonView.isMine + " my team: " + _team + " flag team: " + teamOfFlag);
//		if (_team != -1)
//		{
//			Flag flag = FlagGameManager.Instance.GetFlag(teamOfFlag);
//			flag.OnPlayerTriggerEnter(this);
//		}
//
//		yield return null;
//
//	}

	public void FireRocket(Vector3 startPos, Quaternion rotation, float speed)
	{
		float startTime = (float)PhotonNetwork.time;

		photonView.RPC("FireRocketRPC",PhotonTargets.All,startPos,rotation,speed,startTime);

	}

	[RPC]
	void FireRocketRPC(Vector3 startPos, Quaternion rotation, float speed, float startTime)
	{
	
		GameObject rocket = (GameObject)Instantiate(RocketPrefab,startPos,rotation);

		float timeDelta = (float)PhotonNetwork.time - startTime;
		rocket.transform.position += rocket.transform.forward * speed * timeDelta;
		rocket.GetComponent<Rocket>().Fire(speed,photonView.isMine,this);

	}

	public void FireProjectile(Vector3 startPosition, Quaternion rotation, float scale, int shotType, int terrainDensity)
	{

		vp_Bullet bullet = ((GameObject)Object.Instantiate(ProjectilePrefab, startPosition, rotation)).GetComponent<vp_Bullet>();

		HitType hitType; 

		Vector3 hitPos = bullet.Fire((ShotType)shotType,terrainDensity, out hitType, photonView.viewID);

		if (hitType == HitType.Cube)
		{
			photonView.RPC("HitCube",PhotonTargets.Others,hitPos,shotType,terrainDensity,false);
		}


		bullet.transform.localScale = new Vector3(scale, scale, scale);	// preset defined scale
	}

	[RPC]
	public void HitCube(Vector3 hitPos, int shotType, int density, bool isRocket)
	{

		GameObject chunkObj = TerrainPrefabBrain.findTerrainChunk(hitPos);
		
		
		if ( chunkObj != null )
		{
			if (isRocket)
			{
				int hitCubeDensity = TerrainBrain.Instance().getTerrainDensity(hitPos);

				if (hitCubeDensity > 0)
				{
					TerrainPrefabBrain terrain = chunkObj.GetComponent<TerrainPrefabBrain>();
					terrain.OnBulletHit(hitPos,(ShotType)shotType,density,true);
				}
			}
			else // it's a bullet
			{
				vp_Bullet bullet = ((GameObject)Object.Instantiate(ProjectilePrefab)).GetComponent<vp_Bullet>();
				bullet.HitCube(hitPos,shotType,density,chunkObj.GetComponent<TerrainPrefabBrain>());
			}
		}
		else // the terrain is not within view distance
		{
			Debug.Log("couldn't find terrain at hit pos: " + hitPos);
			TerrainBrain.Instance().setTerrainDensity(hitPos,density);
		}


	}
	

	public void OnProjectileHitPlayer(float damage, int shootingPlayerViewId)
	{
		photonView.RPC("OnProjectileHitPlayerRPC",PhotonTargets.All, photonView.ownerId,damage,shootingPlayerViewId);
	}

	[RPC]
	void OnProjectileHitPlayerRPC(int ownerId,float damage, int shootingPlayerViewId)
	{

		PhotonView shootingPlayerView = PhotonView.Find(shootingPlayerViewId);

		// don't do anything if we shoot a teammate
		if  (shootingPlayerView.GetComponent<NetworkPlayer>().GetTeam() == GetTeam())
			return;


		if (photonView.ownerId == ownerId && transform.parent != null) // owner
		{
			vp_FPSPlayer player = transform.parent.gameObject.GetComponent<vp_FPSPlayer>();

			if (player.IsDead())
				return;

			player.OnGotHit(damage,shootingPlayerView.transform.position);


			if (player.m_Health <= 0)
			{
				photonView.RPC("OnPlayerDied",PhotonTargets.All);
				audio.PlayOneShot(DeathClip);
				PlayerHelper.Set<int>("Deaths",PlayerHelper.Get<int>("Deaths",0)+1);
				PlayerHelper.Set<int>(shootingPlayerView.owner, "Kills",PlayerHelper.Get<int>(shootingPlayerView.owner,"Kills",0)+1);
				UpdateScore(PhotonNetwork.player);
				UpdateScore(shootingPlayerView.owner);
			}
			else
				audio.PlayOneShot(GrownClip);

		}
		else // non owner
		{
			audio.PlayOneShot(GrownClip);
//			GameObject blood = (GameObject)Instantiate(BloodParticlePrefab,hitPos,Quaternion.LookRotation(hitNormal,Vector3.up));
//			blood.transform.parent = transform;
		}

	}

	public void  UpdateScore(PhotonPlayer player)
	{
		int score = PlayerHelper.Get<int>(player,"Kills",0) * KillScore;
		score += PlayerHelper.Get<int>(player,"Deaths",0) * DeathScore;
		score += PlayerHelper.Get<int>(player,"Captures",0) * CaptureScore;

		PlayerHelper.Set<int>(player,"Score",score);

	}

	[RPC]
	void OnPlayerDied()
	{
		CharacterAnim.SetFloat("Health",0);
		if (IsHoldingFlag())
			GetHeldFlag().SendMessage("OnDroppedFlag",this);
	}

	[RPC]
	void OnPlayerResurrect()
	{
		CharacterAnim.SetFloat("Health",1);
	}

	void OnDestroy()
	{
		if (IsHoldingFlag())
			GetHeldFlag().SendMessage("OnDroppedFlag",this);


		SendObserversMessage("OnPlayerLeft",(_team == vp_FPSPlayer.Instance.GetTeam()));




	}

}
