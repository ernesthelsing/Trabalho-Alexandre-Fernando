using UnityEngine;
using System.Collections.Generic;

public class MainScreen : MonoBehaviour
{

	// Menu Variables
	private delegate void GUIMethod ();
	private GUIMethod currentMenu;
	public float menuWidth;
	public float menuHeight;

	private float screenX;
	private float screenY;

	// Menu windows
	public float subMenuHeight;
	public float subMenuWidth;
	private Rect subMenuWindow = new Rect ();
	private float subMenuX;
	private float subMenuY;

	public float windowOffsetX = 20;
	public float windowOffsetY = 10;

	// Background variables
	public int guiDepth = 0;
	public string levelToLoad = "";
	// this has to correspond to a level (file>build settings)
	public Texture2D mainScreenBG;
	// the logo to splash;
	private Rect mainScreenBGPos = new Rect ();

	// Network stuff
	private string serverName;
	public static string playerName = "";
	private NetworkGame netScript = null;

	// chat stuff
	private Vector2 chatScrollPos = new Vector2 (10, 20);
	private string inputField = "";
	private float boxWidth;
	private float boxHeight;

	// Use this for initialization
	void Start ()
	{
		
		// Background stuff
		mainScreenBGPos.x = 0;
		mainScreenBGPos.y = 0;
		
		mainScreenBGPos.width = Screen.width;
		mainScreenBGPos.height = Screen.height;
		
		// Menu stuff
		screenX = Screen.width * 0.5f - menuWidth * 0.5f;
		screenY = Screen.height * 0.5f - menuHeight * 0.5f;
		currentMenu = MainMenu;
		
		// Network stuff
		netScript = gameObject.GetComponent<NetworkGame> ();
		serverName = netScript.GetServerName ();
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnGUI ()
	{
		
		// Draws the background texture
		GUI.DrawTexture (mainScreenBGPos, mainScreenBG);
		
		currentMenu ();
	}

	/* ---------------------------------------------------------------------------------------------------------- */

	/// <summary>
	/// Main Menu
	/// </summary>
	void MainMenu ()
	{
		
		GUILayout.BeginArea (new Rect (screenX, screenY, menuWidth, menuHeight));
		
		if (GUILayout.Button ("Start local game")) {
			
			
		} else if (GUILayout.Button ("Host network game")) {
			
			currentMenu = HostGameMenu;
		} else if (GUILayout.Button ("Join network game")) {
			
			currentMenu = JoinGameMenu;
		} else if (GUILayout.Button ("Quit")) {
			
			Application.Quit ();
		}
		
		GUILayout.EndArea ();
	}

	/// <summary>
	/// Host NetworkGame
	/// </summary>
	void HostGameMenu ()
	{
		
		subMenuWindow.x = Screen.width * 0.5f - subMenuWidth * 0.5f;
		subMenuWindow.y = Screen.height * 0.5f - subMenuHeight * 0.5f;
		
		subMenuWindow.width = subMenuWidth;
		subMenuWindow.height = 150;
		
		GUILayout.BeginArea (new Rect (subMenuWindow.x, subMenuWindow.y, subMenuWindow.width, subMenuWindow.height));
		{
			subMenuWindow = GUI.Window (1, subMenuWindow, HostGameMenuWindow, "Host Network Game");
		}
		GUILayout.EndArea ();
	}

	/// <summary>
	/// 
	/// </summary>
	void HostGameMenuWindow (int nId)
	{
		
		GUILayout.BeginVertical ();
		{
			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space (10);
				GUILayout.Label ("Server name");
				serverName = GUILayout.TextField(serverName, GUILayout.MinWidth (100));
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space (10);
				GUILayout.Label ("Player name");
				playerName = GUILayout.TextField(playerName, GUILayout.MinWidth (100));
			}
			GUILayout.EndHorizontal ();
			
			GUILayout.Space (10);
			if (GUILayout.Button ("Start game")) {
				
				netScript.StartMasterServer();

				// FIXME: passing to network script?
				currentMenu = LobbyMenu;
			} else if (GUILayout.Button ("Back")) {
				
				currentMenu = MainMenu;
			}
			
		}
		GUILayout.EndVertical ();
	}

	/// <summary>
	/// Join Game Menu
	/// </summary>
	void JoinGameMenu ()
	{
		
		subMenuWindow.x = Screen.width * 0.5f - subMenuWidth * 0.5f;
		subMenuWindow.y = Screen.height * 0.5f - subMenuHeight * 0.5f;
		
		subMenuWindow.width = subMenuWidth;
		subMenuWindow.height = 150;
		
		GUILayout.BeginArea (new Rect (subMenuWindow.x, subMenuWindow.y, subMenuWindow.width, subMenuWindow.height));
		{
			subMenuWindow = GUI.Window (2, subMenuWindow, JoinGameMenuWindow, "Join Network Game");
		}
		GUILayout.EndArea ();
	}

	/// <summary>
	/// 
	/// </summary>
	void JoinGameMenuWindow (int nId)
	{
		
		HostData[] hostsOnline = netScript.GetOnlineHostList ();
		
		GUILayout.BeginHorizontal ();
		{
			
			if (hostsOnline.Length == 0) {
				
				// No hosts found
				GUILayout.Label ("No host found.");
			} else {
				
				foreach (HostData uniqueHost in hostsOnline) {
					
					string hostInfo = uniqueHost.gameName + " " + uniqueHost.connectedPlayers + "/" + uniqueHost.playerLimit;
					
					GUILayout.Label (hostInfo);
					GUILayout.Space (5);
					
					GUILayout.Label (uniqueHost.comment);
					GUILayout.Space (5);
					GUILayout.FlexibleSpace ();
					
					if (GUILayout.Button ("Connect")) {
						
						// FIXME: network stuff goes to the network script
						Network.Connect (uniqueHost);
						currentMenu = LobbyMenu;
					}
				}
			}
		}
		GUILayout.EndHorizontal ();
	}

	/// <summary>
	/// Player Setup Menu
	/// </summary>
	void LobbyMenu ()
	{
		
		subMenuWindow.x = Screen.width * 0.5f - (Screen.width - windowOffsetX * 2) * 0.5f;
		subMenuWindow.y = Screen.height * 0.5f - (Screen.height - windowOffsetY * 2) * 0.5f;
		
		subMenuWindow.width = Screen.width - windowOffsetX * 2;
		subMenuWindow.height = Screen.height - windowOffsetY * 2;
		
		GUILayout.BeginArea (new Rect (subMenuWindow.x, subMenuWindow.y, subMenuWindow.width, subMenuWindow.height));
		{
			subMenuWindow = GUI.Window (3, subMenuWindow, LobbyMenuWindow, "Players Setup Lobby");
		}
		GUILayout.EndArea ();
	}

	/// <summary>
	/// Lobby Menu Window
	/// </summary>
	void LobbyMenuWindow (int nId)
	{
		
		// First, we divide the screen horizontally in two
		float marginX = 10;
		float marginY = 20;
		boxWidth = subMenuWindow.width * 0.5f - (marginX * 1.5f);
		boxHeight = subMenuWindow.height - (marginY * 2);
		Rect chatWindow = new Rect (marginX, marginY, boxWidth, boxHeight);
		// Added title offset
		Rect playerSetupBox = new Rect (marginX * 2 + boxWidth, marginY, boxWidth, boxHeight / 4);
		// Added title offset
		//	GUILayout.BeginHorizontal();
		{
			/*
			// Left: chat
			//	GUILayout.BeginArea(chatWindow);
			//{
			chatWindow = GUI.Window (4, chatWindow, GlobalChatWindow, "Server chat");
			/*GUI.Box(chatWindow, "Server chat");
				chatScrollPos = GUILayout.BeginScrollView(chatScrollPos, GUILayout.Width(boxWidth), GUILayout.Height(boxHeight));
				{
					List<NetworkGame.ChatEntry> chatEntriesCopy = netScript.GetChatEntries();
				
					foreach(NetworkGame.ChatEntry entry in chatEntriesCopy) {
					
						GUILayout.BeginHorizontal();
						GUILayout.Label(entry.text);
						GUILayout.EndHorizontal();
					}
				}
				GUI.EndScrollView();
				
				if(Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0) {
					
					HitEnter(inputField);
				}
				GUI.SetNextControlName("Chat input field");
				inputField = GUILayout.TextField(inputField);
			}
			//GUILayout.EndArea();
			*/
			
			// Right side: connected players
			GUI.Box (playerSetupBox, "Player #1");
		}
		//	GUILayout.EndHorizontal();
	}

	/*
	void GlobalChatWindow (int nId)
	{
		
		GUILayout.BeginVertical ();
		GUILayout.Space (10);
		GUILayout.EndVertical ();
		
		chatScrollPos = GUILayout.BeginScrollView (chatScrollPos, GUILayout.Width (boxWidth), GUILayout.Height (boxHeight));
		{
			List<NetworkGame.ChatEntry> chatEntriesCopy = netScript.GetChatEntries ();
			
			foreach (NetworkGame.ChatEntry entry in chatEntriesCopy) {
				
				GUILayout.BeginHorizontal ();
				GUILayout.Label (entry.text);
				GUILayout.EndHorizontal ();
			}
		}
		GUI.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0) {
			
			HitEnter (inputField);
		}
		GUI.SetNextControlName ("Chat input field");
		inputField = GUILayout.TextField (inputField);
	}

	void HitEnter (string stMsg)
	{
		
		stMsg = stMsg.Replace ("\n", "");
		netScript.SendChatMessage (stMsg);
		inputField = "";
		lastUnfocus = Time.time;
		usingChat = false;
	}
	*/
}

