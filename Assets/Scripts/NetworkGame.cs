using UnityEngine;
using System.Collections.Generic;

public class NetworkGame : MonoBehaviour {
	
	// Points to itself
	public static NetworkGame Script;

	public string serverName = "MyGameServer";
	public string gameTypeOnMasterServer = "MyGameType";
	
	public int networkMaxPlayers = 4;
	public int networkConnectPort = 25001;

	public int mySlotInPlayerList = 0;
	
	public class PlayerInfo {
		
		public string playerName;
		public NetworkPlayer networkPlayer;
	};
	
	public static List<PlayerInfo> playerList = new List<PlayerInfo>();

	private bool lauchingGame = false;
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {
		
		Script = this;
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		if(lauchingGame)
			LauchingGameGUI();

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
		MasterServer.RegisterHost(gameTypeOnMasterServer, serverName, "My comment");
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

		networkView.RPC("LaunchGame", RPCMode.All);
	}

	/*
	 * @brief		RPC to everybody that the game is lauching
	 * @param		void
	 * @return	void
	 */
	[RPC]
	public void LaunchGame() {

		Network.isMessageQueueRunning = false;
		lauchingGame = true;
	}

	/*
	 * @brief		If the game is laughing, show a window with the level loading progress
	 * @param		void
	 * @return	void
	 */
	public void LauchingGameGUI() {

		// TODO: center on the screen and actually fix everything

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
	}

}
