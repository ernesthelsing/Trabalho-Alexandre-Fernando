using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour {

	// PUBLIC
	// Points to itself
	public static LevelControl Script;

	public float TimeCounter = 0.0f;

	public GameObject player;
	//menu variables//
	public GUISkin skin;
	public float width;
	public float height;
	public float levelTimer = 30; // Time of a level game, in seconds. Must be within 120 and 300.
	public float endRestartTimer = 15;
	
	public GameObject[] allPlatforms = null; // All platforms from the level
	public float platformMinAmplitude = 1.0f;
	public float platformMaxAmplitude = 10.0f;
	public float platformMinPeriod = 2.0f;
	public float platformMaxPeriod = 3.0f;
	public GameObject[] allCannons = null; // All canons in the game

	public bool GameStarted {
		get { return gameStarted; }
		set { gameStarted = value; }
	}

	// PRIVATE
	private PlayerControl playerControl = null;
	private bool endOfGame = false;
	private bool startEndCount = false;
	private float endTime = 0.0f;
	private float menuTime = 0.0f;
	private string playerTimeString;
	private bool showMenu;
	private string stLevelTimer;
	private TimeSpan tsLevelTimer;
	private float fNetTimer;
	private bool showExitButton = false;
	
	// Fernando //
	private ScoreCounter scoreScript;
	private NetworkGame networkScript;
	// End Fernando //

	private bool gameStarted = false;
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	void Awake(){
		
		Script = this;

		scoreScript = GameObject.Find("GameCode").GetComponent<ScoreCounter>();
		
		networkScript = GameObject.Find("NetworkCode").GetComponent<NetworkGame>();
		
		levelTimer = (2 + networkScript.levelTimeNetworkGame) * 60;

		menuTime = Time.time;
	}

	// Use this for initialization
	void Start () {
		
		NetworkGame.Script.InitializeSpawnPoint();
		NetworkGame.Script.SpawnPlayers();
		
		// Array for the plaforms
		allPlatforms = GameObject.FindGameObjectsWithTag("Platform");

		GatherAllPlatformInfo();	

		GatherAllCannonsInfo();
	}
	
	// Update is called once per frame
	void Update () {

		if(!gameStarted) {

			if(playerControl != null) {

					playerControl.SetMotionStatus(false);
			}
		}

		// This is the main game timer, which is synchronized through network to all clients
		if(Network.isServer) {
			//The game only runs when its actually started
			if(gameStarted){
			// Updates the network timer, for platform synchronization
			TimeCounter += Time.deltaTime;
			//While its not under visual 0, time should be running, then it should stop
			if(levelTimer > 0.9f)
				{
				//levelTimer = levelTimer - fNetTimer;
				levelTimer -= Time.deltaTime; // Assuming the timer showing to all players is in sync
				}
				else{
					
					//No need to keep calling functions once they did what they had to do
					if(!endOfGame){ 					
					MatchEnded();
					endOfGame = true;				
					}
					
					
				}
				
			//TimeSpan.FromSeconds showing level time is faster
			//if(levelTimer < 0.9f)Debug.Log("secondaryTimer: " + levelTimer);
			
			}else{
				
				if(startEndCount){
						
						endRestartTimer -= Time.deltaTime;
						
						//Debug.Log("End Restart Timer: " + endRestartTimer);
						
						if(endRestartTimer < 0.5f){
							if(networkScript != null)
								networkScript.HostLaunchGameTarget(Application.loadedLevel);
							else Debug.LogWarning("Could not find networkgame component or object");
						}
						
					}
				
				
				
			}
		}
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Toggle(){
	
		showMenu = !showMenu;
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * IN-GAME HUD
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void OnGUI(){

		// Set up the skin preferences
		GUI.skin = skin;

		// Show the level timer
		tsLevelTimer = TimeSpan.FromSeconds(levelTimer);
		
		if(startEndCount)
			tsLevelTimer = TimeSpan.FromSeconds(endRestartTimer);
		
		stLevelTimer = string.Format("{0:D2}:{1:D2}", tsLevelTimer.Minutes, tsLevelTimer.Seconds);
		GUILayout.Label("Time: " + stLevelTimer );

		// DEBUG
		fNetTimer = GetTimeCounter();
		
		// Game not started yet?
		if(!gameStarted) {

			MainMenu();
			return;
		}

		// Game rolling...
		Event e = Event.current;

		// If the player pressed F1, show an option to disconnect from the current game
		if(e.keyCode == KeyCode.F1 && !showExitButton) {

			// DEBUG
			Debug.Log("ESC Pressed");

			// DEBUG
			Debug.Log("[LevelControl] Button on.");	

			SetMotionStatus(false);
			showExitButton = true;
		}

		if(showExitButton) {

			if(GUILayout.Button("Exit Game")) {

				// Disconnects from the game
				Network.Disconnect(200);
				// Go back to the main menu
				Application.LoadLevel("main_screen");
			}

			if(GUILayout.Button("Back")) {

				SetMotionStatus(true);
				showExitButton = false;
			}
		}
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief
	 * @return
	 */
	void MainMenu(){
		
		if(!ScoreCounter.Script.showScoreBoard) {

			if(Network.isServer) {

				if(GUILayout.Button("Start"))	{

					networkView.RPC("GameStarting", RPCMode.All);
				}
			}
			else {

				GUILayout.Label("Waiting for server start the game.");
			}
		}
	}
	
	//
	public void SetPlayerGameObject(GameObject goPlayer) {

		if(player == null) {

			player = goPlayer;
			playerControl = player.GetComponent<PlayerControl>();

			// DEBUG
			Debug.Log("[LevelControl]SetPlayerGameObject: " + goPlayer);
		}
	}

	/*
	 * @brief		Stuff that is done when the player reaches the end area
	 * @param		
	 * @return	void
	 */
	public void PlayerReachedEndArea(GameObject goPlayer) {

		// Fernando //
		if(Network.isServer) {

			scoreScript.ScoreUpdate(goPlayer.networkView.owner);
		}
		else {

			networkView.RPC("Score",RPCMode.Server,goPlayer.networkView.owner);
		}
		// End Fernando //
		
		// Moves the player back to the respawn point
		Vector3 start = goPlayer.GetComponent<PlayerControl>().StartingPosition;
		goPlayer.transform.position = start;
	}
	
	
	/*
	 * @brief		Calls when the timer reaches zero
	 * @param		
	 * @return	void
	 */
	
	void MatchEnded(){
		
		Debug.Log("Match ended once");
		
		gameStarted = false;
		startEndCount = true;
		
		foreach(ScoreCounter.PlayerScore ps in scoreScript.playersScores)
			networkView.RPC("UpdateScoreList",RPCMode.Others,ps.netPlayer,ps.playerName,ps.score);
		
		
		scoreScript.SortScore();
		scoreScript.showScoreBoard = true;
		
		//Debug.Log("Start End Count: " + startEndCount);
		
	}

	/*
	 * @brief		A shortcut to change the player motion from another script
	 * @param		bnStatus	False to disable the player control
	 * @return	void
	 */
	public void SetMotionStatus(bool bnStatus) {

		// DEBUG
		//Debug.Log("[LevelControl] Changing SetMotionStatus to:" + bnStatus);

		playerControl.SetMotionStatus(bnStatus);
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * NETWORK STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {

		float temp = 0.0f;
		bool tempShowScoreBoard = false;
		float tempLevelTimer = 0.0f;

		if(stream.isWriting) {

			// Updates the timer through the network
			temp = TimeCounter;
			
			tempShowScoreBoard = scoreScript.showScoreBoard;
			
			tempLevelTimer = levelTimer;
			stream.Serialize(ref temp);
			stream.Serialize(ref tempShowScoreBoard);
			stream.Serialize(ref tempLevelTimer);
			
		}
		else {

			// Gets the timer from the network and updates here, in the client
			stream.Serialize(ref temp);
			
			stream.Serialize(ref tempShowScoreBoard);
			
			stream.Serialize(ref tempLevelTimer);
			TimeCounter = temp;
			scoreScript.showScoreBoard = tempShowScoreBoard;
			levelTimer = tempLevelTimer;
		}
	}
	
	/*
	 * @brief		Returns the main game timer
	 * @param		void
	 * @return	float with them main game timer
	 */
	public float GetTimeCounter() {

		return TimeCounter;
	}
	
	
	/*
	 * @brief		Sets score when player has reached objective
	 * @param		NetworkPlayer
	 * @return	void
	 */
	[RPC]
	void Score(NetworkPlayer player){
		
		scoreScript.ScoreUpdate(player);
		
	}
	
		/*
	 * @brief		Sets score when player has reached objective
	 * @param		NetworkPlayer
	 * @return	void
	 */
	[RPC]
	void UpdateScoreList(NetworkPlayer player,string name, int score){
		
		scoreScript.UpdateScoreLists(player,name,score);
		
	}
	

	/*
	 * @brief		Signal to all clients that the game has started
	 * @param		void
	 * @return	void
	 */
	[RPC]
	void GameStarting() {

		GameStarted = true;
		playerControl.SetMotionStatus(true);	
	}

	/*
	 * @brief		Gather all platforms game objects in an array. Executed in the server and in clients also
	 * @param		void
	 * @return	void
	 */ 
	public void GatherAllPlatformInfo() {

		// Create random values for the platforms
		if(Network.isServer) {

			for(int nIdx = 0; nIdx < allPlatforms.Length; nIdx++) {

				// Randomizes a new amplitude for this platform
				float aAmplitude = UnityEngine.Random.Range(platformMinAmplitude,platformMaxAmplitude);
				// Randomizes a new period for the platform (Speed)
				float bPeriod = 1.0f;// UnityEngine.Random.Range(platformMinPeriod, platformMaxPeriod);

				// Changes this platform in all connected games
				networkView.RPC("SetPlatformStartUpValues", RPCMode.All, nIdx, aAmplitude, bPeriod);
			}
		}
	}

	/*
	 * @brief		For each platform, calls the procedure which will set up it's initial values
	 * @param		nPlatformIdx	The index of the platform in the array list of all platforms
	 * @param		aAmplitude		Amplitude of the platform
	 * @param		bPeriod				Period (speed) of the platform
	 * @return	void
	 */
	[RPC]
	private void SetPlatformStartUpValues(int nPlatformIdx, float aAmplitude, float bPeriod) {

		GameObject thisPlatform = allPlatforms[nPlatformIdx];

		thisPlatform.GetComponent<NetPlatformControl>().SetupPlatform(aAmplitude, bPeriod);
	}

	/*
	 * @brief		Gather all cannon game objects in an array. Executed in the server and in clients also
	 * @param		void
	 * @return	void
	 */ 
	public void GatherAllCannonsInfo() {

		// Populate the the cannon array for all instances (server and clients)
		allCannons = GameObject.FindGameObjectsWithTag("Cannon");

		for(int nIdx = 0; nIdx < allCannons.Length; nIdx++) {

			// Sets the unique index number for all cannons in all instances
			GameObject thisCannon = allCannons[nIdx];
			thisCannon.GetComponent<NetCannonControl>().SetMyIndex(nIdx);
		}
	}

}
