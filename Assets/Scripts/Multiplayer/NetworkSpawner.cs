using UnityEngine;
using System.Collections;

public class NetworkSpawner : Photon.MonoBehaviour
{
   

    public Transform playerPrefab;


    void OnJoinedRoom()
    {
        Spawnplayer();
    }

    void Spawnplayer()
    {

        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
    }


    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("Clean up after player " + player);
  
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.Log("Clean up a bit after server quit");
        
        /* 
        * To reset the scene we'll just reload it:
        */
        Application.LoadLevel(Application.loadedLevel);
    }

}