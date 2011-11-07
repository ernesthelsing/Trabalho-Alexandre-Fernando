using UnityEngine;
using System;
using System.Collections.Generic;

public class InGameChat : MonoBehaviour {

	/* According to the assignment, pressing 't' during the game should allow the players to chat.
	 * Press 't', type the message and hit enter to send.
	 * Pressing 'esc' should cancel the chat and allow the player to play again.
	 */
	private bool showChat = false;
	public float waitForFocusTime = 0.5f;
	private float waitFocusDelay;
	private Rect window;

	private string inputField = "";

	private List<LobbyChat.LobbyChatEntry> chatEntries = new List<LobbyChat.LobbyChatEntry>();
	private MainScreen mainScreenScript;
	private float lastUnfocus = 0;
	private string playerName;

	private int windowStartX = 30;
	private int windowStartY = 10;
	private int windowHeight = 100;

	private Vector2 scrollPosition = Vector2.zero;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	// Used when the game start
	void Awake() {

		mainScreenScript = MainScreen.Script;
	
		window = new Rect(windowStartX, windowStartY, Screen.width-(windowStartX*2), windowHeight);
	}

	// Use this for initialization
	void Start() {

		chatEntries = new List<LobbyChat.LobbyChatEntry>();
		waitFocusDelay = waitForFocusTime;
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	// GUI stuff
	void OnGUI() {

		Event e = Event.current;
		
		if(e.keyCode == KeyCode.T && !showChat) {

			ShowChatWindow();
		}

		if(!showChat)
			return;

		/* This is needed because when the input field is created it doesn't have focus on it.
		 * If we force the focus, the character 't' will show up in the input field. So, we wait
		 * a little and the force the focus.
		 */
		if(waitFocusDelay <= 0) {

			GUI.FocusWindow(7);
			GUI.FocusControl("Chat input field");

			waitFocusDelay = waitForFocusTime;
		}
		else {

			waitFocusDelay -= Time.deltaTime;
		}

		if(e.type == EventType.keyDown && e.character == '\n' && inputField.Length <= 0) {

			// Change back the focus to the chat window
			Debug.Log("Enter pressed: change focus back");

			if(lastUnfocus + 0.25 < Time.time) {

				GUI.FocusWindow(7);
				GUI.FocusControl("Chat input field");
			}
		}

		window = GUI.Window(7, window, InGameChatWindow, "");
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CLIENT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * CHAT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Clean the input field, shows the chat window on the screen
	 * @param
	 * @return	void
	 */
	void ShowChatWindow() {

		playerName = MainScreen.playerName;

		showChat = true;
		inputField = "";

		// DEBUG
		Debug.Log("Chat on");
	}

	/*
	 * @brief		Clean the chat entry, dismiss the chat window
	 * @param		
	 * @return 	void
	 */
	void CloseChatWindow() {

		showChat = false;
		inputField = "";
		waitFocusDelay = waitForFocusTime;
		//chatEntries = new List<LobbyChat.LobbyChatEntry>();

		// DEBUG
		Debug.Log("Chat off");
	}

	/*
	 * @brief		Watch for events during the use of the chat (<ESC> dismiss the window, <ENTER> sends the message 
	 * @param		nId		Unity internal window code
	 * @return	void
	 */
	void InGameChatWindow(int nId) {

		Event e = Event.current;

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		{
			foreach(LobbyChat.LobbyChatEntry entry in chatEntries) {

				GUILayout.BeginHorizontal();

				if(entry.name == "") {

					GUILayout.Label(entry.text);
				}
				else {

					// TODO: add different colors for different names
					GUILayout.Label(entry.name + ": " + entry.text);
				}

				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndScrollView();

		if(e.type == EventType.keyDown && e.keyCode == KeyCode.Escape) {

			// Pressing ESC at anytime should dismiss tRua Carlos Gomeshe chat window
			Debug.Log("ESC pressed");

			CloseChatWindow();
		}

		if (e.type == EventType.keyDown && e.character == '\n' && inputField.Length > 0) {
			
			HitEnter(inputField);
		}

		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);
	}

	/*
	 * @brief		Routine to the send the message typed in the chat window
	 * @param		stMsg		String (from the input field) to be sended over the network
	 * @return	void
	 */
	void HitEnter(string stMsg)	{
		
		stMsg = stMsg.Replace ("\n", "");
		
		// FIXME
		//networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, stMsg);
		ApplyGlobalChatText(playerName, stMsg);

		inputField = ""; // Clear input line
		GUI.UnfocusWindow(); // Deselect chat window
		lastUnfocus = Time.time;
	} 
	
	/*
	 * @brief		RPC Adds new chat message to the lobby
	 * @param		stName	Player who sent the message
	 * @param		stMsg		Message to send
	 * @return	void
	 */
	[RPC]
	void ApplyGlobalChatText(string stName, string stMsg) {
		
		LobbyChat.LobbyChatEntry entry = new LobbyChat.LobbyChatEntry();
		entry.name = stName;
		entry.text = stMsg;
		
		chatEntries.Add(entry);
		
		// Remove old entries
		if(chatEntries.Count > 40) {
			
			chatEntries.RemoveAt(0);
		}

		scrollPosition.y = 1000000;
	}

}
