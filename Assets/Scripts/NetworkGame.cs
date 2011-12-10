using UnityEngine;
using System.Collections.Generic;

public class NetworkGame : MonoBehaviour {
	
	// Points to itself
	public static NetworkGame Script;

	public string serverName = "MyGameServer";
	public string gameTypeOnMasterServer = "MyGameType";
	
	public int networkMaxPlayers = 3;
	public string connectToIP = "127.0.0.1";
	public int networkConnectPort = 25001;

	// 'Gambitech' to use or not the master server	
	private bool useMasterServer = false;
	public bool UseMasterServer {
		get { return useMasterServer; }
		set { useMasterServer = value; }
	}

	public int mySlotInPlayerList = 0;
	
	public class PlayerInfo {
		
		public string playerName;
		public NetworkPlayer networkPlayer;
	};
	
	public static List<PlayerInfo> playerList = new List<PlayerInfo>();

	private bool lauchingGame = false;
	private int lastLevelIdx = 0;

	public GameObject serverPlayerAvatar;
	public GameObject clientPlayerAvatar;

	public GameObject serverSpawnPoint;

	private Vector3 v3SpawnPosition = Vector3.zero;

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
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		//if(lauchingGame)
			//LauchingGameGUI();

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
		
		Network.InitializeServer(networkMaxPlayers, networkConnectPort, false);

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

		//
		// TODO: center on the screen and actually fix everything
		//
		//
		// FIXME: the error on in game chat has to be here

		/*
		// Show a loading screen or something...
		GUI.Box(new Rect(Screen.width*0.5f-140, Screen.height*0.5f-25, 280, 50), "");

		if(Application.CanStreamedLevelBeLoaded((Application.loadedLevel+1))){
		
			GUI.Label(new Rect(Screen.width*0.5f + 200, Screen.height * 0.5f - 25, 285, 150), "Loaded, starting the game!");
			Application.LoadLevel((Application.loadedLevel+1));
		}
		else{
		
			GUI.Label(new Rect(Screen.width/4+200,Screen.height/2-25,285,150), 
					"Starting..Loading the game: " + Mathf.Floor(Application.GetStreamProgressForLevel((Application.loadedLevel+1))*100) + " %" );
		}	
		*/
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

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * GAMEPLAY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	
	public void SpawnPlayers() {

		if(Network.isServer) {

			Network.Instantiate(serverPlayerAvatar, v3SpawnPosition, transform.rotation, 0);

		}
		else {

			Network.Instantiate(clientPlayerAvatar, v3SpawnPosition, transform.rotation, 0);
		}
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
