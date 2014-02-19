using UnityEngine;
using System.Collections;

public class ArrowToBase : MonoBehaviour {

	public bool PointToMyFlag;
	bool amHoldingFlag;

	// Use this for initialization
	void Start () {
		vp_Layer.Set(gameObject, vp_Layer.Weapon, true);
		FlagGameManager.Instance.AddFlagObserver(gameObject);
	}

	void OnFlagStateChange()
	{
		amHoldingFlag = FlagGameManager.Instance.GetMyPlayer().IsHoldingFlag();
	}

	// Update is called once per frame
	void Update () 
	{
		Vector3 dir;

		if (PointToMyFlag)
		{
			renderer.enabled = amHoldingFlag;
			dir = (FlagGameManager.Instance.GetMyFlagPosition() - transform.position);
		}
		else
		{
			renderer.enabled = !amHoldingFlag;

			dir = (FlagGameManager.Instance.GetTheirFlagPosition() - transform.position);
		}

		if (dir.y != 1 && dir.y != -1)
			transform.right = -dir;

	}
}
