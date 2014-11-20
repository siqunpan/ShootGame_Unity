using UnityEngine;
using System.Collections;

public class GlobalVariables : MonoBehaviour {

	public static GameObject GO_PLAYER;
	public GameObject go_player;
	
	public static GameObject GO_ENEMY;
	public GameObject go_enemy;

	public static GameObject GO_BULLETPLAYER;
	public GameObject go_bullet_player;

	public static GameObject GO_BULLETENEMY;
	public GameObject go_bullet_enemy;

//	public static GameObject GO_INPUTCONTROLLER;
//	public GameObject go_inputController;

	public static TextAsset TEXT_LEVELS;
	public TextAsset text_levels;

	public static float F_FULL_HEALTH_PLAYER;
	public float f_fullHealthPlayer;

	public static float F_FULL_HEALTH_ENEMY;
	public float f_fullHealthEnemy;

	public static int I_MAX_NUM_LIVES_PLAYER;
	public int i_maxNumLivesPlayer;

	public static GameInfo GO_GAMEINFO;
	public GameInfo go_gameInfo;

	// Use this for initialization
	void Start () {

		GO_PLAYER = go_player;
		GO_ENEMY = go_enemy;

		GO_BULLETPLAYER = go_bullet_player;
		GO_BULLETENEMY = go_bullet_enemy;

//		GO_INPUTCONTROLLER = go_inputController;

		TEXT_LEVELS = text_levels;

		F_FULL_HEALTH_PLAYER = f_fullHealthPlayer;
		F_FULL_HEALTH_ENEMY = f_fullHealthEnemy;

		I_MAX_NUM_LIVES_PLAYER = i_maxNumLivesPlayer;

		GO_GAMEINFO = go_gameInfo;
	}
}
