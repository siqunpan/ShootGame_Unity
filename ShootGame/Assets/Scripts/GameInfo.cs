using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

	private static GameInfo _instance = null;

	private Enum_quitGameMode enum_quitGameMode = Enum_quitGameMode.Non;
	private int i_maxNumPlayers = -1;
	private Enum_playMode enum_playMode = Enum_playMode.Non;
	private string[] sArray_allPlayersNames = null;    // its length is i_maxNumOfPlayer+1 because 0 is the servr
	private int[] iArray_allPlayersId = null;  //just used by server, its length is i_maxNumOfPlayer+1 because 0 is the servr, and the real id
											//in network, the index of the array is i_idPlayer, attention with these two types of value
	private int[] iArray_allPlayersSides;

	public static GameInfo Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = (GameInfo)GameObject.FindObjectOfType(typeof(GameInfo));
				if (_instance == null)
				{
					_instance = GameObject.Instantiate(GlobalVariables.GO_GAMEINFO) as GameInfo;
				}
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}

	public void initGameinfo(int _i_maxNumPlayers)
	{
		i_maxNumPlayers = _i_maxNumPlayers;

		if(iArray_allPlayersId == null)
		{
			iArray_allPlayersId = new int[i_maxNumPlayers];
			sArray_allPlayersNames = new string[i_maxNumPlayers];
			iArray_allPlayersSides = new int[i_maxNumPlayers];
		}

		for(int i = 0; i < i_maxNumPlayers; ++i)
		{
			iArray_allPlayersId[i] = -1;
			sArray_allPlayersNames[i] = null;
			iArray_allPlayersSides[i] = -1;
		}
	}

	public Enum_quitGameMode Enum_quitGameMode
	{
		get{return enum_quitGameMode;}
		set{enum_quitGameMode = value;}
	}

	public int I_maxNumPlayers
	{
		get { return i_maxNumPlayers;}
		set {i_maxNumPlayers = value;}
	}

	public Enum_playMode Enum_playMode
	{
		get{return enum_playMode;}
		set{enum_playMode = value;}
	}

	public int[] getAllPlayersId()
	{
		return iArray_allPlayersId;
	}

	public string[] getAllPlayersName()
	{
		return sArray_allPlayersNames;
	}

	public int[] getAllPlayersSides()
	{
		return iArray_allPlayersSides;
	}

	public int getPlayerIdInNetworkByIndex(int _i_idPlayerInGame)
	{
		return iArray_allPlayersId [_i_idPlayerInGame];
	}

	public string getPlayerNameByIndex(int _i_idPlayerInGame)
	{
		return sArray_allPlayersNames[_i_idPlayerInGame];
	}

	public int getPlayerSideByIndex(int _i_idPlayerInGame)
	{
		return iArray_allPlayersSides[_i_idPlayerInGame];
	}

	public void setWholeArrayPlayersId(int[] _iArray_playersId)
	{
		for(int i = 0; i < i_maxNumPlayers; ++i)
		{
			iArray_allPlayersId[i] = _iArray_playersId[i];
		}
	}

	public void setWholeArrayPlayersName(string[] _sArray_playersNames)
	{
		for(int i = 0; i < i_maxNumPlayers; ++i)
		{
			sArray_allPlayersNames[i] = _sArray_playersNames[i];
		}
	}

	public void setWholeArrayPlayersSide(int[] _iArray_playersSides)
	{
		for(int i = 0; i < i_maxNumPlayers; ++i)
		{
			iArray_allPlayersSides[i] = _iArray_playersSides[i];
		}
	}

	public void setPlayerIdInNetworkByIndex(int _i_idPlayerInGame, int _i_newIdPlayerInNetwork)
	{
		iArray_allPlayersId [_i_idPlayerInGame] = _i_newIdPlayerInNetwork;
	}

	public void setPlayerNameByIndex(int _i_idPlayerInGame, string _s_namePlayer)
	{
		sArray_allPlayersNames [_i_idPlayerInGame] = _s_namePlayer;
	}

	public void setPlayerSideByIndex(int _i_idPlayerInGame, int _i_sidePlayer)
	{
		iArray_allPlayersSides [_i_idPlayerInGame] = _i_sidePlayer;
	}
}















