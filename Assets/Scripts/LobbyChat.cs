using UnityEngine;
using System.Collections.Generic;

/* According to the assignment, pressing 't' during the game should allow the players to chat.
 * Press 't', type the message and hit enter to send.
 * Pressing 'esc' should cancel the chat and allow the player to play again.
 */

public class LobbyChat : MonoBehaviour
{

	public bool usingChat = false;
	public bool showChat = false;
	public bool inGame = false;

	private string inputField = "";

	private Vector2 scrollPosition = Vector2.zero;
	private int width = 300;
	private int height = 320;
	private string playerName = null;
	private float lastUnfocus = 0;
	private Rect window;

	private MainScreen mainScreenScript;

	// Chat stuff
	public class LobbyChatEntry {
		public string name;
		public string text;
	}
	private List<LobbyChatEntry> chatEntries = new List<LobbyChatEntry>();

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {

		mainScreenScript = MainScreen.Script;

		//window = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height - height+5, width, height);
		//window = mainScreenScript.GetAreaForChat();
		window = new Rect(30, 30, width, height);
		
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

		window = GUI.Window(5, window, GlobalChatWindow, "");
	}


	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CLIENT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		When someone connects to the server
	 * @param
	 * @return	void
	 */
	void OnConnectedToServer() {
		
		ShowChatWindow();
		networkView.RPC("TellServerOurName", RPCMode.Server, playerName);
	}

	/*
	 * @brief		When someone disconnects from the server
	 * @param
	 * @return	void
	 */
	void OnDisconnectedFromServer() {

		CloseChatWindow();
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SERVER STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		When the server is initialized. Show the chat window, create a new player (for the player hosting
	 * 					the game) and send a message to everyone.
	 * @param
	 * @return	void
	 */
	void OnServerInitialized() {

		ShowChatWindow();

		NetworkGame.PlayerInfo newEntry = new NetworkGame.PlayerInfo();
		newEntry.playerName = playerName;
		newEntry.networkPlayer = Network.player;
		NetworkGame.playerList.Add(newEntry);

		addGameChatMessage(playerName + " joined the game.");
	}

	/*
	 * @brief		Handles when some player disconnects from the server. Tell everyone about it, and remove the
	 * 					player from the player list
	 * @param		NetworkPlayer player	The player data structure on the network
	 * @return	void
	 */
	void OnPlayerDisconnected(NetworkPlayer player) {

		// Send a message to all
		addGameChatMessage("Player disconnected from " + player.ipAddress);

		// Removes the player from the list of connected players
		NetworkGame.Script.RemovePlayerFromPlayerList(player);
	}

	/*
	 * @brief		Function when the player connects on the server
	 * @param		NetworkPlayer player	The player data structure on the network
	 * @return	void
	 */
	void OnPlayerConnected(NetworkPlayer player) {

		addGameChatMessage("Player connected from "+ player.ipAddress);
	}

	/*
	 * @brief		RPC Announces the player to the server
	 * @param		stName	Player name
	 * @param		info		Unity's data structure
	 * @return	void
	 */
	[RPC]
	void TellServerOurName(string stName, NetworkMessageInfo info) {
		
		NetworkGame.PlayerInfo newEntry = new NetworkGame.PlayerInfo();
		newEntry.playerName = stName;
		newEntry.networkPlayer = info.sender;
		NetworkGame.playerList.Add(newEntry);

		addGameChatMessage(stName + " joined the game.");
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CHAT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Clean the chat entry, dismiss the chat window
	 * @param		
	 * @return 	void
	 */
	void CloseChatWindow() {

		showChat = false;
		inputField = "";
		chatEntries = new List<LobbyChatEntry>();
	}

	/*
	 * @brief		Clean the input field, shows the chat window on the screen
	 * @param
	 * @return	void
	 */
	void ShowChatWindow() {

		playerName = MainScreen.playerName;

		if(playerName == null || playerName == "") {

			playerName = "RandomName" + Random.Range(1,999);
		}

		showChat = true;
		inputField = "";
		chatEntries = new List<LobbyChatEntry>();
	}

	/*
	 * @brief		Chat window 
	 * @param		nId		Unity internal window code
	 * @return	void
	 */
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
		GUILayout.EndScrollView();

		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0) {
			
			HitEnter(inputField);
		}

		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);

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
		
	/*
	 * @brief		Routine to the send the message typed in the chat window
	 * @param		stMsg		String (from the input field) to be sended over the network
	 * @return	void
	 */
	void HitEnter(string stMsg)
	{
		
		stMsg = stMsg.Replace ("\n", "");
		networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, stMsg);
		inputField = ""; // Clear input line
		GUI.UnfocusWindow(); // Deselect chat window
		lastUnfocus = Time.time;
		usingChat = false;
	}

	/*
	 * @brief		Add another message to the chat rooster
	 * @param		stMsg		Message to be sent
	 * @return	void
	 */
	private void addGameChatMessage(string stMsg) {
		
		ApplyGlobalChatText("" ,stMsg);
		
		if(Network.connections.Length > 0) {

			networkView.RPC("ApplyGlobalChatText", RPCMode.Others, "", stMsg);
		}
	}

	/*
	 * @brief		RPC Adds new chat message to the lobby
	 * @param		stName	Player who sent the message
	 * @param		stMsg		Message to send
	 * @return	void
	 */
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
