using UnityEngine;
using System.Collections;

public class XRay : MonoBehaviour {

	public UILabel TimerLabel;
	public float MaxTime = 120;
	float _timeLeft;
	bool _isActive = true;

	public static XRay Instance;

	public float TimeLeft
	{
		get { return _timeLeft; }

		set 
		{
			_timeLeft = value;

			int minutes = (int)(_timeLeft / 60);
			int seconds = ((int)_timeLeft) % 60;

			TimerLabel.text = minutes.ToString() + ":" + seconds.ToString("D2");
		}

	}

	// Use this for initialization
	void Start () {
		Instance = this;
		TimeLeft = MaxTime;
		Activate(false);
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void Activate(bool active)
	{
		for (int i=0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(active);
		}

		if (active)
			TimeLeft = MaxTime;

		_isActive = active;
	}

	void Update()
	{
		if (TimeLeft > 0 && _isActive)
		{
			TimeLeft -= Time.deltaTime;

			if (TimeLeft <= 0)
				Activate(false);
		}

	}


}
