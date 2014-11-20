using UnityEngine;
using System.Collections;

public class EnemyWeapon : Weapon {

	// Use this for initialization
	void Start () {
	
	}

	public override void Attack(Enum_shotMode shotMode)
	{

	}

	public void AttackAI()
	{
		GameObject.Instantiate(GlobalVariables.GO_BULLETENEMY,
		                       this.gameObject.transform.position + new Vector3 (-0.3250118f, 0f, 0f), Quaternion.identity);
	}
}
