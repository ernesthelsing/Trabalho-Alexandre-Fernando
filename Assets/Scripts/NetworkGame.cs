using UnityEngine;
using System.Collections.Generic;

public class NetworkGame : MonoBehaviour {
	
	public string serverName = "MyGameServer";
	public string gameTypeOnMasterServer = "MyGameType";
	
	public int networkMaxPlayers = 4;
	public int networkConnectPort = 25001;
	
	public bool bnIAmTheMaster = false;
	
	public string playerName = null;
	
	// In game chat stuff
	public class ChatEntry {
		
		public string time;
		public string name;
		public string text;
	};
	
	public class PlayerNode {
		
		public string playerName;
		public NetworkPlayer networkPlayer;
	};
	
	private List<ChatEntry> chatEntries = new List<ChatEntry>();
	private List<PlayerNode> playerList = new List<PlayerNode>();
	
	private string playerName;
	
	void Awake() {
		
		chatEntries = new List<ChatEntry>();
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	 * Client Stuff
	 */
	private void OnConnectedToServer() {
		
		// Example3: ShowChatWindow();
		networkView.RPC("TellServerOurName", RPCMode.Server, playerName);
	}
	
	/*
	 * Server Stuff
	 */
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
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public string GetServerName() {
		
		return serverName;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="npPlayer">
	/// A <see cref="NetworkPlayer"/>
	/// </param>
	private void OnPlayerConnected(NetworkPlayer npPlayer) {
		
		ApplyGlobalChatText(AddTimeStamp(), npPlayer.ipAddress, "Jogador conectado.");
		// TODO: missing code here...
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="npPlayer">
	/// A <see cref="NetworkPlayer"/>
	/// </param>
	private void OnPlayerDisconnected(NetworkPlayer npPlayer) {
		
		// TODO: add code here
	}
	
	
	/*
	 * CHAT STUFF
	 */
	public List<ChatEntry> GetChatEntries() {
		
		return chatEntries;
	} 
	
	/// <summary>
	/// Gets the current time and converts it to a string, so it can be added to the message
	/// </summary>
	public string AddTimeStamp ()
	{
		
		return System.DateTime.Now.ToString ("HH:mm:ss");
	}
	
	public void SendChatMessage(string stMsg) {
		
		if(bnIAmTheMaster) {
			ApplyGlobalChatText(AddTimeStamp(), "Server", stMsg);
		}
		else {
			
			networkView.RPC("ApplyGlobalChatText", RPCMode.Server, AddTimeStamp(), "FIXME Player", stMsg);
		}
	}
	
	[RPC]
	private void ApplyGlobalChatText(string stTimeStamp, string stName, string stMsg) {
		
		ChatEntry entry = new ChatEntry();
		entry.time = stTimeStamp;
		entry.name = stName;
		entry.text = stMsg;
		
		chatEntries.Add(entry);
		
		// Remove old entries
		if(chatEntries.Count > 20) {
			
			chatEntries.RemoveAt(0);
		}
		
	}

	[RPC]
	private void TellServerOurName(string stName, NetworkMessageInfo info) {
		
		PlayerNode newEntry = new PlayerNode();
		newEntry.playerName = stName;
		newEntry.networkPlayer = info.sender;
		playerList.Add(newEntry);
		
		
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void addGameChatMessage(string stMsg) {
		
		// FIXME: review the number of parameters in ApplyGlocablChatText
		ApplyGlobalChatText(AddTimeStamp(), "Pq eu fiz isso?" ,stMsg);
		
		if(Network.connections.Length > 0) {
			networkView.RPC("ApplyGlobalChatText", RPCMode.Others, "", stMsg);
		}
	}
}
