using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TeamCount : MonoBehaviour {

	public bool IsMine;
	int count = 0;

	// Use this for initialization
	IEnumerator Start () {

		while (true)
		{
			yield return new WaitForSeconds(2);
			UpdateCount();
			UpdateLabel();

		}

	}

	public void UpdateCount()
	{
		if (NetworkPlayer.Instance == null)
			return;
		count = 0;

		foreach(PhotonPlayer player in PhotonNetwork.playerList)
		{
			if (IsMine)
			{
				if ((int)player.customProperties["Team"] == NetworkPlayer.Instance.GetTeam())
					count++;

			}
			else if ((int)player.customProperties["Team"] != NetworkPlayer.Instance.GetTeam())
				count++;


		}


	}
	
	void UpdateLabel()
	{
		if (IsMine)
			GetComponent<UILabel>().text = "Us: " + count;
		else
			GetComponent<UILabel>().text = "Them: " + count;
	}
}
