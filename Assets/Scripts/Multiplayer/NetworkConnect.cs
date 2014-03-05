using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkConnect : Photon.MonoBehaviour
{

    /*
     * We want this script to automatically connect to Photon and to enter a room.
     * This will help speed up debugging in the next tutorials.
     * 
     * In Awake we connect to the Photon server(/cloud).
     * Via OnConnectedToPhoton(); we will either join an existing room (if any), otherwise create one. 
     */

    void Start()
    {

		PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);


        PhotonNetwork.ConnectUsingSettings("1.0");
    }


//    void OnGUI()
//    {
//        //Check connection state..
//        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
//        {
//            //We are currently disconnected
//            GUILayout.Label("Connection status: Disconnected");
//
//            GUILayout.BeginVertical();
//            if (GUILayout.Button("Connect"))
//            {
//                //Connect using the PUN wizard settings (Self-hosted server or Photon cloud)
//                PhotonNetwork.ConnectUsingSettings("1.0");
//            }
//            GUILayout.EndVertical();
//        }
//        else
//        {
//            //We're connected!
//            if (PhotonNetwork.connectionState == ConnectionState.Connected)
//            {
//                GUILayout.Label("Connection status: Connected");
//                if (PhotonNetwork.room != null)
//                {
//                    GUILayout.Label("Room: " + PhotonNetwork.room.name);
//                    GUILayout.Label("Players: " + PhotonNetwork.room.playerCount + "/" + PhotonNetwork.room.maxPlayers);
//
//                }
//                else
//                {
//                    GUILayout.Label("Not inside any room");
//                }
//
//                GUILayout.Label("Ping to server: " + PhotonNetwork.GetPing());
//            }
//            else
//            {
//                //Connecting...
//                GUILayout.Label("Connection status: " + PhotonNetwork.connectionState);
//            }
//        }
//    }

    private bool receivedRoomList = false;

	void OnJoinedRoom()
	{
		int[] numInTeam = new int[2];
		int team;

		foreach (PhotonPlayer photonPlayer in PhotonNetwork.otherPlayers)
		{
			if (photonPlayer.customProperties.ContainsKey("Team") == false)
			{
				Debug.Log("Error: player does not have team assigned");
				continue;
			}

			numInTeam[(int)photonPlayer.customProperties["Team"]]++;

		}


		team = (numInTeam[0] > numInTeam[1]) ? 1 : 0;
		Hashtable hash = new Hashtable();
		hash["Team"] = team;
		PhotonNetwork.player.SetCustomProperties(hash);

		Debug.Log("joined room " + PhotonNetwork.playerList.Length);
		vp_FPSPlayer player = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPSPlayer>();
		player.OnPlayerConnected(team);
	}

    void OnConnectedToPhoton()
    {
        StartCoroutine(JoinOrCreateRoom());
    }

    void OnDisconnectedFromPhoton()
    {
        receivedRoomList = false;
    }

   



    
    /// <summary>
    /// Helper function to speed up our testing: 
    /// - after connecting to Photon, check for active rooms and join the first if possible
    /// - if no roomlist was found within 2 seconds: Create a room
    /// </summary>
    /// <returns></returns>
    IEnumerator JoinOrCreateRoom()
    {
        float timeOut = Time.time + 2;
        while (Time.time < timeOut && !receivedRoomList)
        {
            yield return 0;
        }
        //We still didn't join any room: create one
        if (PhotonNetwork.room == null){
            string roomName = "TestRoom"+Application.loadedLevelName;
            PhotonNetwork.CreateRoom(roomName, true, true, 16);
        }
    }
    
    /// <summary>
    /// Not used in this script, just to show how list updates are handled.
    /// </summary>
    void OnReceivedRoomListUpdate()
    {
        Debug.Log("We received a room list update, total rooms now: " + PhotonNetwork.GetRoomList().Length);

        string wantedRoomName = "TestRoom" + Application.loadedLevelName;
        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            if (room.name == wantedRoomName)
            {
                PhotonNetwork.JoinRoom(room.name);
                break;
            }
        }
        receivedRoomList = true;
    }
}
