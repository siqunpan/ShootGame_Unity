using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[Serializable]
public enum Enum_playMode
{
	Non,
	Solo,
	MultiPlayer
}

[Serializable]
public enum Enum_menuType
{
	Non,
	MainMenu,
	SoloMenu,
	MultiPlayerMenu,
	RoomMenu,
	InstructionMenu,
	QuitMenu,
	InGameMenu
}

[Serializable]
public enum Enum_playRole
{
	Non,
	Server,
	Client
}

public enum Enum_quitGameMode
{
	Non,
	FinishGame,
	QuitDuringGame
}

public class GUIManager : MonoBehaviour {

	public Rect r_menuArea;
	public Rect r_soloModeButton;
	public Rect r_multiModeButton;
	public Rect r_instructionsButton;
	public Rect r_quitButton;
	private Rect r_menuAreaNomalized;
	
	private int i_serverClientRole = 0;  //0:server ; 1:client 
	private int i_playerSideSelected = 0;
	private string[] sArray_ToolBarPlayRoles = {"Server", "Client"};
	private string[] sArray_SelectionString = {"Side 0", "Side 1"};
	private const string s_typeName = "MyUniqueShootingGame";
	private string s_gameName = "DefaultRoomName";
	private string s_playerName = "DefaultPlayerName";
	private bool b_onlyLocalServerAvailable = false;
	private GameInfo gameInfo;

//	private int i_numOfPlayers = 1;
	private int i_maxNumOfClients = 4;   // maximum 4 palyers
	private string[] sArray_allPlayersNames;    // its length is i_maxNumOfPlayer+1 because 0 is the servr
	private int[] iArray_allPlayersId;  //just used by server, its length is i_maxNumOfPlayer+1 because 0 is the servr, and the real id
											//in network, the index of the array is i_idPlayer, attention with these two types of value
	private int[] iArray_allPlayersSides;

	private int i_idMyPlayerInGame;
	private int i_idMyPlayerInNetwork;

	private const int NEW_PLAYER_JOINED = 1;
	private const int EXIST_PLAYER_EXITED = 2;

	#region test internet
	private System.Int32 dwFlag = new int();
	private const int INTERNET_CONNECTION_MODEM = 1; 
	private const int INTERNET_CONNECTION_LAN = 2;
	[DllImport("wininet.dll")]
	private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
	#endregion test internet

	#region client
	private HostData[] hostList;
	#endregion client

	private Enum_playMode enum_playMode = Enum_playMode.Non;
	private Enum_menuType enum_menuType = Enum_menuType.Non; 

	public Enum_playMode getCurrentPlayMode()
	{
		return enum_playMode;
	}

	public Enum_menuType getCurrentMenuType()
	{
		return enum_menuType;
	}

	public int getMaxNumClients()
	{
		return i_maxNumOfClients;
	}

	public string[] getAllPlayersNames()
	{
		return sArray_allPlayersNames;
	}

	public int[] getAllPlayersId()
	{
		return iArray_allPlayersId;
	}

	public int[] getAllPlayersSides()
	{
		return iArray_allPlayersSides;
	}

	private void StartServer ()
	{
		if (!InternetGetConnectedState(ref dwFlag, 0))  //no network
		{
			MasterServer.ipAddress = "127.0.0.1";   //in order to test locally
			Network.InitializeServer (i_maxNumOfClients, 25000, false);
		}
		else
		{
			Network.InitializeServer (i_maxNumOfClients, 25000, !Network.HavePublicAddress());
			MasterServer.RegisterHost (s_typeName, s_gameName);
		}

		sArray_allPlayersNames[0] = s_playerName;
		iArray_allPlayersId [0] = 0;
	}

	private void StopServer()
	{
		MasterServer.UnregisterHost ();
		Network.Disconnect ();
	}

	private void StopClient()
	{
		Network.Disconnect ();
	}

	void OnServerInitialized()
	{
		Debug.Log ("Server Initialized");
	}

	private void RefreshHostList()
	{
		MasterServer.RequestHostList (s_typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
		}
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect (hostData);
	}

	void OnConnectedToServer()
	{	
		enum_menuType = Enum_menuType.RoomMenu;

		//We call the rpc here because here already connects to the network
		networkView.RPC ("server_updateRoomMenuRPC", RPCMode.Server, Network.player, s_playerName, NEW_PLAYER_JOINED);

		Debug.Log ("Client Joined, id : " + int.Parse(Network.player.ToString()));
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (Network.isServer)
		{
			for(int i = 0; i < i_maxNumOfClients+1; ++i)
			{
				iArray_allPlayersId[i] = -1;
				sArray_allPlayersNames[i] = null;
				iArray_allPlayersSides[i] = -1;
			}

			i_playerSideSelected = 0;
			
			b_onlyLocalServerAvailable = false;
//			i_numOfPlayers = 1;

			Debug.Log("Local server connection disconnected");
		}
		else
		{
			if (info == NetworkDisconnection.LostConnection)
			{
				Debug.Log("Lost connection to the server");
			}
			else
			{
				Debug.Log("Successfully diconnected from the server");
			}

			// initiate these variables
			for(int i = 0; i < i_maxNumOfClients+1; ++i)
			{
				sArray_allPlayersNames[i] = null;
				iArray_allPlayersId[i] = -1;
				iArray_allPlayersSides[i] = -1;
			}
			b_onlyLocalServerAvailable = false;

			i_playerSideSelected = 0;

			if(enum_menuType != Enum_menuType.InGameMenu)
			{
				enum_menuType = Enum_menuType.MultiPlayerMenu;
			}

			if(!InternetGetConnectedState(ref dwFlag, 0)) //no network
			{
				MasterServer.ClearHostList();
			}
			else
			{
				RefreshHostList();
			}
		}
	}

	void OnFailedToConnect(NetworkConnectionError error) 
	{
		enum_menuType = Enum_menuType.MultiPlayerMenu;
		RefreshHostList ();
		Debug.Log("Could not connect to server: " + error);
	}
	
	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
//		DontDestroyOnLoad (this.gameObject);

//		gameInfo = FindObjectOfType (typeof(GameInfo)) as GameInfo;
		gameInfo = GameInfo.Instance;

		r_menuAreaNomalized = new Rect (r_menuArea.x * Screen.width - r_menuArea.width * 0.5f,
		                                r_menuArea.y * Screen.height - r_menuArea.height *0.5f,
		                                r_menuArea.width, r_menuArea.height);

		enum_menuType = Enum_menuType.MainMenu;
		enum_playMode = Enum_playMode.Non;
		 
		if (!InternetGetConnectedState(ref dwFlag, 0))
		{
			Debug.Log("no network!");
		}
		else if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
		{
			Debug.Log("network by modem!");
		}
		else if((dwFlag & INTERNET_CONNECTION_LAN)!=0)   
		{
			Debug.Log("network by card!");  
		}

		MasterServer.ClearHostList ();

		//No server and client here, so initiate the two arrays for all the players
		iArray_allPlayersId = new int[i_maxNumOfClients+1];
		sArray_allPlayersNames = new string[i_maxNumOfClients+1];
		iArray_allPlayersSides = new int[i_maxNumOfClients+1];
		for(int i = 0; i < i_maxNumOfClients + 1; ++i)   //I use i_maxNumOfClients + 1 because 0 is for server
		{
			sArray_allPlayersNames[i] = null;
			iArray_allPlayersId[i] = -1;
			iArray_allPlayersSides[i] = -1;
		}

		i_playerSideSelected = 0;

		if(gameInfo.Enum_quitGameMode != Enum_quitGameMode.Non)
		{
			loadGameInfo();
		}

	}

	public void loadGameInfo()
	{
		if(gameInfo.Enum_quitGameMode == Enum_quitGameMode.FinishGame)
		{
			if(gameInfo.Enum_playMode == Enum_playMode.Solo)
			{
				// no need to load
			}
			else
			{
				for(int i = 0; i < i_maxNumOfClients+1; i++)
				{
					sArray_allPlayersNames[i] = gameInfo.getPlayerNameByIndex(i);
					iArray_allPlayersId[i] = gameInfo.getPlayerIdInNetworkByIndex(i);
				}

				enum_menuType = Enum_menuType.RoomMenu;
			}
		}
		else if(gameInfo.Enum_quitGameMode == Enum_quitGameMode.QuitDuringGame)
		{
			if(gameInfo.Enum_playMode == Enum_playMode.Solo)
			{
				// no need to load
			}
			else
			{
				if(Network.isServer)
				{
					MasterServer.UnregisterHost();
				}

				Network.Disconnect();

				enum_menuType = Enum_menuType.MultiPlayerMenu;
			
			
			}
		}

		gameInfo.Enum_quitGameMode = Enum_quitGameMode.Non;
	}

	public void saveGameInfo()
	{
		if(gameInfo != null)
		{
			if(enum_playMode != Enum_playMode.Solo)
			{
				gameInfo.I_maxNumPlayers = i_maxNumOfClients + 1;
				gameInfo.Enum_playMode = enum_playMode;
				gameInfo.setWholeArrayPlayersName(sArray_allPlayersNames);
				gameInfo.setWholeArrayPlayersId(iArray_allPlayersId);
				gameInfo.Enum_quitGameMode = Enum_quitGameMode.Non;
			}
			else
			{
				gameInfo.I_maxNumPlayers = i_maxNumOfClients + 1;
				gameInfo.Enum_playMode = Enum_playMode.Solo;
				sArray_allPlayersNames[0] = s_playerName;
				iArray_allPlayersId[0] = 0;
				gameInfo.setWholeArrayPlayersName(sArray_allPlayersNames);
				gameInfo.setWholeArrayPlayersId(iArray_allPlayersId);
				gameInfo.Enum_quitGameMode = Enum_quitGameMode.Non;
			}
		}
	}

	void OnGUI()
	{
		if(enum_menuType == Enum_menuType.MainMenu)
		{
			showMainMenu();
		}
		else if(enum_menuType == Enum_menuType.MultiPlayerMenu)
		{
			showMultiplayerMenu();
		}
		else if(enum_menuType == Enum_menuType.RoomMenu)
		{
			showRoomMenu();
		}
		else if(enum_menuType == Enum_menuType.InstructionMenu)
		{
			showInstructionMenu();
		}
//		else if(enum_menuType == Enum_menuType.InGameMenu)
//		{
//
//		}
	}

	private void showMainMenu()
	{
		GUI.BeginGroup (r_menuAreaNomalized);
		
		if(GUI.Button(r_soloModeButton, "Solo"))
		{
			enum_menuType = Enum_menuType.SoloMenu;
			enum_playMode = Enum_playMode.Solo;

			gameInfo.initGameinfo(i_maxNumOfClients+1);
			saveGameInfo();  //must call gameInfo.initGameinfo(); first

			Application.LoadLevel("Solo_Multiplayer");
		}
		else if(GUI.Button(r_multiModeButton, "Multiplayer"))
		{
			enum_menuType = Enum_menuType.MultiPlayerMenu;
			enum_playMode = Enum_playMode.MultiPlayer;
		}
		else if(GUI.Button(r_instructionsButton, "Instruction"))
		{
			enum_menuType = Enum_menuType.InstructionMenu;
			enum_playMode = Enum_playMode.Non;
		}
		else if(GUI.Button(r_quitButton, "Quit"))
		{
			enum_menuType = Enum_menuType.QuitMenu;
			enum_playMode = Enum_playMode.Non;
			Application.Quit();
		}
		GUI.EndGroup();
	}

	private void showMultiplayerMenu()
	{
		i_serverClientRole = GUI.Toolbar (new Rect (25, 25, 250, 30), i_serverClientRole, sArray_ToolBarPlayRoles);

		GUI.Label(new Rect (25, 80, 150, 30), "Name of you: ");
		s_playerName = GUI.TextField(new Rect(175, 80, 250, 30), s_playerName);
		s_playerName = s_playerName.Replace(" ", string.Empty);    //space is not allowed in the name

		if(i_serverClientRole == 0)  //is server
		{
			GUI.Label(new Rect (25, 135, 150, 30), "Name of the room: ");
			s_gameName = GUI.TextField(new Rect(175, 135, 250, 30), s_gameName);

			if(GUI.Button(new Rect(25, 190, 150, 30), "Start Server"))
			{
				StartServer();
				enum_menuType = Enum_menuType.RoomMenu;
			}
		}
		else if(i_serverClientRole == 1)   // is client
		{
			GUI.Box(new Rect(25, 190, 700, 220), "Server Hosts List");

			if(GUI.Button(new Rect(25, 135, 150, 30), "RefreshServerHosts"))
			{
				if(!InternetGetConnectedState(ref dwFlag, 0)) //no network
//				if(Network.player.externalIP != "UNASSIGNED_SYSTEM_ADDRESS")
				{
					Debug.Log("no network available, we will search local server");
					MasterServer.ClearHostList();
					b_onlyLocalServerAvailable = true;
				}
				else
				{
					RefreshHostList();
					b_onlyLocalServerAvailable = false;
				}
			}

			if(b_onlyLocalServerAvailable)
			{
				if(GUI.Button(new Rect(30, 215, 300, 30), s_gameName + " (LocalServer)"))
				{
					Network.Connect("127.0.0.1", 25000);
					enum_menuType = Enum_menuType.RoomMenu;
//					Debug.Log("connect to local server");
				}
			}

			if(hostList != null)
			{
				for(int i = 0; i < hostList.Length; ++i)
				{
					if(GUI.Button(new Rect(30, 215+i*55, 150, 30), hostList[i].gameName))
					{
						JoinServer(hostList[i]);
//						enum_menuType = Enum_menuType.RoomMenu;
					}
				}
			}
		}

		if(GUI.Button(new Rect(500, 25, 150, 30), "Back"))
		{
			enum_menuType = Enum_menuType.MainMenu;
		}
	}

	private void showRoomMenu()
	{
		int i_idPlayerInGame = -1;

		for(int i = 0; i < i_maxNumOfClients+1; ++i)
		{
			if(sArray_allPlayersNames[i] != null)
			{
				GUI.Label (new Rect (25, 25+55*i, 250, 30), 
				           sArray_allPlayersNames[i] == "DefaultPlayerName" ? 
				           sArray_allPlayersNames[i] +i.ToString() : sArray_allPlayersNames[i]
				           , "box");
			}

			if(iArray_allPlayersId[i] != int.Parse(Network.player.ToString()))
			{
				if(sArray_allPlayersNames[i] != null)
				{
					GUI.Label (new Rect (300, 25+55*i, 100, 30), "Side "+iArray_allPlayersSides[i], "box");
				}
			}
			else
			{
				i_idPlayerInGame = i;
			}
		}

		i_playerSideSelected = GUI.SelectionGrid (new Rect(300, 25+55*i_idPlayerInGame, 150, 30), i_playerSideSelected, sArray_SelectionString, 2);
		if(i_idPlayerInGame != -1)
		{
			if(Network.isServer)
			{
	//			iArray_allPlayersSides[i_idPlayerInGame] = i_playerSideSelected;
				networkView.RPC("updatePlayerSideRPC", RPCMode.All, i_idPlayerInGame, i_playerSideSelected);
			}
			if(Network.isClient)
			{
				if(iArray_allPlayersSides[0] == -1) //firstly initiate the array
				{
					networkView.RPC("server_updatePlayerSideRPC", RPCMode.Server, Network.player, i_playerSideSelected);
				}
				else
				{
					if(i_playerSideSelected != iArray_allPlayersSides[i_idPlayerInGame])
					networkView.RPC("updatePlayerSideRPC", RPCMode.All, i_idPlayerInGame, i_playerSideSelected);
				}
			}
		}

		if(Network.isServer)
		{
			if(GUI.Button(new Rect(550, 25, 150, 30), "StartGame"))
			{
				networkView.RPC("startGameRPC", RPCMode.All);
			}
		}

		if(GUI.Button(new Rect(550, 150, 150, 30), "Back"))
		{
			if(Network.isServer)
			{
				StopServer();
			}
			else if(Network.isClient)
			{
				StopClient();
			}

			enum_menuType = Enum_menuType.MultiPlayerMenu;
		}
	}

	public void showInstructionMenu()
	{
		if(GUI.Button(new Rect(350, 150, 150, 30), "Back"))
		{
			enum_menuType = Enum_menuType.MainMenu;
		}
	}

	[RPC]
	public void startGameRPC()
	{
		startGameLocal ();
	}

	public void startGameLocal()
	{
		gameInfo.initGameinfo(i_maxNumOfClients+1);
		saveGameInfo();  //must call gameInfo.initGameinfo(); first

		enum_menuType = Enum_menuType.InGameMenu;
//		GetComponent<NetworkView> ().enabled = false;
		Application.LoadLevel ("Solo_Multiplayer");
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
//		i_numOfPlayers ++;
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
//		i_numOfPlayers --;
		
		server_updateRoomMenuLocal (player, null, EXIST_PLAYER_EXITED);
	}

	void OnApplicationQuit()
	{
		if(Network.isServer)
		{
			StopServer();
		}

		/*
		 * We can't StopClient here because when quit the game, the network disconnect immediately, and the rpc funtion can't be sent to server.  
		 */

		Debug.Log("Exit the game");
	}
	
	[RPC]
	public void server_updateRoomMenuRPC(NetworkPlayer _updatePlayer, string _s_playerName, int _i_updateRoomMenuMode)
	{
		server_updateRoomMenuLocal (_updatePlayer, _s_playerName, _i_updateRoomMenuMode);
	}

	public void server_updateRoomMenuLocal(NetworkPlayer _updatePlayer, string _s_playerName, int _i_updateRoomMenuMode)
	{
		if(Network.isServer)
		{
			int i_idUpdatePlayerInNetwork = int.Parse(_updatePlayer.ToString());
			int i_idUpdatePlayer = -1;
			string s_allPlayersNamesBuffer = null;
			string s_allPlayersIdsBuffer = null;

			if(enum_menuType == Enum_menuType.InGameMenu)   //not allowed connected by the client when game already starts
			{
				Network.CloseConnection(_updatePlayer, true);
			}

			if(_i_updateRoomMenuMode == NEW_PLAYER_JOINED && enum_menuType != Enum_menuType.InGameMenu)
			{
				for(int i = 1; i < i_maxNumOfClients+1; ++i)  //0 is for server
				{
					if(iArray_allPlayersId[i] == -1)
					{
						iArray_allPlayersId[i] = i_idUpdatePlayerInNetwork;
						sArray_allPlayersNames[i] = _s_playerName;
						i_idUpdatePlayer = i;
						i = i_maxNumOfClients + 1; 
					}
					
					if(i == i_maxNumOfClients)
					{
						Debug.LogError("No place for the new client !!!!");
					}
				}
				
				for(int i = 0; i < i_maxNumOfClients+1; ++i)
				{
					s_allPlayersNamesBuffer += sArray_allPlayersNames[i];
					s_allPlayersNamesBuffer += " ";
				}

				for(int i = 0; i < i_maxNumOfClients+1; ++i)
				{
					s_allPlayersIdsBuffer += iArray_allPlayersId[i];
					s_allPlayersIdsBuffer += " ";
				}

				networkView.RPC("client_initRoomMenuRPC", _updatePlayer, s_allPlayersIdsBuffer, s_allPlayersNamesBuffer);
			}
			else if(_i_updateRoomMenuMode == EXIST_PLAYER_EXITED)
			{
				for(int i = 1; i < i_maxNumOfClients+1; ++i)  //0 is for server
				{
					if(iArray_allPlayersId[i] == i_idUpdatePlayerInNetwork)  // We can't test by the name, because two clients can have the same name
					{
						iArray_allPlayersId[i] = -1;
						sArray_allPlayersNames[i] = null;
						i_idUpdatePlayer = i;
						i = i_maxNumOfClients + 1; 
					}
					
					if(i == i_maxNumOfClients)
					{
						Debug.LogError("Can't find the exited client !!!!");
					}
				}
			}
			
			int i_idServerInNetwork = int.Parse(Network.player.ToString());

			if(_s_playerName == null) //When we want to remove the player exited, _s_playerName is non as called, so we have to assign it a name, here I use the serverName
			{
				_s_playerName = s_playerName;
			}
			
			networkView.RPC("updateRoomMenuRPC", RPCMode.All, i_idUpdatePlayer, i_idUpdatePlayerInNetwork, _s_playerName, _i_updateRoomMenuMode);
		}
	}

	[RPC]
	public void client_initRoomMenuRPC(string _s_allPlayersIdsBuffer, string _s_allPlayersNamesBuffer)
	{
		client_initRoomMenuLocal (_s_allPlayersIdsBuffer, _s_allPlayersNamesBuffer);
	}

	public void client_initRoomMenuLocal(string _s_allPlayersIdsBuffer, string _s_allPlayersNamesBuffer)
	{
		if(Network.isClient)
		{
			int i = 0;
			int i_countPlayer = 0;
			string s_namePlayer = null;
			string s_idPlayer = null;
			char c_space = ' ';
			
			while(i_countPlayer < i_maxNumOfClients+1)
			{
				if(_s_allPlayersNamesBuffer[i] != c_space)
				{
					s_namePlayer += _s_allPlayersNamesBuffer[i];
				}
				else
				{
					sArray_allPlayersNames[i_countPlayer] = s_namePlayer;
					s_namePlayer = null;
					i_countPlayer ++;
					
					if(i+1 < _s_allPlayersNamesBuffer.Length)
					{
						if(_s_allPlayersNamesBuffer[i+1] == c_space)
						{
							sArray_allPlayersNames[i_countPlayer] = null;
							i_countPlayer++;
							i++;
						}
					}
					else
					{
						i_countPlayer++;
					}
				}
				i++;
			}
			
			i = 0;
			i_countPlayer = 0;

			while(i_countPlayer < i_maxNumOfClients+1)
			{
				if(_s_allPlayersIdsBuffer[i] != c_space)
				{
					s_idPlayer += _s_allPlayersIdsBuffer[i];
				}
				else
				{
					iArray_allPlayersId[i_countPlayer] = int.Parse(s_idPlayer);
					s_idPlayer = null;
					i_countPlayer ++;
				}
				i++;
			}
		}
	}

	[RPC]
	public void updateRoomMenuRPC(int _i_idUpdatePlayer, int _i_idUpdatePlayerInNetwork, string _s_playerName, int _i_updateMenuMode)
	{
		updateRoomMenuLocal (_i_idUpdatePlayer, _i_idUpdatePlayerInNetwork, _s_playerName, _i_updateMenuMode);
	}

	public void updateRoomMenuLocal(int _i_idUpdatePlayer, int _i_idUpdatePlayerInNetwork, string _s_playerName, int _i_updateMenuMode)
	{
		if(_i_updateMenuMode == NEW_PLAYER_JOINED)
		{
			sArray_allPlayersNames[_i_idUpdatePlayer] = _s_playerName;
			iArray_allPlayersId[_i_idUpdatePlayer] = _i_idUpdatePlayerInNetwork;
		}
		else if(_i_updateMenuMode == EXIST_PLAYER_EXITED)
		{
			sArray_allPlayersNames[_i_idUpdatePlayer] = null;
			iArray_allPlayersId[_i_idUpdatePlayer] = -1;
		}
	}

	[RPC]
	public void server_updatePlayerSideRPC(NetworkPlayer _player, int _i_playerSide)
	{
		server_updatePlayerSideLocal (_player, _i_playerSide);
	}

	public void server_updatePlayerSideLocal(NetworkPlayer _player, int _i_playerSide)
	{
		if(Network.isServer)
		{
			int i_idUpdatePlayerInNetwork = int.Parse(_player.ToString());
			int i_idUpdatePlayer = -1;
			string s_allPlayersSidesBuffer = null;
			
			
			for(int i = 0; i < i_maxNumOfClients+1; ++i)  //0 is for server
			{
				if(iArray_allPlayersId[i] == i_idUpdatePlayerInNetwork)
				{
					iArray_allPlayersSides[i] = _i_playerSide;
					i_idUpdatePlayer = i;
					i = i_maxNumOfClients + 1; 
				}
			}
			
			for(int i = 0; i < i_maxNumOfClients+1; ++i)
			{
				s_allPlayersSidesBuffer += iArray_allPlayersSides[i];
				s_allPlayersSidesBuffer += " ";
			}
			
			networkView.RPC("client_initPlayersSidesRPC", _player, s_allPlayersSidesBuffer);
			networkView.RPC("updatePlayerSideRPC", RPCMode.Others, i_idUpdatePlayer, _i_playerSide);
		}
	}

	[RPC]
	public void client_initPlayersSidesRPC(string _s_allPlayersSidesBuffer)
	{
		client_initPlayersSidesLocal (_s_allPlayersSidesBuffer);
	}

	public void client_initPlayersSidesLocal(string _s_allPlayersSidesBuffer)
	{
		int i = 0;
		int i_countPlayer = 0;
		char c_space = ' ';
		string s_sidePlayer = null;

		while(i_countPlayer < i_maxNumOfClients+1)
		{
			if(_s_allPlayersSidesBuffer[i] != c_space)
			{
				s_sidePlayer += _s_allPlayersSidesBuffer[i];
			}
			else
			{
				iArray_allPlayersSides[i_countPlayer] = int.Parse(s_sidePlayer);
				s_sidePlayer = null;
				i_countPlayer ++;
			}
			i++;
		}
	}

	[RPC]
	public void updatePlayerSideRPC(int _i_idUpdatePlayer, int _i_playerSide)
	{
		updatePlayerSideLocal (_i_idUpdatePlayer, _i_playerSide);
	}

	public void updatePlayerSideLocal(int _i_idUpdatePlayer, int _i_playerSide)
	{
		iArray_allPlayersSides [_i_idUpdatePlayer] = _i_playerSide; 
	}
}




































