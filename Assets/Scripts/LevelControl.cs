using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour {

	// PUBLIC
	// Points to itself
	public static LevelControl Script;

	public float TimeCounter = 0.0f;
	public float LevelTimer = 100;	// Level time in seconds. Must be between 120 and 300 seconds;

	public GameObject player;
	static public bool gameStarted = false;
	//menu variables//
	public GUISkin skin;
	public float width;
	public float height;
	public AudioClip EndReachedSound;
	public float levelTimer = 120; // Time of a level game, in seconds. Must be within 120 and 300.
	
	// PRIVATE
	private bool endOfGame = false;
	private float endTime = 0.0f;
	private float menuTime = 0.0f;
	private string playerTimeString;
	private PlayerControl playerControl;
	private bool showMenu;
	private string stLevelTimer;
	private TimeSpan tsLevelTimer;
	private float fNetTimer;
	
	// Fernando //
	private ScoreCounter scoreScript;
	// End Fernando //
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
		
		// From InGamePlay
		// DEBUG
		Debug.Log("Spawning player.");
		// FIXME:
		NetworkGame.Script.InitializeSpawnPoint();
		NetworkGame.Script.SpawnPlayers();
		// End - InGamePlay
		
		showMenu = true;
		if(player != null) {

			playerControl = player.GetComponent<PlayerControl>();
		}
		
		//platforms = GameObject.FindGameObjectsWithTag("Platform");
	
	}
	
	// Update is called once per frame
	void Update () {

		/* From InGamePlay */

		// This is the main timer, which is synchronized through network to all clients
		if(Network.isServer) {

			// Updates the network timer
			TimeCounter += Time.deltaTime;
		}
		/* End - InGamePlay */
		

		
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
		GUILayout.Label("NetTimer: " + fNetTimer);
		
		// FIXME
		//if(!showMenu) return;
		
		playerControl.SetMotionStatus(false);
		
		float screenX = Screen.width * 0.5f - width * 0.5f;
		float screenY = Screen.height * 0.5f - height * 0.5f;
		
		GUILayout.BeginArea(new Rect(screenX,screenY,width,height) );
		
		// FIXME

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
		
//		if(GUILayout.Button("Start"))	{

			menuTime = Time.time - menuTime;
			Toggle();
			playerControl.SetMotionStatus(true);	
			gameStarted = true;
	//	}
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

	public float GetTimeCounter() {

		return TimeCounter;
	}
}
