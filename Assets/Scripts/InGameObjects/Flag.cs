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
			
				ReturnFlagToOtherBase(player);

				FlagGameManager.Instance.OnScore(player.GetTeam());

			}
		}
		else // just took their flag
		{
			Debug.Log("took their flag");
			collider.enabled = false;
			transform.parent = player.transform;
			transform.localPosition = Vector3.zero;
			FlagGameManager.Instance.OnFlagStateChange();

		}

	}

	void ReturnFlagToOtherBase(NetworkPlayer player)
	{
		int otherTeam = (player.GetTeam() + 1) % 2;
		
		Transform flag = FlagGameManager.Instance.GetFlag(otherTeam).transform;
		
		flag.parent = FlagGameManager.Instance.GetBase(otherTeam).transform;
		flag.localPosition = Vector3.zero;
		
		flag.collider.enabled = true;
		FlagGameManager.Instance.OnFlagStateChange();
	}

	void OnDroppedFlag(NetworkPlayer player)
	{
		ReturnFlagToOtherBase(player);

	}

}
