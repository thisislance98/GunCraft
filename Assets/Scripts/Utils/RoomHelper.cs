using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomHelper {


	public static T Get<T>(string key, T defaultVal)
	{
		T val = defaultVal;


		if (PhotonNetwork.room.customProperties[key] != null)
			val = (T)PhotonNetwork.room.customProperties[key];


		return val;
	}

	public static void Set<T>(string key, T val)
	{
		Hashtable hash = new Hashtable();

		hash[key] = val;

		PhotonNetwork.room.SetCustomProperties(hash);

	}

}
