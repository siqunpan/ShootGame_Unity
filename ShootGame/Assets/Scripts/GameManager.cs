using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private int i_maxNumLives = 3;
	private float f_durationCreateNewLife = 2f;
	private float f_timerCreateNewLife = 0f;

	private List<LevelDescription> list_levels = new List<LevelDescription>();
	private string s_pathToSaveXml = "Assets/Scripts/TD2/Levels/Levels.xml";
	private int i_currentLevel = 1;
	private int i_maxLevels = 0;
	private float f_timerLevel = 0f;
	private int i_idNextEnemy = 0;
//	private int i_numPlayers = 1;
//	private bool b_init = false;
	private Enum_playMode playMode = Enum_playMode.Non;
	private int i_maxNumPlayers = 1;
	private GUIManager guiManager;

	private class PlayerInfo
	{
		public int i_idPlayer;
		public int i_numLives;
		public GameObject go_player;
	}

	private PlayerInfo[] array_players;

	private List<PlayerInfo> list_players = new List<PlayerInfo>(); 

//	private void setNumPlayers(int _i_num)
//	{
//		i_numPlayers = _i_num;
//	}

	public int getCurrentLevel ()
	{
		return i_currentLevel;
	}

	public void levelUp()
	{
		i_currentLevel++;
	}

	// Use this for initialization
	void Start () {
		initGame ();
	}

	private void initGame()
	{
		guiManager = FindObjectOfType (typeof(GUIManager)) as GUIManager;
		i_maxNumPlayers = guiManager.getMaxNumClients ();
		playMode = guiManager.getCurrentPlayMode ();

		GameObject go_newPlayer;
		PlayerInfo newPlayerInfo;
		
		if(playMode == Enum_playMode.Solo)
		{
			newPlayerInfo = new PlayerInfo();
			
			go_newPlayer = GameObject.Instantiate (GlobalVariables.GO_PLAYER, new Vector3(-15f, 2f, 0f), Quaternion.identity) as GameObject;
			
			newPlayerInfo.go_player = go_newPlayer;
			newPlayerInfo.i_numLives = i_maxNumLives;
			newPlayerInfo.i_idPlayer = 0;
			
			list_players.Add (newPlayerInfo);
		}
		else if(playMode == Enum_playMode.MultiPlayer)
		{

		}

		initLevels ();
	}

	private void initLevels()
	{
		list_levels = XmlHelpers.LoadFromTextAsset<LevelDescription> (GlobalVariables.TEXT_LEVELS, null);

		i_maxLevels = list_levels.Count;
	}
	
	public void initDatas()
	{
		f_timerLevel = 0f;
		i_idNextEnemy = 0;
	}

	public void addNewLife(int _i_idPlayer)
	{
		GameObject go_newPlayer;

		go_newPlayer = GameObject.Instantiate (GlobalVariables.GO_PLAYER, new Vector3(-15f, 2f, 0f), Quaternion.identity) as GameObject;
		list_players[_i_idPlayer].go_player = go_newPlayer;

	}

	public bool canFinishCurrentLevel()
	{
		bool b_finish = false;

		if(f_timerLevel >= list_levels [i_currentLevel-1].Duration)
		{
			b_finish = true;
		}

		return b_finish;
	}

	public bool canFinishGame()
	{
		bool b_finishGame = false;

		if((canFinishCurrentLevel() && i_currentLevel == i_maxLevels)
		   || list_players[0].i_numLives <= 0)
		{
			b_finishGame = true;
		}
		else
		{
			b_finishGame = false;
		}

		return b_finishGame;
	}

	public bool canCreateEnemy()
	{
		bool b_canCreateEnemy = false;

		if(i_idNextEnemy == 0)
		{
			if(list_levels [i_currentLevel-1].Enemies == null)
			{
				b_canCreateEnemy = false;
				return b_canCreateEnemy;
			}
			else
			{
				b_canCreateEnemy = true;
				return b_canCreateEnemy;
			}
		}

		if (i_idNextEnemy < list_levels [i_currentLevel-1].Enemies.Length
		    && f_timerLevel >= list_levels [i_currentLevel-1].Enemies[i_idNextEnemy].SpawnDate)
		{
			b_canCreateEnemy = true;
		}
		else
		{
			b_canCreateEnemy = false;
		}

		return b_canCreateEnemy; 
	}

	public void createEnemy()
	{
		Instantiate (GlobalVariables.GO_ENEMY, 
		             list_levels [i_currentLevel-1].Enemies [i_idNextEnemy].SpawnPosition,
		             Quaternion.identity);
		i_idNextEnemy ++;
	}

	public void quitGame()
	{
		list_players.Clear ();
		Application.LoadLevel ("Menu");
	}

	// Update is called once per frame
	void Update () {

		f_timerLevel += Time.deltaTime;

		for (int i = 0; i < list_players.Count; i++) 
		{
			if(list_players[i].go_player == null)
			{
				if(list_players[i].i_numLives > 0)
				{
					addNewLife(i);
					list_players[i].i_numLives--;
				}
			}
		}

		if (canCreateEnemy ()) 
		{
			f_timerCreateNewLife += Time.deltaTime;

			if(f_timerCreateNewLife >= f_durationCreateNewLife)
			{
				createEnemy();
				f_timerCreateNewLife = 0f;
			}
		}

		if(canFinishCurrentLevel())
		{
			if(i_currentLevel < i_maxLevels)
			{
				levelUp();
				initDatas();
			}
		}

		if(canFinishGame())
		{
			Debug.Log("The Game Is Finished Now !!!!!!!");

			quitGame();
		}
	}
}
