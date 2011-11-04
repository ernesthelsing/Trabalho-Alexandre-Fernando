using UnityEngine;
using System.Collections.Generic;

public class NetworkGame : MonoBehaviour {
	
	public string serverName = "MyGameServer";
	public string gameTypeOnMasterServer = "MyGameType";
	
	public int networkMaxPlayers = 4;
	public int networkConnectPort = 25001;
	
	public bool bnIAmTheMaster = false;
	
	public class PlayerInfo {
		
		public string playerName;
		public NetworkPlayer networkPlayer;
	};
	
	private List<PlayerInfo> playerList = new List<PlayerInfo>();
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {
		
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SERVER STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	public void StartMasterServer() {
		
		Network.InitializeServer(networkMaxPlayers, networkConnectPort, false);
		MasterServer.RegisterHost(gameTypeOnMasterServer, serverName, "My comment");
		
		bnIAmTheMaster = true; // tells everybody that I am the server
	}
	
	public HostData[] GetOnlineHostList() {

		// Update the list
		MasterServer.RequestHostList(gameTypeOnMasterServer);
		return MasterServer.PollHostList();
	}
	
	public string GetServerName() {
		
		return serverName;
	}

}
