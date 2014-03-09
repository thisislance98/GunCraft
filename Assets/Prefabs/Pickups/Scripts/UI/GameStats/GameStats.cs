using UnityEngine;
using System.Collections;

public class GameStats : MonoBehaviour {

	public GameObject StatsPanel;

	vp_FPSPlayer _player;

	// Use this for initialization
	void Start () {
		_player = vp_FPSPlayer.Instance;
		StatsPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (Input.GetKeyDown(KeyCode.Tab) && _player.IsDead() == false)
		    StatsPanel.SetActive(true);

		if (Input.GetKeyUp(KeyCode.Tab) && _player.IsDead() == false)
			StatsPanel.SetActive(false);

		if (_player.IsDead() && StatsPanel.activeSelf == false)
		{
			StatsPanel.SetActive(true);
		}
		else if (_player.IsDead() == false && StatsPanel.activeSelf == true && Input.GetKey(KeyCode.Tab) == false)
		{
			StatsPanel.SetActive(false);
		}


	
	}
}
