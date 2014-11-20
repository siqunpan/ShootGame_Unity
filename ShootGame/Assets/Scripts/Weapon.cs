using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

//	public float f_damage = -1f;
	public float f_shotRate = 4f;
//	public Vector2 v2_speedNomalShot = Vector2.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	public virtual void Attack(Enum_shotMode shotMode)
	{

	}
}