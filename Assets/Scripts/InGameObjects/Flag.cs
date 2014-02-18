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

		if (player.GetTeam() == TeamBase.Team)
		{
			// did we just score
			if (FlagGameManager.Instance.IsHoldingFlag)
			{
				Transform theirFlag = FlagGameManager.Instance.GetTheirFlag();
				theirFlag.position = FlagGameManager.Instance.GetTheirBasePosition();
				theirFlag.parent = FlagGameManager.Instance.GetTheirBase().transform;
				theirFlag.collider.enabled = true;
				FlagGameManager.Instance.IsHoldingFlag = false;
				FlagGameManager.Instance.OnScore();
			}
		}
		else // just took their flag
		{
			collider.enabled = false;
			transform.parent = player.transform;
			transform.localPosition = Vector3.zero;
			FlagGameManager.Instance.IsHoldingFlag = true;
		}

	}

}
