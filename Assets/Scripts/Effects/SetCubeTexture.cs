using UnityEngine;
using System.Collections;

public class SetCubeTexture : MonoBehaviour {

	Texture[] groundTextures;
	
	public void SetTexture(int index)
	{
		if (groundTextures == null)
			groundTextures = TextureManager.Instance.Textures;
		

		Texture tex = groundTextures[index];
		transform.renderer.material.SetTexture("_MainTex",tex);
			
	}
}
