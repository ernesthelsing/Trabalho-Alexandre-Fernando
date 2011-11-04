using UnityEngine;
using System.Collections.Generic;

public class LobbyChat : MonoBehaviour
{

	public bool usingChat = false;
	public bool showChat = false;

	private string inputField = "";

	private Vector2 scrollPosition = Vector2.zero;
	private int width = 400;
	private int height = 180;
	private string playerName = null;
	private float lastUnfocus = 0;
	private Rect window;

	// Playerlist on the server
	class LobbyPlayerNode {

		string playerName;
		NetworkPlayer networkPlayer;
	}
	private List<LobbyPlayerNode> playerList = new List<LobbyPlayerNode>();

	// Chat stuff
	class LobbyChatEntry {
		string name;
		string text;
	}
	private List<LobbyChatEntry> chatEntries = new List<LobbyChatEntry>();


	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {

		window = Rect(Screen.width * 0.5f - width*0.5f, Screen.height - height+5, width, height);
	}

	void OnGUI() {

		if(!showChat)
			return;

		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length <= 0) {

			if(lastUnfocus + 0.25 < Time.time) {

				usingChat = true;
				GUI.FocusWindow(5);
				GUI.FocusControl("Chat input field");
			}
		}

		window = GUI.window(5, window, GlobalChatWindow, "");
	}


	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CLIENT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void OnConnectedToServer() {
		
		ShowChatWindow();
		networkView.RPC("TellServerOurName", RPCMode.Server, playerName);
	}

	void OnDisconnectedFromServer() {

		CloseChatWindow();
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SERVER STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void OnServerInitialized() {

		ShowChatWindow();

		LobbyPlayerNode newEntry = new LobbyPlayerNode();
		newEntry.playerName = playerName;
		newEntry.networkPlayer = Network.player;
		playerList.Add(newEntry);

		addGameChatMessage(playerName + " joined the game.");
	}

	void OnPlayerDisconnected(NetworkPlayer player) {

		addGameChatMessage("Player disconnected from "+ player.ipAddress);

		// Remove player from the list
		foreach(LobbyPlayerNode entry in playerList) {

			if(entry.networkPlayer == player) {
				playerList.Remove(entry);
				break;
			}
		}
	}

	void OnPlayerConnected(NetworkPlayer player) {

		addGameChatMessage("Player connected from "+ player.ipAddress);
	}

	[RPC]
	void TellServerOurName(string stName, NetworkMessageInfo info) {
		
		LobbyPlayerNode newEntry = new LobbyPlayerNode();
		newEntry.playerName = stName;
		newEntry.networkPlayer = info.sender;
		playerList.Add(newEntry);

		addGameChatMessage(stName + " joined the game");
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CHAT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void CloseChatWindow() {

		showChat = false;
		inputField = "";
		chatEntries = new List<LobbyChatEntry>();
	}

	void ShowChatWindow() {

		// FIXME: this code generated problems before...
		playerName = PlayerPrefs.GetString("playerName", "");

		if(!playerName || playerName == "") {

			playerName = "RandomName" + Random.Range(1,999);
		}

		showChat = true;
		inputField = "";
		chatEntries = new List<LobbyChatEntry>();
	}

	void GlobalChatWindow(int nId) {

		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		{

			foreach(LobbyChatEntry entry in chatEntries) {

				GUILayout.BeginHorizontal();

				if(entry.name == "") {

					GUILayout.Label(entry.text);
				}
				else {

					// TODO: add different colors for different names
					GUILayout.Label(entry.name + ": " + entry.text);
				}

				GUILayout.EndHorizontal();
				GUILayout.Space(1);
			}
		}
		GUILayout.EndScrollView():

		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0) {
			
			HitEnter (inputField);
		}
		GUI.SetNextControlName ("Chat input field");
		inputField = GUILayout.TextField (inputField);

		// New!
		//
		if(Input.GetKeyDown("mouse 0")) {

			if(usingChat) {

				usingChat = false;
				GUI.UnfocusWindow(); // deselect chat window
				lastUnfocus = Time.time;
			}
		}
	}
		
	void HitEnter (string stMsg)
	{
		
		stMsg = stMsg.Replace ("\n", "");
		networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, stMsg);
		inputField = ""; // Clear input line
		GUI.UnfocusWindow(); // Deselect chat window
		lastUnfocus = Time.time;
		usingChat = false;
	}

	/// <summary>
	/// 
	/// </summary>
	private void addGameChatMessage(string stMsg) {
		
		ApplyGlobalChatText("" ,stMsg);
		
		if(Network.connections.Length > 0) {
			networkView.RPC("ApplyGlobalChatText", RPCMode.Others, "", stMsg);
		}
	}

	[RPC]
	void ApplyGlobalChatText(string stName, string stMsg) {
		
		LobbyChatEntry entry = new LobbyChatEntry();
		entry.name = stName;
		entry.text = stMsg;
		
		chatEntries.Add(entry);
		
		// Remove old entries
		if(chatEntries.Count > 4) {
			
			chatEntries.RemoveAt(0);
		}

		scrollPosition.y = 1000000;
	}
	

}
