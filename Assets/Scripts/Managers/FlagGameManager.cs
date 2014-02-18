using UnityEngine;
using System.Collections;

public class FlagGameManager : Photon.MonoBehaviour {

	public Base[] Bases;
	public Transform[] Flags;
	public UILabel TheirScoreLabel;
	public UILabel MyScoreLabel;

	public static FlagGameManager Instance;

	public bool IsHoldingFlag;
	int _myTeam;
	int _theirTeam;
	int _myTeamScore;
	int _theirScore;

	// Use this for initialization
	void Awake () {
		Instance = this;

	}

	public void OnScore()
	{
		photonView.RPC("DoScore",PhotonTargets.All);
	}

	[RPC]
	void DoScore()
	{
		if (photonView.isMine)
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
