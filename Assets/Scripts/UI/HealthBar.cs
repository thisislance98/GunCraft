using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	vp_FPSPlayer player;
	
	float _startScaleX;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPSPlayer>();
		_startScaleX = transform.localScale.x;

	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 scale = transform.localScale;
		scale.x = (player.m_Health / player.MaxHealth) * _startScaleX;
		transform.localScale = scale;
	}
}
