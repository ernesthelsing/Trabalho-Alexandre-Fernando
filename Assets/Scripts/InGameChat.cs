using UnityEngine;
using System;
using System.Collections.Generic;

public class InGameChat : MonoBehaviour {

	/* According to the assignment, pressing 't' during the game should allow the players to chat.
	 * Press 't', type the message and hit enter to send.
	 * Pressing 'esc' should cancel the chat and allow the player to play again.
	 */
	private bool showChat = false;
	private float waitFocusDelay = 0.25f;
	private Rect window;

	private string inputField = "";

	private List<LobbyChat.LobbyChatEntry> chatEntries = new List<LobbyChat.LobbyChatEntry>();
	private MainScreen mainScreenScript;
	private float lastUnfocus = 0;
	private string playerName;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	// Used when the game start
	void Awake() {

		mainScreenScript = MainScreen.Script;
	
		window = new Rect(30, 30, Screen.width-60, 40);
	}

	// Use this for initialization
	void Start() {

		chatEntries = new List<LobbyChat.LobbyChatEntry>();
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	// GUI stuff
	void OnGUI() {

		bool userHasHitReturn = false;
		string stMsg = "";

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

			waitFocusDelay = 0.25f;
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

		if(e.type == EventType.keyDown && e.keyCode == KeyCode.Escape) {

			// Pressing ESC at anytime should dismiss the chat window
			Debug.Log("ESC pressed");

			CloseChatWindow();
			return;
		}

		if (e.type == EventType.keyDown && e.character == '\n' && inputField.Length > 0) {
			
			HitEnter(inputField);
			CloseChatWindow();
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
		Debug.Log("Message sent across the network.");
		inputField = ""; // Clear input line
		GUI.UnfocusWindow(); // Deselect chat window
		lastUnfocus = Time.time;
	} 

}
