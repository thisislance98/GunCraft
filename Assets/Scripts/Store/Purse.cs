using UnityEngine;
using System.Collections;

public class Purse : MonoBehaviour {
	
	int gold;
	
	public static Purse Instance;
	
	void Start()
	{
		gold = PlayerPrefs.GetInt("Gold",0);
		Instance = this;
		UpdateUI();
	}
	
	public int Gold {
		get {return gold;}

	}
	
	public void AddGold(int amount)
	{
		gold += amount;
		UpdateUI();
		PlayerPrefs.SetInt("Gold",gold);
	}
	
	public void RemoveGold(int amount)
	{
		gold -= amount;
		UpdateUI();
		PlayerPrefs.SetInt("Gold",gold);
	}	
	void UpdateUI()
	{
		GetComponent<UILabel>().text = "Gold: " + gold;
	}
}
