using UnityEngine;
using System.Collections;

public class TeamCount : MonoBehaviour {

	public bool IsMine;
	int count = 0;

	// Use this for initialization
	void Start () {
		NetworkPlayer.AddPlayerObserver(gameObject);
	}
	
	void OnPlayerJoined(bool isMine)
	{
		Debug.Log("player joined");
		if (IsMine == isMine)
			count++;

		UpdateLabel();
	}

	void OnPlayerLeft(bool isMine)
	{
		if (IsMine == isMine)
			count--;

		UpdateLabel();
	}

	void UpdateLabel()
	{
		if (IsMine)
			GetComponent<UILabel>().text = "Us: " + count;
		else
			GetComponent<UILabel>().text = "Them: " + count;
	}
}
