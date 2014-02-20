using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlagGameManager : Photon.MonoBehaviour {

	Base[] Bases = new Base[2];
	Transform[] Flags = new Transform[2];
	public UILabel TheirScoreLabel;
	public UILabel MyScoreLabel;

	public static FlagGameManager Instance;
	
	List<GameObject> _flagObservers = new List<GameObject>();

	NetworkPlayer _myPlayer;
	int _myTeam;
	int _theirTeam;
	int _myTeamScore;
	int _theirScore;

	// Use this for initialization
	void Awake () {
		Instance = this;
		Bases[0] = GameObject.Find("Base0").GetComponent<Base>();
		Flags[0] = Bases[0].transform.FindChild("Flag");
		Bases[1] = GameObject.Find("Base1").GetComponent<Base>();
		Flags[1] = Bases[1].transform.FindChild("Flag");
	}

	public void AddFlagObserver(GameObject observer)
	{
		_flagObservers.Add(observer);
	}

	public void OnFlagStateChange()
	{
		foreach(GameObject observer in _flagObservers)
		{
			observer.SendMessage("OnFlagStateChange");
		}
	}

	public NetworkPlayer GetMyPlayer()
	{
		return _myPlayer;
	}

	public void SetMyPlayer(NetworkPlayer player)
	{
		_myPlayer = player;
	}

	public void OnScore(int scoringTeam)
	{
		if (FlagGameManager.Instance.GetMyPlayer().GetTeam() == scoringTeam)
		{
			_myTeamScore++;
			MyScoreLabel.text = "Your Score: " + _myTeamScore;
		}
		else
		{
			_theirScore++;
			TheirScoreLabel.text = "Their Score: " + _theirScore;
		}
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
