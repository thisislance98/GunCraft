using UnityEngine;
using System.Collections;

public class ArrowToBase : MonoBehaviour {

	public bool PointToMyFlag;
	bool amHoldingFlag;
	bool areTheyHoldingOurFlag;

	// Use this for initialization
	void Start () {
		vp_Layer.Set(gameObject, vp_Layer.Weapon, true);
		FlagGameManager.Instance.AddFlagObserver(gameObject);
	}

	void OnFlagStateChange()
	{
		amHoldingFlag = FlagGameManager.Instance.GetMyPlayer().IsHoldingFlag();
		areTheyHoldingOurFlag = FlagGameManager.Instance.GetFlag(FlagGameManager.Instance.GetMyPlayer().GetTeam()).transform.parent.tag != "Base";
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 dir;

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
