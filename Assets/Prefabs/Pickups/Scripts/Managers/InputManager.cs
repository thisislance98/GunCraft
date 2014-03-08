using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {


	Dictionary <int,Touch> _touches = new Dictionary<int, Touch>();
	List<GameObject> _observers = new List<GameObject>();

	public static InputManager Instance;

	// Use this for initialization
	void Awake () {

		Instance = this;
	
	}

	public void AddObserver(GameObject obj)
	{
		if (_observers.Contains(obj) == false)
			_observers.Add(obj);
	}

	void NotifyObservers(string message, Touch touch)
	{
		foreach(GameObject observer in _observers)
		{
			observer.SendMessage(message,touch,SendMessageOptions.DontRequireReceiver);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	//	_touches.Clear();

		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began)
			{
				OnTouchBegan(touch);

			}
			else if (touch.phase == TouchPhase.Stationary)
			{
				OnTouchStationary(touch);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				OnTouchMoved(touch);
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				OnTouchEnded(touch);
			}

		}

		// check if we missed on touched events here
		if (_touches.Count > Input.touches.Length)
		{
			List<Touch> realTouches = new List<Touch>(Input.touches);

			List<Touch> touchValues = new List<Touch>(_touches.Values);

			for (int i = touchValues.Count-1; i >= 0; i--)
			{
				Touch touch = touchValues[i];

				if (realTouches.Contains(touch) == false)
					OnTouchEnded(touch);
			}
		}

	
	}

	void OnTouchBegan(Touch touch)
	{
		// did it miss the touched ended?
		if (_touches.ContainsKey(touch.fingerId)) 
		{	
			OnTouchEnded(touch);
			Debug.Log("already has key: " + touch.fingerId);
		}
		_touches.Add(touch.fingerId, touch);

		NotifyObservers("OnTouchBegan",touch);
//		Debug.Log("touch started: " + touch.fingerId);
	}

	void OnTouchStationary(Touch touch)
	{
		// missed the touch began on this touch?
		if (_touches.ContainsKey(touch.fingerId) == false)
			OnTouchBegan(touch);

		NotifyObservers("OnTouchStationary",touch);
	}

	void OnTouchMoved(Touch touch)
	{
		// missed the touch began on this touch?
		if (_touches.ContainsKey(touch.fingerId) == false)
			OnTouchBegan(touch);

		NotifyObservers("OnTouchMoved",touch);
	//	Debug.Log("touch moved: " + touch.fingerId);
	}

	void OnTouchEnded(Touch touch)
	{
		// missed the touch began on this touch?
		if (_touches.ContainsKey(touch.fingerId) == false)
			OnTouchBegan(touch);

		_touches.Remove(touch.fingerId);

		NotifyObservers("OnTouchEnded",touch);

		if (touch.tapCount > 0)
			NotifyObservers("OnTouchTap",touch);
	//	Debug.Log("touch ended: " + touch.fingerId);
	}

	public bool HasTouch(int fingerId)
	{
		return _touches.ContainsKey(fingerId);
	}

	public Touch GetTouch(int fingerId)
	{
		return _touches[fingerId];
	}
}
