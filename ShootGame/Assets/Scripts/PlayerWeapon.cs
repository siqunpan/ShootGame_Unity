using UnityEngine;
using System.Collections;

public class PlayerWeapon : Weapon {
	
	// Use this for initialization
	void Start () {
	
	}
	
	public override void Attack(Enum_shotMode shotMode, bool _b_isEnemy)
	{
		switch (shotMode)
		{
		case Enum_shotMode.Normal:
			GameObject go_bulletNormal = null;
			go_bulletNormal = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                             this.gameObject.transform.position + new Vector3 (1.099044f, -0.5950161f, 0f),
			                                         this.transform.rotation) as GameObject;
//			go_bulletNormal.GetComponent<Bullet>().v2_speed = v2_speedNomalShot;

			break;
		case Enum_shotMode.DeuxLine:
			GameObject[] array_bulletNormal = new GameObject[2];

			array_bulletNormal[0] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                             this.gameObject.transform.position + new Vector3 (1.099044f, -0.5950161f, 0f),
			                                             this.transform.rotation) as GameObject;
			array_bulletNormal[1] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                               this.gameObject.transform.position + new Vector3 (1.099044f, -0.5950161f, 0f),
			                                               this.transform.rotation) as GameObject;

			array_bulletNormal[0].transform.Rotate(0f, 0f, 45f);
			array_bulletNormal[1].transform.Rotate(0f, 0f, -45f);

			break;
		case Enum_shotMode.TestMode:
			GameObject[] array_bulletTsetMode = new GameObject[4];
			
			array_bulletTsetMode[0] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                               this.gameObject.transform.position + new Vector3 (1.099044f, -0.6f, 0f),
			                                                 this.transform.rotation) as GameObject;
			array_bulletTsetMode[1] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                               this.gameObject.transform.position + new Vector3 (1.099044f, -0.2f, 0f),
			                                                 this.transform.rotation) as GameObject;
			array_bulletTsetMode[2] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                               this.gameObject.transform.position + new Vector3 (1.099044f, 0.2f, 0f),
			                                                 this.transform.rotation) as GameObject;
			array_bulletTsetMode[3] = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                               this.gameObject.transform.position + new Vector3 (1.099044f, 0.6f, 0f),
			                                                 this.transform.rotation) as GameObject;
			break;
		case Enum_shotMode.Spiral:


			break;
		default:
			break;
		}
	}
}
