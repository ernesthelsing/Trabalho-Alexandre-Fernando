using UnityEngine;
using System.Collections.Generic;

public class MainScreen : MonoBehaviour {

	// Points to itself
	public static MainScreen Script;

	// Menu Variables
	private delegate void GUIMethod ();
	private GUIMethod currentMenu;
	float screenX;
	float screenY;
	public float menuHeight = 450;
	public float menuWidth = 300;
	public float subMenuWidth = 300;
	public float subMenuHeight = 300;
	Rect subMenuWindow = new Rect();
	public float windowOffsetX = 20;
	public float windowOffsetY = 10;

	// Background variables
	public int guiDepth = 0;
	//public string levelToLoad = "";
	// this has to correspond to a level (file>build settings)
	public Texture2D mainScreenBG;
	// the logo to splash;
	private Rect mainScreenBGPos = new Rect ();

	// Network stuff
	private string serverName;
	public static string playerName = "";
	private NetworkGame netScript = null;

	// chat stuff
	//private Vector2 chatScrollPos = new Vector2 (10, 20);
	//private string inputField = "";
	private float boxWidth;
	private float boxHeight;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	// Use this for initialization
	void Awake()
	{
		
		Script = this;

		// Background stuff
		mainScreenBGPos.x = 0;
		mainScreenBGPos.y = 0;
		
		mainScreenBGPos.width = Screen.width;
		mainScreenBGPos.height = Screen.height;
		
		// Menu stuff
		screenX = Screen.width * 0.5f - menuWidth * 0.5f;
		screenY = Screen.height * 0.5f - menuHeight * 0.5f;
		currentMenu = MainMenu;
	
		// Host game window stuff	
		subMenuWindow.x = Screen.width * 0.5f - subMenuWidth * 0.5f;
		subMenuWindow.y = Screen.height * 0.5f - subMenuHeight * 0.5f;
		
		subMenuWindow.width = subMenuWidth;
		subMenuWindow.height = 150;
		
		// Network stuff
		netScript = GameObject.Find("NetworkCode").GetComponent<NetworkGame>();
		serverName = netScript.GetServerName();

		if(playerName == null || playerName == "") {

			playerName = "RandomName" + Random.Range(1,999);
		}
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

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * MENU STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Main menu for the game
	 * @param		
	 * @return	void
	 */
	void MainMenu()
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

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * HOST GAME STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Menu for hosting a game
	 * @param
	 * @return	void
	 */
	void HostGameMenu()
	{
		
		GUILayout.BeginArea (new Rect (subMenuWindow.x, subMenuWindow.y, subMenuWindow.width, subMenuWindow.height));
		{
			subMenuWindow = GUI.Window (1, subMenuWindow, HostGameMenuWindow, "Host Network Game");
		}
		GUILayout.EndArea ();
	}

	/*
	 * @brief		Hosting game window
	 * @param		nId	Unity's window code
	 * @return	void
	 */
	void HostGameMenuWindow(int nId)
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
			if (GUILayout.Button ("Create server")) {
				
				netScript.StartMasterServer();

				// FIXME: passing to network script?
				currentMenu = LobbyMenu;
			} else if (GUILayout.Button ("Back")) {
				
				currentMenu = MainMenu;
			}
			
		}
		GUILayout.EndVertical ();
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * JOIN GAME STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Menu for joining a game
	 * @param		
	 * @return	void
	 */
	void JoinGameMenu()	{
		
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

	/*
	 * @brief		Join game window
	 * @param		nId	Unity's window code
	 * @return	void
	 */
	void JoinGameMenuWindow (int nId)	{
		
		HostData[] hostsOnline = netScript.GetOnlineHostList();
		
		{
			GUILayout.Label ("Player name");
			playerName = GUILayout.TextField(playerName, GUILayout.MinWidth (100));
			GUILayout.Space (10);
		}

		GUILayout.BeginHorizontal();
		{
			
			if (hostsOnline.Length == 0) {
				
				// No hosts found
				GUILayout.Label ("No host found.");

				// FIXME: remove this code
				if(!netScript.UseMasterServer) {
					if (GUILayout.Button ("Connect")) {
						netScript.ConnectWithServer(null);
					}
				}
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
						netScript.ConnectWithServer(uniqueHost);
						currentMenu = LobbyMenu;
					}
				}
			}
		}
			
		if(GUILayout.Button ("Back")) {
				
			currentMenu = MainMenu;
		}
		GUILayout.EndHorizontal ();
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * LOBBY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Menu for the lobby on the game.
	 * @param
	 * @return	void
	 */
	void LobbyMenu() {
	
		// Defines the menu area	
		LobbyPlayerSetup.Script.ShowThisMenu();

		if(Network.isServer) {

			float buttonHeight = 20;
			float buttonWidth = 80;

			if(GUI.Button(new Rect(Screen.width * 0.5f - buttonWidth * 0.5f,
							Screen.height - buttonHeight*2, buttonWidth, buttonHeight), "Start game")) {

					netScript.HostLaunchGame();
			}
		}
		else {

			float buttonHeight = 20;
			float buttonWidth = 120;

			GUI.Label(new Rect(Screen.width * 0.5f - buttonWidth * 0.5f,
							Screen.height - buttonHeight*2, buttonWidth, buttonHeight), 
					"Waiting for the game start...");
		}
	}
}
