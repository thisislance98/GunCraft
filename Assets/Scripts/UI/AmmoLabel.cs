using UnityEngine;
using System.Collections;

public class AmmoLabel : MonoBehaviour {

	vp_FPSPlayer player;
	UILabel label;

	// Use this for initialization
	void Start () {

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPSPlayer>();
		label = GetComponent<UILabel>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (player.CurrentShooter != null)
		{
			label.text = player.CurrentShooter.GetAmmoText();
		}
	}
}
