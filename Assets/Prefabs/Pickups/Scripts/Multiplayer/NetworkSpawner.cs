using UnityEngine;
using System.Collections;

public class NetworkSpawner : Photon.MonoBehaviour
{
   
	public UILabel StatusLabel;
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
        
		StatusLabel.enabled = true;
		StatusLabel.text = "Bad Internet Connection.. Restarting in 5 seconds";
        /* 
        * To reset the scene we'll just reload it:
        */
        
		StartCoroutine(RestartInSeconds(5));
    }

	IEnumerator RestartInSeconds(float time)
	{
		yield return new WaitForSeconds(time);

		Application.LoadLevel(Application.loadedLevel);

	}

}