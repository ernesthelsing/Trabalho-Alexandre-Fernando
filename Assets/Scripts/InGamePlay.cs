using UnityEngine;
using System.Collections;

public class InGamePlay : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// DEBUG
		Debug.Log("Spawning player.");
		NetworkGame.Script.SpawnPlayers();

		// I'm not controlling this player
		if(!networkView.isMine) {

			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
