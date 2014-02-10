using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public GameObject InGameUI;
	public GameObject StoreUI;
	
	GameObject mainCamera;
	
	// Use this for initialization
	void Start () {
		mainCamera = Camera.mainCamera.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E))
		{
			
			InGameUI.SetActiveRecursively(!InGameUI.activeSelf);
			StoreUI.SetActiveRecursively(!StoreUI.activeSelf);
			
			
		//	Time.timeScale = (StoreUI.activeSelf) ? 0 : 1;
			mainCamera.SetActive(InGameUI.activeSelf);
			
			GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPSPlayer>().LockCursor = InGameUI.activeSelf;
			
			
//						if (player.activeSelf)
//				player.GetComponent<vp_FPSPlayer>().LockCursor = InGameUI.activeSelf;
		}
	}
}
