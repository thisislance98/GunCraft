using UnityEngine;
using System.Collections;

public enum SpellType
{
	Movement,
	Transformation,
	Recorder,
	Destruction
}

public class Spell : MonoBehaviour {

	public SpellType SpellType = SpellType.Movement;
	
	public void HideParent()
	{
		transform.parent.SendMessage("OnHide",SendMessageOptions.DontRequireReceiver);
		transform.parent.renderer.enabled = false;
	}
	
	protected void DestroySpellsOfType(SpellType type)
	{
		foreach (Transform child in transform.parent)
		{
			Spell spell = child.GetComponent<Spell>();
			
			if (spell != null && spell.SpellType == type)
			{
				Destroy(child.gameObject);	
			}
		}
	}
}
