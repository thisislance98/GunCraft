using UnityEngine;
using System.Collections;

public class TextureManager : MonoBehaviour {
	
	
	public Texture2D[] Textures;
	
	int currentIndex;
	public static TextureManager Instance;
	
	UITexture uiTexture;
	
	// Use this for initialization
	void Awake () {
		Instance = this;
		uiTexture = GetComponent<UITexture>();
		uiTexture.mainTexture = Textures[currentIndex];
		
	}
	
	public int GetTextureIndex()
	{
		return currentIndex;	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
			NextTexture();
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
			PreviousTexture();
	}
	
	void NextTexture()
	{
		currentIndex = (currentIndex + 1) % Textures.Length;
		uiTexture.mainTexture = Textures[currentIndex];
		
	}
	
	void PreviousTexture()
	{
		currentIndex = ((currentIndex - 1) >= 0) ? (currentIndex - 1) : Textures.Length-1;
		uiTexture.mainTexture = Textures[currentIndex];
		
	}	

}
