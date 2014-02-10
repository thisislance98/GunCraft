using uLink;
using UnityEngine;

public class SpawnObject : uLink.MonoBehaviour
{
	//************************************************************************************************
	// Owner is the actual player using the client Scene. It has animations + camera.  
	// OwnerInit.cs connects the owner to the camera.
	//
	// Proxy is what appears on the the opponent players computers, It has animations but no camera connection.
	//
	// Creator is instansiated in the server and it has no camera. It has the animation script, but it has been 
	// deactivated.  Just turn it on if you really do want to see animations on the server.
	//************************************************************************************************
	
	public GameObject proxyPrefab = null;
	public GameObject ownerPrefab = null;
	public GameObject creatorPrefab = null;
	public GameObject spawnLocation = null;
		
	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		Debug.Log("player connected!");
		string loginName;
		if (!player.loginData.TryRead<string>(out loginName)) loginName = "Nameless";
		
		//Instantiates an avatar for the player connecting to the server
		//The player will be the "owner" of this object. Read the manual chapter 7 for more
		//info about object roles: Creator, Owner and Proxy.
		uLink.Network.Instantiate(player, proxyPrefab, ownerPrefab, creatorPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation, 0, loginName);
	}
	
}
