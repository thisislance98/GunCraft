using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttributeListGrid : ListGrid {

	public string Attribute;

	protected override List<string> GetList()
	{
		sortedList.Clear();


		for (int i=0; i < PhotonNetwork.playerList.Length; i++)
		{
			int key = PhotonNetwork.playerList[i];
			string value = PlayerHelper.Get<int>(PhotonNetwork.playerList[i],Attribute,0).ToString();
			Debug.Log("adding key: " + key + " name: " + PhotonNetwork.playerList[i] +  " value: " + value + " for attibute: " + Attribute + " list count: " + sortedList.Count);

			sortedList.Add(key,value);
		}

		return new List<string>( sortedList.Values );
	}


}
