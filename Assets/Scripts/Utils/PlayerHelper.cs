using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerHelper {


	public static T Get<T>(PhotonPlayer player, string key, T defaultVal)
	{
		T val = defaultVal;
		
		
		if (player.customProperties[key] != null)
			val = (T)player.customProperties[key];
		
		
		return val;
	}
	
	public static T Get<T>(string key, T defaultVal)
	{
		T val = defaultVal;
		
		
		if (PhotonNetwork.player.customProperties[key] != null)
			val = (T)PhotonNetwork.player.customProperties[key];
		
		
		return val;
	}
	
	public static void Set<T>(string key, T val)
	{
		Hashtable hash = new Hashtable();
		
		hash[key] = val;
		
		PhotonNetwork.player.SetCustomProperties(hash);
		
	}

	public static void Set<T>(PhotonPlayer player, string key, T val)
	{
		Hashtable hash = new Hashtable();
		
		hash[key] = val;
		
		player.SetCustomProperties(hash);
		
	}
	
}
