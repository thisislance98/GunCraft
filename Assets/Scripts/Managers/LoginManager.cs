using UnityEngine;
using System.Collections;

public class LoginManager : MonoBehaviour {

	public UILabel InputLabel;
	public GameObject ButtonObj;

	// Use this for initialization
	void Start () {
		ButtonObj.SetActive(false);

		if (PlayerPrefs.HasKey("PlayerName"))
			InputLabel.text = PlayerPrefs.GetString("PlayerName");
	}

	void Update()
	{
		if (InputLabel.text != "Type your name here" && InputLabel.text.Length > 1)
		{

			ButtonObj.SetActive(true);
		}
	}

	void OnStartClick()
	{
		PlayerPrefs.SetString("PlayerName",InputLabel.text);
		Application.LoadLevel("SinglePlayer");

	}


}
