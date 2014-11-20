using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager_V2 : MonoBehaviour {

	private int i_maxNumPlayers = 1;
	private List<LevelDescription> list_levels = new List<LevelDescription>();
	private string s_pathToSaveXml = "Assets/Scripts/TD2/Levels/Levels.xml";
	private int i_currentLevel = 1;    //attention when use i_currentLevel as index, you should -1
	private int i_maxLevels = 0;
	private float f_timerLevel = 0f;
	private string[] sArray_nameOfAllPlayers;
	private int[] iArray_idOfAllPlayers;
	private int[] iArray_sideOfAllPlayers;
	private const int NEW_PLAYER_JOINED = 1;
	private const int EXIST_PLAYER_EXITED = 2;

	private int i_idNextEnemy = 0;
	private int i_idnextEnemyTotalInGame = 0;
	private int i_numMaxEnemies = 0;
	private GameObject[] goArray_allEnemies;

	private Enum_playMode playMode = Enum_playMode.Non;
//	private GUIManager guiManager;
	private GameInfo gameInfo;

	private class PlayerInfo
	{
		public int i_idPlayerInGame = -1;
		public int i_idPlayerInNetwork = -1;
		public int i_numLives = -1;
		public string s_playerName = null;
//		public float f_healthCurrentLife = -1;
		public int i_sidePlayer = -1; // for using in the future: player VS player
		public GameObject go_player = null;
	}

	private PlayerInfo[] array_allPlayersInfos;
	private int i_idMyPlayerInGame = -1;
	private int i_idMyPlayerInNetwork = -1;

	private InputController inputController;

	private bool b_quitGame = false;   //player can set this in the inputController when clicking the quitGame button

	public bool B_quitGame
	{
		get{return b_quitGame;}
		set{b_quitGame = value;}
	}

	// Use this for initialization
	void Start () {
		initGame ();
		initLevels ();
	}

	private void initGame()
	{
		inputController = FindObjectOfType (typeof(InputController)) as InputController;

		gameInfo = FindObjectOfType (typeof(GameInfo)) as GameInfo;
		i_maxNumPlayers = gameInfo.I_maxNumPlayers;
		playMode = gameInfo.Enum_playMode;

		sArray_nameOfAllPlayers = new string[i_maxNumPlayers];
		sArray_nameOfAllPlayers = gameInfo.getAllPlayersName();
		
		iArray_idOfAllPlayers = new int[i_maxNumPlayers];
		iArray_idOfAllPlayers = gameInfo.getAllPlayersId ();;

		iArray_sideOfAllPlayers = new int[i_maxNumPlayers];
		iArray_sideOfAllPlayers = gameInfo.getAllPlayersSides ();

		array_allPlayersInfos = new PlayerInfo[i_maxNumPlayers];
		for(int i = 0; i < i_maxNumPlayers; ++i)    //index 0 is the server, others are the clients
		{
			array_allPlayersInfos[i] = new PlayerInfo();
			
			array_allPlayersInfos[i].i_idPlayerInGame = i;
			array_allPlayersInfos[i].i_idPlayerInNetwork = iArray_idOfAllPlayers[i];
			array_allPlayersInfos[i].i_numLives = GlobalVariables.I_MAX_NUM_LIVES_PLAYER;
			array_allPlayersInfos[i].s_playerName = sArray_nameOfAllPlayers[i];
//			array_allPlayersInfos[i].f_healthCurrentLife = GlobalVariables.F_FULL_HEALTH_PLAYER;   //need assign value after
			array_allPlayersInfos[i].i_sidePlayer = iArray_sideOfAllPlayers[i];
			array_allPlayersInfos[i].go_player = null;

			if(iArray_idOfAllPlayers[i] == int.Parse(Network.player.ToString()))
			{
				i_idMyPlayerInGame = i;
				inputController.setSidePlayer (array_allPlayersInfos[i_idMyPlayerInGame].i_sidePlayer);
			}
		}

		i_idMyPlayerInNetwork = int.Parse (Network.player.ToString());

		if(!Network.isServer && !Network.isClient)
		{
			i_idMyPlayerInGame = 0;
			i_idMyPlayerInNetwork = -1;
		}

		createPlayer ();
	}

	private void initLevels()
	{
		initNewLevelData ();

		list_levels = XmlHelpers.LoadFromTextAsset<LevelDescription> (GlobalVariables.TEXT_LEVELS, null);
		
		i_maxLevels = list_levels.Count;

		for(int i = 0; i < i_maxLevels; i++)
		{
			if(list_levels[i].Enemies != null)
			{
				i_numMaxEnemies += list_levels[i].Enemies.Length;
			}
		}

		goArray_allEnemies = new GameObject[i_numMaxEnemies];
	}

	public void initNewLevelData()
	{
		f_timerLevel = 0f;
		i_idNextEnemy = 0;
		i_idnextEnemyTotalInGame = 0;
		i_numMaxEnemies = 0;
	}

	// Update is called once per frame
	void Update () {
		f_timerLevel += Time.deltaTime;

		if(canCreatePlayer())
		{
			createPlayer();
		}

		if(canCreateEnemy ())
		{
			createEnemy();
		}

		if(canLevelUp())
		{
			levelUp ();
		}

		if(canFinishGame())
		{
			quitGame();
		}
	}

	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.DestroyPlayerObjects (player);

		int i_idExitPlayerInNetwork = int.Parse (player.ToString());

		for(int i = 0; i < i_maxNumPlayers; ++i)
		{
			if(gameInfo.getPlayerIdInNetworkByIndex(i) == i_idExitPlayerInNetwork)
			{
				networkView.RPC("updateGameInfoRPC", RPCMode.All, i, i_idExitPlayerInNetwork, EXIST_PLAYER_EXITED);
			}
		}
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		gameInfo.Enum_quitGameMode = Enum_quitGameMode.QuitDuringGame;
		Application.LoadLevel ("Menu");
	}

	public void quitGame ()
	{

		if(b_quitGame == true)
		{
			gameInfo.Enum_quitGameMode = Enum_quitGameMode.QuitDuringGame;

		}
		else
		{
			gameInfo.Enum_quitGameMode = Enum_quitGameMode.FinishGame;
		}

		Application.LoadLevel ("Menu");
	}

	public bool canFinishGame()
	{
		bool b_canFinishgame = false;

		if(b_quitGame)
		{
			b_canFinishgame = true;
		}
		else
		{
			if(canFinishCurrentLevel() && i_currentLevel == i_maxLevels)
			{
				b_canFinishgame = true;
			}
		}

		return b_canFinishgame;
	}

	public bool canFinishCurrentLevel()
	{
		bool b_canFinishCurrentLevel = false;

//		if(list_levels [i_currentLevel-1].Enemies != null)
//		{
//			if(i_idNextEnemy >= list_levels [i_currentLevel-1].Enemies.Length)
//			{
//				b_canFinishCurrentLevel = true;
//			}
//		}

		if(f_timerLevel >= list_levels [i_currentLevel-1].Duration)
		{
			b_canFinishCurrentLevel = true;
		}

//		if(array_allPlayersInfos[i_idMyPlayerInGame].i_numLives < 0
//		   && array_allPlayersInfos[i_idMyPlayerInGame].f_healthCurrentLife <= 0)
		if(array_allPlayersInfos[i_idMyPlayerInGame].i_numLives < 0
		   && array_allPlayersInfos[i_idMyPlayerInGame].go_player == null)
		{
			b_canFinishCurrentLevel = false;
		}

		return b_canFinishCurrentLevel;
	}

	public bool canLevelUp()
	{
		bool b_canLevelUp = false;

		if(canFinishCurrentLevel() && i_currentLevel < i_maxLevels)
		{
			b_canLevelUp = true;
		}

		return b_canLevelUp;
	}

	public void levelUp()
	{
		if(Network.isServer)
		{
			networkView.RPC("levelUpRPC", RPCMode.All, i_currentLevel);
		}
		else if(Network.isClient)
		{
			//client can't level up by itself
		}
		else
		{
			initNewLevelData();
			i_currentLevel++;
		}

	}

	public bool canCreatePlayer()
	{
		bool b_canCreatePlayer = false;

		if(array_allPlayersInfos[i_idMyPlayerInGame].go_player == null)
		{
			if(array_allPlayersInfos[i_idMyPlayerInGame].i_numLives > 0)
			{
				b_canCreatePlayer = true;
			}
		}

		return b_canCreatePlayer;
	}
	
	public void createPlayer()
	{
		if(Network.isServer)
		{
			server_createPlayer(i_idMyPlayerInGame, i_idMyPlayerInNetwork);
		}
		else if(Network.isClient)
		{
			networkView.RPC("server_requestNewPlayerRPC", RPCMode.Server, i_idMyPlayerInGame, i_idMyPlayerInNetwork);
		}
		else
		{
			array_allPlayersInfos[i_idMyPlayerInGame].i_numLives --;
//			array_allPlayersInfos[i_idMyPlayerInGame].f_healthCurrentLife = GlobalVariables.F_FULL_HEALTH_PLAYER;
//			myPlayer = GameObject.Instantiate(GlobalVariables.GO_PLAYER, new Vector3(-25f, 2f, 0f), Quaternion.identity) as GameObject;
			array_allPlayersInfos[i_idMyPlayerInGame].go_player =
				GameObject.Instantiate(GlobalVariables.GO_PLAYER, new Vector3(-25f, 2f, 0f), Quaternion.identity) as GameObject;
		}
	}

	public void server_createPlayer(int _i_idPlayerInGame, int _i_idPlayerInNetwork)
	{
		if(Network.isServer)
		{
			array_allPlayersInfos[_i_idPlayerInGame].i_numLives--;
//			array_allPlayersInfos[_i_idPlayerInGame].f_healthCurrentLife = GlobalVariables.F_FULL_HEALTH_PLAYER;

			if(array_allPlayersInfos[_i_idPlayerInGame].go_player != null)
			{
				Destroy(array_allPlayersInfos[_i_idPlayerInGame].go_player);
			}

			array_allPlayersInfos[_i_idPlayerInGame].go_player = 
				GameObject.Instantiate (GlobalVariables.GO_PLAYER, new Vector3(-25f, 2f, 0f), Quaternion.identity) as GameObject;
			
			array_allPlayersInfos [_i_idPlayerInGame].go_player.GetComponent<PlayerAvatar> ().initThisPlayer (_i_idPlayerInGame, i_idMyPlayerInGame);
			
			NetworkViewID viewID = Network.AllocateViewID ();
			array_allPlayersInfos [_i_idPlayerInGame].go_player.networkView.viewID = viewID;
			
			networkView.RPC("client_createPlayerInGameRPC", RPCMode.Others, _i_idPlayerInGame, _i_idPlayerInNetwork, viewID);
		}
	}

	public bool canCreateEnemy ()
	{
		bool b_canCreateEnemy = false;

		if(i_currentLevel <= i_maxLevels)
		{
			if(list_levels[i_currentLevel-1].Enemies != null
			   && i_idNextEnemy < list_levels[i_currentLevel-1].Enemies.Length
			   && f_timerLevel >= list_levels [i_currentLevel-1].Enemies[i_idNextEnemy].SpawnDate)
			{
				return true;
			}
		}

		return b_canCreateEnemy;
	}

	public void createEnemy()
	{
		Vector3 v3_nextEnemyPosition = Vector3.zero;

		if(Network.isServer)   //just server demande all players create enemies
		{
			v3_nextEnemyPosition.x = list_levels [i_currentLevel-1].Enemies [i_idNextEnemy].SpawnPosition.x;
			v3_nextEnemyPosition.y = list_levels [i_currentLevel-1].Enemies [i_idNextEnemy].SpawnPosition.y;
			v3_nextEnemyPosition.z = 0;

			networkView.RPC("createEnemyInGameRPC", RPCMode.All, i_idnextEnemyTotalInGame, i_idNextEnemy, v3_nextEnemyPosition);
		}
		else if(Network.isClient)   
		{
			//client can't create any enemies by itself in order to prevent cheating
		}
		else
		{
			v3_nextEnemyPosition.x = list_levels [i_currentLevel-1].Enemies [i_idNextEnemy].SpawnPosition.x;
			v3_nextEnemyPosition.y = list_levels [i_currentLevel-1].Enemies [i_idNextEnemy].SpawnPosition.y;
			v3_nextEnemyPosition.z = 0;

			 goArray_allEnemies[i_idnextEnemyTotalInGame] = 
		     	GameObject.Instantiate (GlobalVariables.GO_ENEMY, v3_nextEnemyPosition, Quaternion.identity) as GameObject;
			i_idNextEnemy ++;
			i_idnextEnemyTotalInGame ++;
		}
	}

	[RPC]
	public void createEnemyInGameRPC(int _i_idnextEnemyTotalInGame, int _i_idnextEnemy, Vector3 _v3_enemyPosition)
	{
		createEnemyInGameLocal (_i_idnextEnemyTotalInGame, _i_idnextEnemy, _v3_enemyPosition);
	}

	public void createEnemyInGameLocal(int _i_idnextEnemyTotalInGame, int _i_idnextEnemy, Vector3 _v3_enemyPosition)
	{
		goArray_allEnemies[_i_idnextEnemyTotalInGame] = 
			GameObject.Instantiate (GlobalVariables.GO_ENEMY,
			                        _v3_enemyPosition, 
			                        Quaternion.identity) as GameObject;

		i_idnextEnemyTotalInGame = _i_idnextEnemyTotalInGame + 1;
		i_idNextEnemy = _i_idnextEnemy + 1;
	}

	[RPC]
	public void server_requestNewPlayerRPC (int _i_idPlayerInGame, int _i_idPlayerInNetwork)
	{
		server_requestNewPlayerLocal (_i_idPlayerInGame, _i_idPlayerInNetwork);
	}

	public void server_requestNewPlayerLocal(int _i_idPlayerInGame, int _i_idPlayerInNetwork)
	{
		if(Network.isServer)
		{
			for(int i = 0; i < i_maxNumPlayers; ++i)
			{
				if(array_allPlayersInfos[i].i_idPlayerInNetwork == _i_idPlayerInNetwork)
				{
//					if((array_allPlayersInfos[i].i_numLives == GlobalVariables.I_MAX_NUM_LIVES_PLAYER)
//					   || (array_allPlayersInfos[i].i_numLives < GlobalVariables.I_MAX_NUM_LIVES_PLAYER
//					   		&& array_allPlayersInfos[i].f_healthCurrentLife <= 0f))  //array_allPlayersInfos[i].f_healthCurrentLife <= 0 is to prevent cheated by clients
//					if((array_allPlayersInfos[i].i_numLives == GlobalVariables.I_MAX_NUM_LIVES_PLAYER)
//					   || (array_allPlayersInfos[i].i_numLives < GlobalVariables.I_MAX_NUM_LIVES_PLAYER
//					   		&& array_allPlayersInfos[i].go_player == null))  //array_allPlayersInfos[i].f_healthCurrentLife <= 0 is to prevent cheated by clients
					if(array_allPlayersInfos[i].go_player == null)
					{
						server_createPlayer(_i_idPlayerInGame, _i_idPlayerInNetwork);
					}
					i = i_maxNumPlayers;
				}
			}
		}
	}

	[RPC]
	public void client_createPlayerInGameRPC(int _i_idPlayerInGame, int _i_idPlayerInNetwork, NetworkViewID _viewID)
	{
		client_createPlayerInGameLocal (_i_idPlayerInGame, _i_idPlayerInNetwork, _viewID);
	}
	
	public void client_createPlayerInGameLocal(int _i_idPlayerInGame, int _i_idPlayerInNetwork, NetworkViewID _viewID)
	{
		if(Network.isClient)
		{
			array_allPlayersInfos[_i_idPlayerInGame].i_numLives--;
//			array_allPlayersInfos[_i_idPlayerInGame].f_healthCurrentLife = GlobalVariables.F_FULL_HEALTH_PLAYER;

			if(array_allPlayersInfos[_i_idPlayerInGame].go_player != null)
			{
				Destroy(array_allPlayersInfos[_i_idPlayerInGame].go_player);
			}

			array_allPlayersInfos[_i_idPlayerInGame].go_player = 
				GameObject.Instantiate (GlobalVariables.GO_PLAYER, new Vector3(-25f, 2f, 0f), Quaternion.identity) as GameObject;

			array_allPlayersInfos [_i_idPlayerInGame].go_player.networkView.viewID = _viewID;

			array_allPlayersInfos [_i_idPlayerInGame].go_player.GetComponent<PlayerAvatar> ().initThisPlayer (_i_idPlayerInGame, i_idMyPlayerInGame);
		}
	}

	[RPC]
	public void levelUpRPC(int _i_curentLevel)
	{
		levelUpLocal (_i_curentLevel);
	}

	public void levelUpLocal(int _i_curentLevel)
	{
		initNewLevelData ();

		i_currentLevel = _i_curentLevel + 1;
	}

	[RPC]
	public void updateGameInfoRPC(int _i_idUpdatePlayerInGame, int _i_idUpdatePlayerInNetwork, int _i_updateMode)
	{
		updateGameInfoLocal (_i_idUpdatePlayerInGame, _i_idUpdatePlayerInNetwork, _i_updateMode);

	}

	public void updateGameInfoLocal(int _i_idUpdatePlayerInGame, int _i_idUpdatePlayerInNetwork, int _i_updateMode)
	{
		if(_i_updateMode == NEW_PLAYER_JOINED)
		{
			//not need now
		}
		else if(_i_updateMode == EXIST_PLAYER_EXITED)
		{
			gameInfo.setPlayerIdInNetworkByIndex(_i_idUpdatePlayerInGame, -1);
			gameInfo.setPlayerNameByIndex(_i_idUpdatePlayerInGame, null);
			
			iArray_idOfAllPlayers[_i_idUpdatePlayerInGame] = -1;
			sArray_nameOfAllPlayers[_i_idUpdatePlayerInGame] = null;
			
			array_allPlayersInfos[_i_idUpdatePlayerInGame].i_idPlayerInGame = -1;
			array_allPlayersInfos[_i_idUpdatePlayerInGame].i_idPlayerInNetwork = -1;
			array_allPlayersInfos[_i_idUpdatePlayerInGame].i_numLives = GlobalVariables.I_MAX_NUM_LIVES_PLAYER;
			array_allPlayersInfos[_i_idUpdatePlayerInGame].s_playerName = null;
//			array_allPlayersInfos[_i_idUpdatePlayerInGame].f_healthCurrentLife = GlobalVariables.F_FULL_HEALTH_PLAYER;   //need assign value after
			array_allPlayersInfos[_i_idUpdatePlayerInGame].i_sidePlayer = -1;

			if(array_allPlayersInfos[_i_idUpdatePlayerInGame].go_player != null)
			{
				Destroy(array_allPlayersInfos[_i_idUpdatePlayerInGame].go_player);
			}
		}
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(500, 25, 150, 30), "Back"))
		{
			b_quitGame = true;
		}
	}


}

















