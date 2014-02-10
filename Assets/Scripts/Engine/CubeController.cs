using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {
	
	
	public GameObject CubePrefab;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire2"))
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2));
			
			RaycastHit hit;
			
			Physics.Raycast(ray,out hit);
			
			if (hit.transform != null)
			{
				Vector3 pos = hit.point;
				pos.y++;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				pos.z = Mathf.Round(pos.z);
				
				Instantiate(CubePrefab,pos,Quaternion.identity);
			}
		}
	}
}
