using UnityEngine;
using System.Collections.Generic;

public class InGameChat : MonoBehaviour {

	/* According to the assignment, pressing 't' during the game should allow the players to chat.
	 * Press 't', type the message and hit enter to send.
	 * Pressing 'esc' should cancel the chat and allow the player to play again.
	 */

	private bool showChat = false;
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

	// Use this for initialization
	void Awake() {

		mainScreenScript = MainScreen.Script;
	
		window = new Rect(30, 30, Screen.width-60, 40);
	}

	void Start() {

		chatEntries = new List<LobbyChat.LobbyChatEntry>();
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	void OnGUI() {

		bool userHasHitReturn = false;
		string stMsg = "";

		Event e = Event.current;

		if(e.keyCode == KeyCode.T && !showChat) {
			// FIXME: or the 't' key appears on the input field, or the window appears without been focused. What to do?
//			GUI.FocusWindow(7);
//			GUI.FocusControl("Chat input field");

			ShowChatWindow();
		}

		if(!showChat)
			return;

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
	void ShowChatWindow() {

		playerName = MainScreen.playerName;

		showChat = true;
		inputField = "";

		// DEBUG
		Debug.Log("Chat on");
	}

	void CloseChatWindow() {

		showChat = false;
		inputField = "";
		//chatEntries = new List<LobbyChat.LobbyChatEntry>();

		// DEBUG
		Debug.Log("Chat off");
	}

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

	void HitEnter(string stMsg)
	{
		
		stMsg = stMsg.Replace ("\n", "");
		//networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, stMsg);
		Debug.Log("Message sent across the network.");
		inputField = ""; // Clear input line
		GUI.UnfocusWindow(); // Deselect chat window
		lastUnfocus = Time.time;
	}
}
