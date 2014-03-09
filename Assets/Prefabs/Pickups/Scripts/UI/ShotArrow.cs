using UnityEngine;
using System.Collections;

public class ShotArrow : MonoBehaviour {

	float _timeSinceHit;
	public Renderer ArrowRenderer;

	// Use this for initialization
	void Awake () {
		ArrowRenderer.enabled = false;
		Debug.Log("arrow awoke");
	}
	
	void OnGotHit(Vector3 shootingPos)
	{
		ArrowRenderer.enabled = true;
		_timeSinceHit = 0;

		transform.forward = (shootingPos + Vector3.up) - transform.position;
	}

	void Update()
	{
		_timeSinceHit += Time.deltaTime;

		if (ArrowRenderer.enabled && _timeSinceHit > 1)
			ArrowRenderer.enabled = false;

	}
}
