using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour {


	public float TotalTime;
	UILabel _label;

	float _timeLeft;

	public float TimeLeft
	{
		get { return _timeLeft; }
		
		set 
		{
			_timeLeft = value;

			if (_timeLeft <= 0)
			{
				_timeLeft = 0;
				PhotonNetwork.room.customProperties["StartTime"] = PhotonNetwork.time;
				FlagGameManager.Instance.OnTimeUp();
			}

			int minutes = (int)(_timeLeft / 60);
			int seconds = ((int)_timeLeft) % 60;
			
			_label.text = minutes.ToString() + ":" + seconds.ToString("D2");

		}
		
	}

	// Use this for initialization
	void Awake () {

		_label = GetComponent<UILabel>();
		TimeLeft = TotalTime;
	}
	
	// Update is called once per frame
	void Update () {

		if (PhotonNetwork.room == null || PhotonNetwork.room.customProperties["StartTime"] == null)
			return;

		double startTime = (double)PhotonNetwork.room.customProperties["StartTime"];
		
	//	TimeLeft = TotalTime - (float)( PhotonNetwork.time - startTime );



	}
}
