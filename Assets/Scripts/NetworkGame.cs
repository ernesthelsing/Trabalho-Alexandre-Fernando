using UnityEngine;
using System.Collections.Generic;

public class NetworkGame : MonoBehaviour {
	
	// Points to itself
	public static NetworkGame Script;

	public string serverName = "DescartedServer";
	public string gameTypeOnMasterServer = "DescartedGame";
	
	public static int networkMaxPlayers = 4; 
	public string connectToIP = "127.0.0.1";
	public int networkConnectPort = 25001;

	// 'Gambitech' to use or not the master server	
	private bool useMasterServer = true;
	public bool UseMasterServer {
		get { return useMasterServer; }
		set { useMasterServer = value; }
	}

	public int mySlotInPlayerList = 0;
	
	public class PlayerInfo {
		
		public string playerName;							// Network name 
		public NetworkPlayer networkPlayer;		// Network data from Unity
		public int playerAvatarIdx;						// The index of the player's character choosen
	};
	
	public static List<PlayerInfo> playerList = new List<PlayerInfo>();

	private bool lauchingGame = false;
	private int lastLevelIdx = 0;

	// Check MainScreen: must match the colors selected there in playerAvatarColors and playerAvatarIcons
	public GameObject[] playersAvatars = new GameObject[MainScreen.playerUniqueAvatarsNumber];

	public GameObject serverSpawnPoint;

	private Vector3 v3SpawnPosition = Vector3.zero;
	
	public int levelTimeNetworkGame = 0;

	// Error window
	public bool showErrorWindow = false;
	public Rect errorWindowRect = new Rect();
	private string errorMessageString = ""; 
	public GUISkin skin;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {
		
		Script = this;

		//http://unity3d.com/support/documentation/Components/net-NetworkLevelLoad.html
		DontDestroyOnLoad(this);
		networkView.group = 1;

		/* HACK TO USE LOCAL MASTER SERVER */
		/*
		MasterServer.ipAddress = "127.0.0.1";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "127.0.0.1";
		Network.natFacilitatorPort = 50005; // your own port number
		*/

	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		GUI.skin = skin;

		if(!showErrorWindow)
			return;

		errorWindowRect.width = Screen.width - 20;;
		errorWindowRect.height = 60;
		errorWindowRect.x = 10;
		errorWindowRect.y = Screen.height - errorWindowRect.height -20;

		GUILayout.BeginArea(new Rect(errorWindowRect.x, errorWindowRect.y, 
					errorWindowRect.width, errorWindowRect.height));
		{
			errorWindowRect = GUI.Window(7, errorWindowRect, ErrorWindow, "Error!");
		}
		GUILayout.EndArea();
	}

	void ErrorWindow(int windowID) {

		GUILayout.BeginHorizontal();
		{

			GUILayout.Label(errorMessageString);

			if(GUILayout.Button("Ok")) {

				showErrorWindow = false;
			}
			if(GUILayout.Button("Exit game")) {

				showErrorWindow = false;
				Application.Quit();
			}
		}
		GUILayout.EndHorizontal();
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SERVER STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Start the master server, allowing others player in the game to find this game
	 * @param		void
	 * @return	void
	 */
	public void StartMasterServer() {
		
		Network.InitializeServer(networkMaxPlayers-1, networkConnectPort, false);

		if(useMasterServer) {
			MasterServer.RegisterHost(gameTypeOnMasterServer, serverName, "My comment");
		}
	}

	/*
	 * @brief		Fetches all online games on the master server
	 * @param		void
	 * @return	An array with the games
	 */
	public HostData[] GetOnlineHostList() {

		// Update the list
		MasterServer.RequestHostList(gameTypeOnMasterServer);
		return MasterServer.PollHostList();
	}

	/*
	 * @brief		Returns the server name
	 * @param		void
	 * @return	String with the server name	
	 */
	public string GetServerName() {
		
		return serverName;
	}

	/*
	 * @brief		Warns everybody that the server is lauching the game
	 * @param		void
	 * @return	void
	 */
	public void HostLaunchGame() {

		if(!Network.isServer)
			return;

		MasterServer.UnregisterHost();

		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);

		networkView.RPC("LaunchLevel", RPCMode.All, Application.loadedLevel+1);
	}
	
	/*
	 * @brief		Used in restarting the game
	 * @param		void
	 * @return	void
	 */
	public void HostLaunchGameTarget(int levelIdx) {

		if(!Network.isServer)
			return;

		MasterServer.UnregisterHost();

		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);

		networkView.RPC("LaunchLevel", RPCMode.All, levelIdx);
	}

	/*
	 * @brief		RPC to everybody that the game is lauching
	 * @param		void
	 * @return	void
	 */
	[RPC]
	public void LaunchLevel(int levelIdx) {

		// DEBUG
		Debug.Log("[LaunchLevel]: " + levelIdx);

		lastLevelIdx = levelIdx;

		Network.isMessageQueueRunning = false;

		Network.SetLevelPrefix(levelIdx);
		Application.LoadLevel(levelIdx);

		Network.isMessageQueueRunning = true;

		lauchingGame = true;
	}

	/*
	 * @brief		If the game is loading, show a window with the level loading progress
	 * @param		void
	 * @return	void
	 */
	public void LauchingGameGUI() {

	}

	/*
	 * @brief		Connects to a specified server
	 * @param
	 * @return	void
	 */
	public void ConnectWithServer(HostData hostToConnect) {

		if(hostToConnect != null) {

			Network.Connect(hostToConnect);
		}
		else {

			Network.Connect(connectToIP, networkConnectPort);	
		}
	}

	/*
	 * @brief		Removes a player from the player list
	 * @param		player	A NetworkPlayer to be removed
	 * @return	void
	 */
	public void RemovePlayerFromPlayerList(NetworkPlayer player) {

		// DEBUG
		Debug.Log("[NetworkGame]RemovePlayerFromPlayerList " + player);

		// Remove player from the list
		foreach(NetworkGame.PlayerInfo entry in NetworkGame.playerList) {

			if(entry.networkPlayer == player) {

				playerList.Remove(entry);
				break;
			}
		}
		
		foreach(ScoreCounter.PlayerScore entry in ScoreCounter.Script.playersScores) {

			if(entry.netPlayer == player) {

				ScoreCounter.Script.playersScores.Remove(entry);
				break;
			}
		}
		
	}

	/*
	 * @brief		Finds and returns the PlayerInfo entry from a networkPlayer
	 * @param		networkPlayer	A NetworkPlayer to search in the PlayerList
	 * @return	A PlayerInfo entry, if found, or null if not
	 */
	public PlayerInfo GetPlayerInfoFromNetwork(NetworkPlayer networkPlayer) {

		foreach(PlayerInfo entry in playerList) {

			if(entry.networkPlayer == networkPlayer) {

				return entry;
			}
		}

		return null;
	}

	/*
	 * @brief		What to do when a player disconnects from the game
	 * @param		player	The NetworkPlayer for the disconnected player
	 * @return	void
	 */
	void OnPlayerDisconnected(NetworkPlayer player) {

		// DEBUG
		Debug.Log("[NetworkGame] Player disconnected: " + player);

		if(Network.isServer) {
		
			// FIXME: when the game is not started, this generates a error!	
			InGameChat.Script.ClientPlayerDisconnected(player);
			
			// Clean-up the player stuff
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);

			// DEBUG
			Debug.Log("[NetworkGame] Disconnection cleaned up");
		}

	}

	/*
	 * @brief	When the server disconnects
	 * @param	dcinfo	Network disconnection info
	 * @return	void
	 */
	void OnDisconnectedFromServer(NetworkDisconnection dcinfo){
		
		// DEBUG
		Debug.Log("[NetworkGame] Disconnection from server: " + dcinfo);

		if(Network.isServer){
			
			// DEBUG
			Debug.Log("[NetworkGame] Server has been disconected");	

			errorMessageString = "Server disconnected";
		}
		else{
		
			// DEBUG	
			Debug.Log("[NetworkGame] Disconected from server");

			errorMessageString = "Disconnected from server";
			showErrorWindow = true;

			//Application.LoadLevel("main_screen"); // Go back to the main screen
		}
		
		// DEBUG
		Debug.Log("[NetworkGame] Destroying this object.");
		// This object NetworkCode must always be destroyed when a new level is loaded
	//	Destroy(gameObject);
	}

	/*
	 * @brief		Called when we cannot connect to the MasterServer for some reason
	 * @param		NetworkConnectionError info	Info on the error
	 * @return	void
	 */
	void OnFailedToConnectToMasterServer(NetworkConnectionError errorInfo) {

		// DEBUG
		Debug.LogError("[NetworkGame] Could not connect to the master server: " + errorInfo);

		errorMessageString = "Could not connect to the master server";
		// Turn on error message
		showErrorWindow = true;
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * GAMEPLAY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	
	public void SpawnPlayers() {

		int playerIdx = MainScreen.Script.GetPlayerAvatarIndex();

		Vector3 playerSpawnPosition = v3SpawnPosition + new Vector3(0,0, 2 * playerIdx);

		Network.Instantiate(playersAvatars[playerIdx],	playerSpawnPosition, transform.rotation, 0);

	}

	public void InitializeSpawnPoint() {

		serverSpawnPoint = GameObject.FindWithTag("Respawn");

		if(serverSpawnPoint != null) {

			v3SpawnPosition = serverSpawnPoint.transform.position;
		}
		else {

			Debug.LogError("Respawn object not found.");
		}
	}

}
