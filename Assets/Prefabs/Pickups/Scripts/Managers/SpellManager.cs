using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

	public Transform[] spells;
	
	Transform currentSpell;
	
	public static SpellManager Instance;
	
	// Use this for initialization
	void Start () {
		
		Instance = this;
		
	}
	
	public Transform GetCurrentSpell()
	{
		return currentSpell;
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i=0; i <= 9; i++)
		{
			if (Input.GetKey(i.ToString()))
			{
				if (spells.Length >= i)
					currentSpell = spells[i];
				
			}
		}
	}
}
