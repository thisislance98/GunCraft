using UnityEngine;
using System.Collections;

public class NameLabel : MonoBehaviour {

	Transform mainCamera;
	NetworkPlayer player;
	TextMesh textMesh;

	// Use this for initialization
	void Start () {
	
		player = transform.parent.GetComponent<NetworkPlayer>(); 

		if (player == NetworkPlayer.Instance)
			Destroy(gameObject);

		textMesh = GetComponent<TextMesh>();

		mainCamera = Camera.main.transform;


	}

	void SetTeamColor(Color color)
	{
		renderer.material.color = color;
		if (color == Color.red)
			renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.forward = transform.position - mainCamera.position;

		textMesh.text = (player.IsHoldingFlag()) ? player.photonView.owner.name + " has flag!" : player.photonView.owner.name;

	}
}
