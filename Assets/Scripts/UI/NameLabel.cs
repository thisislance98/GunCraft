using UnityEngine;
using System.Collections;

public class NameLabel : MonoBehaviour {

	Transform mainCamera;
	NetworkPlayer player;
	TextMesh textMesh;
	Color _labelColor;

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
		_labelColor = color;
		renderer.material.color = color;
		if (color == Color.red)
			renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.forward = transform.position - mainCamera.position;

		textMesh.text = (player.IsHoldingFlag()) ? player.photonView.owner.name + " has flag!" : player.photonView.owner.name;

		if (_labelColor == Color.red)
		{
			bool canSeePlayer = false;
			if (XRay.Instance.IsActive() == true)
				canSeePlayer = true;
			else
			{
				RaycastHit hit = new RaycastHit();
				Vector3 startPos = transform.position;
				Vector3 dir = (NetworkPlayer.Instance.transform.position + Vector3.up) - transform.position;
				Ray ray = new Ray(startPos,dir);

				if (Physics.Raycast(ray, out hit))
				{

					if (hit.transform.tag == "NetworkPlayerOwner" || hit.transform.tag == "Player")
						canSeePlayer = true;
				}
			}

			renderer.enabled = canSeePlayer;
		}

	}
}
