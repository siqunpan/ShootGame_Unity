using UnityEngine;
using System.Collections;

public class PlayerWeapon : Weapon {

	private int i_shotMode = -1; //0:Non, 0:Normal, 2:DeuxLine, 3:TestMode, 4:Spiral

	// Use this for initialization
	void Start () {
	
	}
	
	public override void Attack(Enum_shotMode shotMode)
	{
		switch (shotMode)
		{
		case Enum_shotMode.Normal:
			i_shotMode = 1;
			break;
		case Enum_shotMode.DeuxLine:
			i_shotMode = 2;
			break;
		case Enum_shotMode.TestMode:
			i_shotMode = 3;
			break;
		case Enum_shotMode.Spiral:
			i_shotMode = 4;
			break;
		default:
			break;
		}

		if (!Network.isServer && !Network.isClient) 
		{
			AttackLocal(i_shotMode);	
		}
		else
		{
			networkView.RPC("AttackRPC", RPCMode.All, i_shotMode);
		}
	}

	[RPC]
	public void AttackRPC(int _i_shotMode)
	{
		AttackLocal (_i_shotMode);
	}

	public void AttackLocal(int _i_shotMode)
	{
		switch (_i_shotMode)
		{
		case 1:
			GameObject go_bulletNormal = null;
			
			go_bulletNormal = GameObject.Instantiate(GlobalVariables.GO_BULLETPLAYER, 
			                                         this.gameObject.transform.position + new Vector3 (1.099044f, -0.5950161f, 0f),
			                                         this.transform.rotation) as GameObject;
			//			go_bulletNormal.GetComponent<Bullet>().v2_speed = v2_speedNomalShot;
			
			break;
		case 2:
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
		case 3:
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
		case 4:
			break;
		default:
			break;
		}
	}
}
