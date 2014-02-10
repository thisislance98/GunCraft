using UnityEngine;
using System.Collections;

public class EnemyTextureSetter : MonoBehaviour {
	
	public Texture[] textures;
	
	// Use this for initialization
	void Start () {
		renderer.material.SetTexture("_MainTex",textures[Random.Range(0,textures.Length)]);
	}
	

}
