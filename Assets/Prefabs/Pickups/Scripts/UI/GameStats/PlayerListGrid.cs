using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerListGrid : ListGrid {


	protected override List<string> GetList()
	{
		sortedList.Clear();
		
		for (int i=0; i < PhotonNetwork.playerList.Length; i++)
		{
			sortedList.Add(PhotonNetwork.playerList[i],PhotonNetwork.playerList[i].name);
		}

		return new List<string>( sortedList.Values );
	}


}
