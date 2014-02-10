using UnityEngine;
using System.Collections;

public class Glow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.ColorTo(gameObject,iTween.Hash("time",.5f,"color",Color.gray,"looptype",iTween.LoopType.pingPong));
	}
	
}
