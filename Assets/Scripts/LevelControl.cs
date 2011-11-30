using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour {
	
	//private GameObject[] platforms;

	// Points to itself
	public static LevelControl Script;
	
	private bool endOfGame = false;
	
	private float endTime = 0.0f;
	//private float playerTime = 0.0f;
	private float menuTime = 0.0f;
	
	private string playerTimeString;
	
	private PlayerControl playerControl;
	
	public GameObject player;
	
	static public bool gameStarted = false;
	
	//menu variables//
	//public GUISkin skin;
	public float width;
	public float height;
	private bool showMenu;
	
	private bool masterMode = false;
	public Camera masterModeCamera;
	
	void Awake(){
		
		Script = this;

		menuTime = Time.time;

/*
		masterModeCamera = GameObject.FindWithTag("EditorCamera").GetComponent<Camera>();
		if(masterModeCamera == null)
			Debug.LogError("Master camera not found!");
	*/	
	}
	
	// Use this for initialization
	void Start () {
		
		showMenu = true;
		if(player != null) {

			playerControl = player.GetComponent<PlayerControl>();
		}
		
		//platforms = GameObject.FindGameObjectsWithTag("Platform");
	
	}
	
	// Update is called once per frame
	void Update () {
		
	
		// TODO: added to change the camera to Master Mode
		if(Input.GetKeyDown("tab")) {

			ToogleMasterMode();
		}
	}

	void ToogleMasterMode() {
		
		playerControl.SetMotionStatus(masterMode);

		masterMode = !masterMode;

		// switch cameras
		Camera.main.enabled = masterMode;
		masterModeCamera.enabled = !masterMode;

	}
	
	void Toggle(){
	
		showMenu = !showMenu;
		
	}
	
	void OnGUI(){
		
		GUILayout.Label("Lives: " + playerControl.lives);
		
		if(playerControl.lives <= 0)
		{
			DeathMenu();
		}
		
		if(!showMenu) return;
		
		playerControl.SetMotionStatus(false);
		
		float screenX = Screen.width * 0.5f - width * 0.5f;
		float screenY = Screen.height * 0.5f - height * 0.5f;
		
		GUILayout.BeginArea(new Rect(screenX,screenY,width,height) );
		
		if(endOfGame)
		{
			
			GUILayout.Label("You finished the level in " + playerTimeString + " seconds");
			if(GUILayout.Button("Restart"))
			Application.LoadLevel("Game");
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
	
	void MainMenu(){
		
		if(GUILayout.Button("Start"))
		{
			menuTime = Time.time - menuTime;
			Toggle();
			playerControl.SetMotionStatus(true);	
			gameStarted = true;
			
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
		
		if(other.gameObject.name.Equals("First Person Controller"))
		{
			endOfGame = true;
			endTime = Time.realtimeSinceStartup - menuTime;
			Debug.Log("Player got to end and menuTime is " + menuTime);
			playerTimeString = string.Format("{0:0.00}", endTime);
			Toggle();
			Debug.Log("Player got to end");
		}
		
		
	}

	//
	public void SetPlayerGameObject(GameObject goPlayer) {

		if(player == null) {

			player = goPlayer;
			playerControl = player.GetComponent<PlayerControl>();
		}
	}

}
