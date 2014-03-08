using UnityEngine;
using System.Collections;

public class ArrowToBase : MonoBehaviour {

	public bool PointToMyFlag;
	public UILabel TheyHaveFlagLabel;
	public UILabel YouHaveFlagLabel;
	bool amHoldingFlag;
	bool areTheyHoldingOurFlag;

	// Use this for initialization
	void Start () {
		vp_Layer.Set(gameObject, vp_Layer.Weapon, true);
		FlagGameManager.Instance.AddFlagObserver(gameObject);
	}

	void OnFlagStateChange()
	{
		if (FlagGameManager.Instance == null)
			return;


		amHoldingFlag = FlagGameManager.Instance.GetMyPlayer().IsHoldingFlag();
		areTheyHoldingOurFlag = FlagGameManager.Instance.GetFlag(FlagGameManager.Instance.GetMyPlayer().GetTeam()).transform.parent.tag != "Base";
	}

	// Update is called once per frame
	void Update () 
	{
		if (FlagGameManager.Instance.IsTeamSet() == false)
			return;

		Vector3 dir;

		if (TheyHaveFlagLabel != null)
			TheyHaveFlagLabel.enabled = areTheyHoldingOurFlag;

		if (YouHaveFlagLabel != null)
			YouHaveFlagLabel.enabled = amHoldingFlag;

		if (PointToMyFlag) // is arrow to my flag
		{
			renderer.enabled = amHoldingFlag || areTheyHoldingOurFlag;
			dir = (FlagGameManager.Instance.GetMyFlagPosition() - transform.position);
		}
		else // is arrow to their flag
		{
			renderer.enabled = !amHoldingFlag;

			dir = (FlagGameManager.Instance.GetTheirFlagPosition() - transform.position);
		}

		if (dir.y != 1 && dir.y != -1)
			transform.right = -dir;

	}
}
