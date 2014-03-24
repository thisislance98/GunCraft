using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour {

	List<string> itemNames = new List<string>(new string[] {"Revolver", "MachineGun"} );
	int[] 	 itemPrices = {5,			15};
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnBuyItem(GameObject itemObj)
	{
		if (!itemNames.Contains(itemObj.name))
		{
			// item obj name must be in itemNames list
			Debug.Break();
			return;
		}
		
		int price = itemPrices[itemNames.IndexOf(itemObj.name)];
		if (Purse.Instance.Gold >= price)
		{
			PlayerPrefs.SetInt(itemObj.name,PlayerPrefs.GetInt(itemObj.name,0)+1);
			Purse.Instance.RemoveGold(price);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
