using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreCounter : MonoBehaviour {
	
	//private InGamePlay gameScript;
	//private LevelControl levelScript;
	
	public class PlayerScore{
		
		public NetworkPlayer netPlayer;
		public int score;
		
	}
	
	public List<PlayerScore> playersScores;
	
	void Awake() {
		
		playersScores = new List<PlayerScore>();
		
	}

	// Use this for initialization
	void Start () {
		
		//Debug.Log("This script exists");
		
		if(!Network.isServer) return;
		
		PlayerScore tempPlayer = new PlayerScore();
		
		tempPlayer.netPlayer = networkView.owner;
		tempPlayer.score = 0;
		
		playersScores.Add(tempPlayer);
		
		foreach(NetworkPlayer np in Network.connections){
			
			//Debug.Log("Found someone");
			
			tempPlayer = new PlayerScore();
			
			tempPlayer.netPlayer = np;
			tempPlayer.score = 0;
			playersScores.Add(tempPlayer);
				
		}
		
		foreach(PlayerScore ps in playersScores)
			Debug.Log("This is player address in score list: " + ps.netPlayer.ipAddress + " this is his current score " + ps.score);
	
	}
	
	public void ScoreUpdate(NetworkPlayer nPlayer){
		
		foreach(PlayerScore ps in playersScores){
		
			if(nPlayer == ps.netPlayer){
				ps.score += 1;
				Debug.Log("This is player address in score list: " + ps.netPlayer.ipAddress + " this is his current score " + ps.score);
				break;
			}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
