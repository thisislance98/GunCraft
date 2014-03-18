using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	public Base TeamBase;

	public int GetTeam()
	{
		return TeamBase.Team;
	}

	void OnPlayerTouch(vp_FPSPlayer player)
	{


	}

	public void OnPlayerTriggerEnter(NetworkPlayer player)
	{
//		Debug.Log("our team: " + player.GetTeam() + " flag team: " + TeamBase.Team + " is holding: " + player.IsHoldingFlag() + " ismine: " + player.photonView.isMine);
		if (player.GetTeam() == TeamBase.Team)
		{
			// did we just score
			if (player.IsHoldingFlag())
			{
			
				FlagGameManager.Instance.OnScore(player.GetTeam());
				int captures = PlayerHelper.Get<int>(player.photonView.owner,"Captures",0);
				PlayerHelper.Set<int>(player.photonView.owner,"Captures",captures+1);

				NetworkPlayer.Instance.UpdateScore(player.photonView.owner);

				SetFlagHolder(null,player.GetTeam());

			}
		}
		else // just took their flag
		{

			SetFlagHolder(player.photonView,player.GetTeam());

		}

	}


	void SetFlagHolder(PhotonView holder, int team)
	{

		int viewId = (holder == null) ? -1 : holder.viewID;
		Debug.Log("setting flag holder: " + viewId + " for team: " + team);

		RoomHelper.Set<int>("Team" + team + "FlagHolderId",viewId);
		FlagGameManager.Instance.OnFlagStateChange();

	}

	void GrabFlag(PhotonView playerView)
	{
		Debug.Log("grabbing flag " + GetTeam());
		int otherTeam = (GetTeam() + 1) % 2;
		
		Transform theirFlag = FlagGameManager.Instance.GetFlag(otherTeam).transform;

		theirFlag.collider.enabled = false;
		theirFlag.parent = playerView.transform;
		theirFlag.localPosition = Vector3.zero;

	}

	void ReturnFlagToOtherBase()
	{
		int otherTeam = (GetTeam() + 1) % 2;
		
		Transform flag = FlagGameManager.Instance.GetFlag(otherTeam).transform;
		
		flag.parent = FlagGameManager.Instance.GetBase(otherTeam).transform;
		flag.localPosition = Vector3.zero;
		
		flag.collider.enabled = true;

		Debug.Log("returning flag: " + GetTeam());

	}

	public void UpdateFlag()
	{
		int holderId = RoomHelper.Get<int>("Team" + GetTeam() + "FlagHolderId",-1 );

		PhotonView flagHolder = null; 
	
//		Debug.Log("updating holder id: " + holderId + " for team: " + GetTeam());
		if (holderId != -1)
			flagHolder = PhotonView.Find(holderId );

		if (flagHolder == null)
			ReturnFlagToOtherBase();
		else
			GrabFlag(flagHolder);
	}

	void OnDroppedFlag(NetworkPlayer player)
	{
		SetFlagHolder(null,player.GetTeam());

	}

}
