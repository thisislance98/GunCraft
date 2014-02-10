using UnityEngine;
using System.Collections;

public class MiniCubeParent : MonoBehaviour {
	
	Texture[] groundTextures;
	
	public void SetTexture(int index)
	{
		if (groundTextures == null)
			groundTextures = TextureManager.Instance.Textures;
		
		for (int i=0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			Texture tex = groundTextures[index];
			child.renderer.material.SetTexture("_MainTex",tex);
			
		}
	}
}
