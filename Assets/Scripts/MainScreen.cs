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
	public float menuHeight = 100;
	public float menuWidth = 400;

	// Host Menu Stuff
	public float hostMenuWidth = 600;
	public float hostMenuHeight = 300;
	public float hostMenuOffsetY = 50;
	
	public int levelTimeToolbarInt = 0;
	public string[] levelTimeToolbarStrings = new string[]{"-2-", "-3-","-4-", "-5-"};

	// Join Menu Stuff
	public float joinMenuWidth = 600;
	public float joinMenuHeight = 300;

	Rect subMenuWindow = new Rect();
	public float windowOffsetX = 20;
	public float windowOffsetY = 10;

	public int maxInputLength = 20;
	public Vector2 scrollPosition;

	public GUISkin skin;

	// Player models/colors selection
	public static int playerUniqueAvatarsNumber = 4;
	// playerAvatarIcons and playerAvatarColors must match
	public Texture2D[] playerAvatarIcons = new Texture2D[playerUniqueAvatarsNumber];
	private Color[] playerAvatarColors = 
		new Color[] { Color.red, Color.green, Color.blue, Color.yellow};
	public static int playerAvatarIdx = 0;

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
		screenY = 200;
		//screenY = Screen.height * 0.5f - menuHeight * 0.5f;
		currentMenu = MainMenu;
	
		// Host game window stuff	
		subMenuWindow.x = Screen.width * 0.5f - hostMenuWidth * 0.5f;
		subMenuWindow.y = (Screen.height * 0.5f - hostMenuHeight * 0.5f) + hostMenuOffsetY;
		
		subMenuWindow.width = hostMenuWidth;
		subMenuWindow.height = hostMenuHeight;
		
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
		
		GUI.skin = skin;

		// Draws the background texture
		GUI.DrawTexture (mainScreenBGPos, mainScreenBG);
		
		currentMenu();
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
		
		GUILayout.BeginArea (new Rect (screenX,100 + screenY, menuWidth, menuHeight));
		
		if (GUILayout.Button ("Host network game")) {
			
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
				serverName = 
					GUILayout.TextField(serverName, maxInputLength, GUILayout.MinWidth (200), GUILayout.MaxWidth(200));
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			{
				GUILayout.Space (10);
				GUILayout.Label ("Player name");
				playerName = 
					GUILayout.TextField(playerName, maxInputLength, GUILayout.MinWidth (200), GUILayout.MaxWidth(200));
			}
			GUILayout.EndHorizontal();
			
			PlayerSelectButtons();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Match Time(in minutes):");
			levelTimeToolbarInt = GUILayout.Toolbar(levelTimeToolbarInt,levelTimeToolbarStrings);
			GUILayout.EndHorizontal();
			
			
			GUILayout.Space (10);
			if (GUILayout.Button ("Create server")) {
				
				netScript.levelTimeNetworkGame = levelTimeToolbarInt;
				
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
		
		subMenuWindow.x = Screen.width * 0.5f - joinMenuWidth * 0.5f;
		subMenuWindow.y = (Screen.height * 0.5f - joinMenuHeight * 0.5f);
		
		subMenuWindow.width = joinMenuWidth;
		subMenuWindow.height = joinMenuHeight;
		
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
		
		GUILayout.BeginHorizontal ();
		{
			GUILayout.Label ("Player name");
			playerName = 
				GUILayout.TextField(playerName, 20, GUILayout.MinWidth(200), GUILayout.MaxWidth(200));
			GUILayout.Space (10);
			// TODO: add player selection here
		}
		GUILayout.EndHorizontal();

		// Player selection
		PlayerSelectButtons();

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

					string hostInfo = 
						uniqueHost.gameName + " " + uniqueHost.connectedPlayers + "/" + uniqueHost.playerLimit;

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

	/*
	 * @brief		Shows the buttons with icons where the player can select his avatar
	 * @param		void
	 * @return	void
	 */
	void PlayerSelectButtons() {

			GUILayout.BeginHorizontal();
			{

				GUILayout.Space (10);
				playerAvatarIdx = GUILayout.Toolbar(playerAvatarIdx, playerAvatarIcons);
			}
			GUILayout.EndHorizontal();
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
	
		// Defines the menu area, this is a desperate measure	
		//LobbyPlayerSetup.Script.ShowThisMenu();

		if(Network.isServer) {

			float buttonHeight = 25;
			float buttonWidth = 200;

			if(GUI.Button(new Rect(Screen.width * 0.5f - buttonWidth * 0.5f,
							Screen.height - buttonHeight*2, buttonWidth, buttonHeight), "Start game")) {

					netScript.HostLaunchGame();
			}
		}
		else {

			float buttonHeight = 60;
			float buttonWidth = 360;

			GUI.Label(new Rect(Screen.width * 0.5f - buttonWidth * 0.5f,
							Screen.height - buttonHeight*2, buttonWidth, buttonHeight), 
					"Waiting for the game start...");
		}
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * GENERIC STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	public int GetPlayerAvatarIndex() {

		return playerAvatarIdx;
	}
}
