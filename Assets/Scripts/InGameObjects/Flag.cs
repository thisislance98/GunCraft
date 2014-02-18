using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	public Base TeamBase;
	
	void OnPlayerTouch(vp_FPSPlayer player)
	{


	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{

			if (other.GetComponent<vp_FPSPlayer>().GetTeam() == TeamBase.Team)
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
				transform.parent = other.transform;
				FlagGameManager.Instance.IsHoldingFlag = true;
			}

		}

	}



}
