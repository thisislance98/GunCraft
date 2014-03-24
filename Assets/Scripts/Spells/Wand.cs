using UnityEngine;
using System.Collections;

public class Wand : MonoBehaviour {
	
	public GameObject SetHomeLabel;
	public GameObject WandProjectilePrefab;
	public float ManaForProjectile = 30;
	

	
	// Update is called once per frame
	void Update () {
		
		if (SetHomeLabel.activeSelf)
			return;
		
		bool leftButton = Input.GetMouseButtonDown(0);
		bool rightButton = Input.GetMouseButtonDown(1);
		
		if (leftButton || rightButton)
		{

				Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2,0));
				GameObject projectile = (GameObject)Instantiate(WandProjectilePrefab,pos,Quaternion.identity);
				
				ProjectileType type;
				if (SpellManager.Instance.GetCurrentSpell() != null)
					type = ProjectileType.Spell;
				else
					type = (leftButton) ? ProjectileType.Destroyer : ProjectileType.Creator;
			
				projectile.SendMessage("SetProjectileType",type,SendMessageOptions.DontRequireReceiver);
			
		}
	
	}

}
