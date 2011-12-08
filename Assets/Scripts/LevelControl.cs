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
	public AudioClip EndReachedSound;
	public float levelTimer = 20; // Time of a level game, in seconds. Must be within 120 and 300.
	
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
	private float endTime = 0.0f;
	private float menuTime = 0.0f;
	private string playerTimeString;
	private bool showMenu;
	private string stLevelTimer;
	private TimeSpan tsLevelTimer;
	private float fNetTimer;
	
	// Fernando //
	private ScoreCounter scoreScript;
	// End Fernando //

	private bool gameStarted = false;
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	void Awake(){
		
		Script = this;
		
		// Fernando //
		scoreScript = GameObject.Find("GameCode").GetComponent<ScoreCounter>();
		// End Fernando //

		menuTime = Time.time;
	}

	// Use this for initialization
	void Start () {
		
		// FIXME:
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

			// Updates the network timer
			TimeCounter += Time.deltaTime;
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
		tsLevelTimer = TimeSpan.FromSeconds(levelTimer - fNetTimer);
		
		if(levelTimer < 0.0f) Debug.Log("Time's Up");
				
		stLevelTimer = string.Format("{0:D2}:{1:D2}", tsLevelTimer.Minutes, tsLevelTimer.Seconds);
		GUILayout.Label("Time: " + stLevelTimer );

		// DEBUG
		fNetTimer = GetTimeCounter();
		
		// Game not started yet?
		if(!gameStarted) {

			MainMenu();
			return;
		}

		// TODO: clean this mess!
		/*
		float screenX = Screen.width * 0.5f - width * 0.5f;
		float screenY = Screen.height * 0.5f - height * 0.5f;
		

		GUILayout.BeginArea(new Rect(screenX,screenY,width,height) );
		
		if(endOfGame)
		{
			
			GUILayout.Label("You finished the level in " + playerTimeString + " seconds");

			if(GUILayout.Button("Restart")) {

				// FIXME: this is likely to NOT work on network. I guess it's best to just respawn the player
				//Application.LoadLevel("Game");
				// DEBUG
				Debug.Log("Should load the level again. Waiting to be fixed.");
			}
			playerControl.SetMotionStatus(false);
		}
		else
		{

			MainMenu();
		}
		
		GUILayout.EndArea();
		*/

		//GUILayout.Label("Time Startup: " + Time.realtimeSinceStartup);
		//GUILayout.Label("Time DeltaTime: " + Time.deltaTime);
		//GUILayout.Label("Frame Count: " + Time.frameCount);

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
		
		if(Network.isServer) {

			if(GUILayout.Button("Start"))	{

				//menuTime = Time.time - menuTime;
				// FIXME: all this must be done through RPC!
				//GameStarted = true;
				//playerControl.SetMotionStatus(true);	
				networkView.RPC("GameStarting", RPCMode.All);
			}
		}
		else {

			GUILayout.Label("Waiting for server start the game.");
		}
	}
	
	void DeathMenu(){
		
		playerControl.SetMotionStatus(false);
		
		float screenX = Screen.width * 0.5f - width * 0.5f;
		float screenY = Screen.height * 0.5f - height * 0.5f;
		
		GUILayout.BeginArea(new Rect(screenX,screenY,width,height) );
		
		GUILayout.Label("You lost all your lives");
		if(GUILayout.Button("Restart"))
			Application.LoadLevel("Game");
		
		
		GUILayout.EndArea();
	}
	
	void OnTriggerEnter(Collider other){
		
		Debug.Log("This guy got to the end " + other.gameObject.name);
		
		//if(other.gameObject.name.Equals("First Person Controller"))
		if(other.gameObject.tag.Equals("Player")) {

			PlayerReachedEndArea(other.gameObject);
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
	void PlayerReachedEndArea(GameObject goPlayer) {

		// Play a sound
		if(EndReachedSound != null) {

			audio.PlayOneShot(EndReachedSound);
		}
		
		// Fernando //
		if(Network.isServer)
			scoreScript.ScoreUpdate(goPlayer.networkView.owner);
		else
			networkView.RPC("Score",RPCMode.Server,goPlayer.networkView.owner);
		// End Fernando //
		
		// Moves the player back to the respawn point
		Vector3 start = goPlayer.GetComponent<PlayerControl>().StartingPosition;
		goPlayer.transform.position = start;
		

		// FIXME: clean the stuff below
		//	endOfGame = true;
			endTime = Time.realtimeSinceStartup - menuTime;
			Debug.Log("Player got to end and menuTime is " + menuTime);
			playerTimeString = string.Format("{0:0.00}", endTime);
			//Toggle();
			Debug.Log("Player got to end");
	}
	
	// Fernando //
	[RPC]
	void Score(NetworkPlayer player){
		
		scoreScript.ScoreUpdate(player);
		
	}// End Fernando //

	/*
	 * @brief		A shortcut to change the player motion from another script
	 * @param		bnStatus	False to disable the player control
	 * @return	void
	 */
	public void SetMotionStatus(bool bnStatus) {

		// DEBUG
		Debug.Log("[LevelControl] Changing SetMotionStatus to:" + bnStatus);

		playerControl.SetMotionStatus(bnStatus);
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * NETWORK STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {

		float temp = 0.0f;

		if(stream.isWriting) {

			// Updates the timer through the network
			temp = TimeCounter;
			stream.Serialize(ref temp);
		}
		else {

			// Gets the timer from the network and updates here, in the client
			stream.Serialize(ref temp);
			TimeCounter = temp;
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
				float bPeriod = UnityEngine.Random.Range(platformMinPeriod, platformMaxPeriod);

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
