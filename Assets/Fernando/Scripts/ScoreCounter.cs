using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreCounter : MonoBehaviour {
	
	float screenX;
	float screenY;
	public float scoreMenuHeight = 450;
	public float scoreMenuWidth = 400;
	//public float ScoreSubMenuWidth = 300;
	//public float ScoreSubMenuHeight = 100;
	//Rect ScoreSubMenuWindow = new Rect();
	
	public GUIStyle scoreTextStyle = new GUIStyle();
	public Font scoreTitle;
	
	private NetworkGame networkScript;
	
	public class PlayerScore{
		
		public NetworkPlayer netPlayer;
		public string playerName;
		public int score;
		public Color nameColor;
		
	}
	
	public List<PlayerScore> playersScores;

	public bool showScoreBoard = false;
	public static ScoreCounter Script;
	
	
	//int toolbarint = 0;
	//string[] toolbarstrings = new string[]{"Toolbar 1", "Toolbar 2","Toolbar 3", "Toolbar 4"};
	
	
	void Awake() {
		
		networkScript = GameObject.Find("NetworkCode").GetComponent<NetworkGame>();
		
		playersScores = new List<PlayerScore>();
		
		//scoreTitle.material.color = Color.yellow;
		if(scoreTitle != null)
		scoreTextStyle.font = scoreTitle;
		
		scoreTextStyle.alignment = TextAnchor.MiddleCenter;
		scoreTextStyle.normal.textColor = Color.yellow;
		scoreTextStyle.fontSize = 30;
		
		screenX = Screen.width * 0.5f - scoreMenuHeight * 0.5f;
		screenY = Screen.height * 0.5f - scoreMenuWidth * 0.5f;
		
		Script = this;
	}

	// Use this for initialization
	void Start () {
		
		//Debug.Log("This script exists");
		//This populates the player list
		if(!Network.isServer) return;
		
		//This adds the server player
		PlayerScore tempPlayer = new PlayerScore();
		NetworkGame.PlayerInfo tempPlayerInfo = networkScript.GetPlayerInfoFromNetwork(networkView.owner);
		
		tempPlayer.netPlayer = networkView.owner;
		tempPlayer.playerName = tempPlayerInfo.playerName;
		tempPlayer.nameColor = ReturnColorBasedOnIndex(tempPlayerInfo.playerAvatarIdx);
		tempPlayer.score = 0;
		
		playersScores.Add(tempPlayer);
		
		//This adds the other players
		foreach(NetworkPlayer np in Network.connections){
			
			//Debug.Log("Found someone");
			
			tempPlayer = new PlayerScore();
			tempPlayerInfo = networkScript.GetPlayerInfoFromNetwork(np);
			
			tempPlayer.netPlayer = np;
			tempPlayer.playerName = tempPlayerInfo.playerName;
			tempPlayer.nameColor = ReturnColorBasedOnIndex(tempPlayerInfo.playerAvatarIdx);
			tempPlayer.score = 0;
			playersScores.Add(tempPlayer);
				
		}
		
		foreach(PlayerScore ps in playersScores)
			Debug.Log("This is player address in score list: " + ps.netPlayer.ipAddress + " this is his current score " + ps.score);
		
		//Creating fake players for testing purposes
		/*for(int i = 0; i < 4; i++){
			
			PlayerScore tempPlayer = new PlayerScore();
			//tempPlayer.netPlayer = null;
			tempPlayer.playerName = "Player" + i;
			tempPlayer.score = i * 5;
			
			playersScores.Add(tempPlayer);
					
		}*/
		
		//Call when the match is done
		//SortScore();
		
		//foreach(PlayerScore ps in playersScores)
			//Debug.Log("This is player name in score list: " + ps.playerName + " this is his current score " + ps.score);
	
	}
	
	void OnGUI(){
		
		if(showScoreBoard) {
		
			ScoreBoard();
		}
	}
	
	//Builds the scoreboard needs polish
	void ScoreBoard(){
		
		GUIStyle playersTextStyle = new GUIStyle();
		
		if(scoreTitle != null)
		playersTextStyle.font = scoreTitle;
		
		GUILayout.BeginArea (new Rect (screenX, 100 + screenY, scoreMenuWidth, scoreMenuHeight));
			
			GUILayout.BeginVertical("box");
				GUILayout.Label("Match is over",scoreTextStyle);
			GUILayout.EndVertical();
		
			GUILayout.BeginVertical("box");
				GUILayout.Label("Scoreboard",scoreTextStyle);
			GUILayout.EndVertical();
		
			foreach(PlayerScore ps in playersScores){
			
				playersTextStyle.normal.textColor = ps.nameColor;
			
				GUILayout.BeginHorizontal("box");
				GUILayout.Label(ps.playerName,playersTextStyle);
				GUILayout.Space(10);
			
				playersTextStyle.normal.textColor = Color.white;
			
				GUILayout.Label("Score: ",playersTextStyle);
				GUILayout.Label(ps.score.ToString(),playersTextStyle);
				GUILayout.EndHorizontal();
			}

		playersTextStyle.normal.background = GUI.skin.GetStyle("Button").normal.background;
		playersTextStyle.alignment = TextAnchor.MiddleCenter;
		
		if(GUI.Button(new Rect(screenX,screenY,100,40),"Exit Game",playersTextStyle)) {

				// Disconnects from the game
				Network.Disconnect(200);
				// Go back to the main menu
				Application.LoadLevel("main_screen");
			}
				
		GUILayout.EndArea();
		
		
	}
			                                      
	//Function for sort	
	public int HighScore(PlayerScore scoreA,PlayerScore scoreB){
		
		return scoreB.score - scoreA.score;
		
	}
	//Sorts players based on score, highest to lowest
	public void SortScore(){
	
		playersScores.Sort(HighScore);
		
		
	}
	
	public void UpdateScoreLists(NetworkPlayer player,string name, int score, int idxColor){
		
		PlayerScore tempPlayer = new PlayerScore();
		
		tempPlayer.netPlayer = player;
		tempPlayer.playerName = name;
		tempPlayer.score = score;
		tempPlayer.nameColor = ReturnColorBasedOnIndex(idxColor);
		                                               
		
		playersScores.Add(tempPlayer);
		
	}
	
	
	//Updates player score
	public void ScoreUpdate(NetworkPlayer nPlayer){
		
		foreach(PlayerScore ps in playersScores){
		
			if(nPlayer == ps.netPlayer){
				ps.score += 1;
				//Debug.Log("This is player address in score list: " + ps.netPlayer.ipAddress + " this is his current score " + ps.score);
				break;
			}
		}
		
	}
	
	
	Color ReturnColorBasedOnIndex(int playerIdx){
		
		Color retColor = Color.black;

		switch(playerIdx)
			{
			case 0:
				retColor = Color.red;
				break;
			case 1:
				retColor =  Color.green;
				break;
			case 2:
				retColor = Color.blue;
				break;
			case 3:
				retColor = Color.yellow;
				break;
			default:
				retColor = Color.white;
				break;
			}	
		
		return retColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
