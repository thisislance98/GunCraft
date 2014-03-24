using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class FlagGameManager : Photon.MonoBehaviour {

	Base[] Bases = new Base[2];
	Transform[] Flags = new Transform[2];
	public UILabel TheirScoreLabel;
	public UILabel MyScoreLabel;
	public AudioClip WinClip;
	public AudioClip LoseClip;


	public static FlagGameManager Instance;


	
	List<GameObject> _flagObservers = new List<GameObject>();

	NetworkPlayer _myPlayer;
	int _myTeam = -1;
	int _theirTeam = -1;
	int _myScore;
	int _theirScore;


	void Awake()
	{
		Instance = this;
		Bases[0] = GameObject.Find("Base0").GetComponent<Base>();
		Flags[0] = Bases[0].transform.FindChild("Flag");
		Bases[1] = GameObject.Find("Base1").GetComponent<Base>();
		Flags[1] = Bases[1].transform.FindChild("Flag");



	}

	public void OnTimeUp()
	{
		Debug.Log("on time up");
		PhotonNetwork.Disconnect();
	//	Application.LoadLevel(Application.loadedLevelName);

	}

	public void AddFlagObserver(GameObject observer)
	{
		_flagObservers.Add(observer);
	}

	public void OnFlagStateChange()
	{
		photonView.RPC("OnFlagStateChangeRPC",PhotonTargets.AllBuffered);
	}

	[RPC]
	public IEnumerator OnFlagStateChangeRPC()
	{

		while (GetMyPlayer() == null || GetMyPlayer().GetTeam() == -1)
		{
			yield return new WaitForSeconds(.3f);
		}

		GetFlag(0).UpdateFlag();
		GetFlag(1).UpdateFlag();


		foreach(GameObject observer in _flagObservers)
		{
			observer.GetComponent<ArrowToBase>().OnFlagStateChange();
		}

		UpdateScoreLabels();
	}

	public NetworkPlayer GetMyPlayer()
	{
		return _myPlayer;
	}

	public void OnConnected(NetworkPlayer player)
	{
		SetMyPlayer(player);

		_myScore = GetMyScore();
		_theirScore = GetTheirScore();
		
		UpdateScoreLabels();
	}

	public void SetMyPlayer(NetworkPlayer player)
	{
		_myPlayer = player;

	}

	public void OnScore(int scoringTeam)
	{
		Hashtable hash = PhotonNetwork.room.customProperties;

		Debug.Log("team scored: " + scoringTeam);

		ValidateTeamScores();

		if (scoringTeam == 0)
			hash["Team0Score"] = (int)hash["Team0Score"] + 1;
		else
			hash["Team1Score"] = (int)hash["Team1Score"] + 1;



		PhotonNetwork.room.SetCustomProperties(hash);
		UpdateScoreLabels();
		
	}

	void ValidateTeamScores()
	{
		Hashtable hash = PhotonNetwork.room.customProperties;

		bool didSet = false;

		if (hash["Team0Score"] == null)
		{
			hash["Team0Score"] = 0;
			didSet = true;
		}
		if (hash["Team1Score"] == null)
		{
			hash["Team1Score"] = 0;
			didSet = true;
		}

		if (didSet)
			PhotonNetwork.room.SetCustomProperties(hash);
	}

	int GetMyScore()
	{
		ValidateTeamScores();

		if (FlagGameManager.Instance.GetMyPlayer().GetTeam() == 0)
			return (int)PhotonNetwork.room.customProperties["Team0Score"];
		else
			return (int)PhotonNetwork.room.customProperties["Team1Score"];
	}

	int GetTheirScore()
	{
		ValidateTeamScores();
		
		if (FlagGameManager.Instance.GetMyPlayer().GetTeam() == 1)
			return (int)PhotonNetwork.room.customProperties["Team0Score"];
		else
			return (int)PhotonNetwork.room.customProperties["Team1Score"];
	}

	void UpdateScoreLabels()
	{
		int myScore = GetMyScore();
		int theirScore = GetTheirScore();


		Debug.Log("my score: " + _myScore + " new score: " + myScore);

		if (myScore > _myScore)
			AudioManager.Instance.Play(WinClip);
		else if (theirScore > _theirScore)
		    AudioManager.Instance.Play(LoseClip);

		_myScore = myScore;
		_theirScore = theirScore;
		MyScoreLabel.text = "Your Score: " + myScore;
		TheirScoreLabel.text = "Their Score: " + theirScore;


	}

	public bool IsTeamSet()
	{
		return _myTeam != -1;
	}

	public void SetTeam(int team)
	{
		_myTeam = team;
		_theirTeam = (team + 1) % 2;
	}

	public Base GetTheirBase()
	{
		return Bases[_theirTeam];
	}

	public Base GetBase(int team)
	{
		return Bases[team];
	}
	public Vector3 GetBasePosition(int team)
	{
		return Bases[team].transform.position;
	}

	public Vector3 GetMyBasePosition()
	{
		return GetBasePosition(_myTeam);
	}

	public Vector3 GetTheirBasePosition()
	{
		return GetBasePosition(_theirTeam);
	}

	public Transform GetTheirFlag()
	{
		return Flags[_theirTeam];
	}

	public Flag GetFlag(int team)
	{

		return Flags[team].GetComponent<Flag>();
	}

	public Vector3 GetTheirFlagPosition()
	{
		return Flags[_theirTeam].position;
	}

	public Vector3 GetMyFlagPosition()
	{
		return Flags[_myTeam].position;
	}
	


}
