using UnityEngine;
using System.Collections;

public class JetPack : MonoBehaviour {

	vp_FPSController _controller;
	

	public static JetPack Instance;
	public UITexture CurrentFuelTexture;
	public UITexture TotalFuelTexture;
	public float FuelBurnRate = .2f;
	public float RefreshRate = .3f;
	public float CurrentToTotalRatio = .25f;

	float _currentFuelStartHeight;
	float _totalFuelStartHeight;
	float _currentFuel;
	float _totalFuel;
	bool _isActive = true;
	bool _canRefuel;
	

	public float CurrentFuel {
		get { return _currentFuel; }
		set
		{
			_currentFuel = value;
			Vector3 scale = CurrentFuelTexture.transform.localScale;
			scale.y = _currentFuel;
			CurrentFuelTexture.transform.localScale = scale;
		}
	}

	public float TotalFuel {
		get { return _totalFuel; }
		set
		{
			_totalFuel = value;
			Vector3 scale = TotalFuelTexture.transform.localScale;
			scale.y = _totalFuel;
			TotalFuelTexture.transform.localScale = scale;
		}
	}

	// Use this for initialization
	void Awake () {
		Instance = this;
		_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPSController>();
		_currentFuelStartHeight = CurrentFuelTexture.transform.localScale.y;
		_totalFuelStartHeight = TotalFuelTexture.transform.localScale.y;

		CurrentFuel = _currentFuelStartHeight;
		TotalFuel = _totalFuelStartHeight;

		Activate(false);
	}

	public void Activate(bool active)
	{
		for (int i=0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(active);
		}

		if (active)
		{
			TotalFuel = _totalFuelStartHeight;
			CurrentFuel = _currentFuelStartHeight;
		}

		_isActive = active;
	}

	public bool OnFuelUsed()
	{
		if (CurrentFuel <= 0 || _isActive == false)
			return false;

		CurrentFuel -= FuelBurnRate * _currentFuelStartHeight * Time.deltaTime;

		if (CurrentFuel < 0)
			CurrentFuel = 0;

		if (CurrentFuel <= 0 && TotalFuel <= 0)
		{
			Debug.Log("deactivating");
			CurrentFuel = _currentFuelStartHeight;
			TotalFuel = _totalFuelStartHeight;
			Activate(false);
		}

		return true;
	}

	void Update()
	{
		if ( _controller.IsGrounded())
			_canRefuel = true;
		else if (Input.GetKey(KeyCode.Space))
			_canRefuel = false;

		if (TotalFuel > 0 && CurrentFuel <= _currentFuelStartHeight && _canRefuel)
		{
			CurrentFuel += RefreshRate * _currentFuelStartHeight * Time.deltaTime;
			TotalFuel -= RefreshRate * CurrentToTotalRatio * _totalFuelStartHeight * Time.deltaTime;

		}
	}
	

}
