using UnityEngine;
using System.Collections;

public class TextureManager : MonoBehaviour {
	
	
	public Texture2D[] Textures;
	public int[] AvailableBlockTextures;
	public UILabel availableBlocksLabel;
	public int GoldTextureIndex;
	public int PickupTextureIndex;
	
	int currentIndex;
	public static TextureManager Instance;
	
	UITexture uiTexture;
	
	// Use this for initialization
	void Awake () {
		Instance = this;
		uiTexture = GetComponent<UITexture>();
		uiTexture.mainTexture = Textures[currentIndex];
		
	}

	public void OnCreatedBlock(int blockDensity)
	{
		AvailableBlockTextures[blockDensity-1]--;

		if (AvailableBlockTextures[blockDensity-1] < 0)
			AvailableBlockTextures[blockDensity-1] = 0;
		
		if (currentIndex == blockDensity-1)
			availableBlocksLabel.text = AvailableBlockTextures[currentIndex].ToString();
	}


	public void OnDestroyedBlock(int blockDensity)
	{
		AvailableBlockTextures[blockDensity-1]++;

		if (currentIndex == blockDensity-1)
			availableBlocksLabel.text = AvailableBlockTextures[currentIndex].ToString();
	}

	public int GetAvaiableBlocks(int blockDensity)
	{
		if (blockDensity == 0)
			return 1;
		else
			return AvailableBlockTextures[blockDensity-1];
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

		availableBlocksLabel.text = AvailableBlockTextures[currentIndex].ToString();
	}
	
	void PreviousTexture()
	{
		currentIndex = ((currentIndex - 1) >= 0) ? (currentIndex - 1) : Textures.Length-1;
		uiTexture.mainTexture = Textures[currentIndex];

		availableBlocksLabel.text = AvailableBlockTextures[currentIndex].ToString();
	}	

}
