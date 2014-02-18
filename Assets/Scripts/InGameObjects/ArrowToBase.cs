using UnityEngine;
using System.Collections;

public class ArrowToBase : MonoBehaviour {

	public bool PointToMyFlag;

	// Use this for initialization
	void Start () {
		vp_Layer.Set(gameObject, vp_Layer.Weapon, true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dir;

		if (PointToMyFlag)
		{
			renderer.enabled = FlagGameManager.Instance.IsHoldingFlag;
			dir = (FlagGameManager.Instance.GetMyFlagPosition() - transform.position);
		}
		else
		{
			renderer.enabled = !FlagGameManager.Instance.IsHoldingFlag;

			dir = (FlagGameManager.Instance.GetTheirFlagPosition() - transform.position);
		}

		if (dir.y != 1 && dir.y != -1)
			transform.right = -dir;

	}
}
