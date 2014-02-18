using UnityEngine;
using System.Collections;

public class TalkButton : MonoBehaviour {


	GameObject _networkPlayerOwner = null;

	void OnPress(bool isPressed)
	{
		Debug.Log("talk: " + isPressed);

		if (_networkPlayerOwner == null)
			_networkPlayerOwner = GameObject.FindGameObjectWithTag("NetworkPlayerOwner");

		if (_networkPlayerOwner != null)
			_networkPlayerOwner.SendMessage("OnTalkButtonPress",isPressed);
	}
}
