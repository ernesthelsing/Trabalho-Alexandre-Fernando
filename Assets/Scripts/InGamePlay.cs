using UnityEngine;
using System.Collections;

public class InGamePlay : MonoBehaviour {

	// PUBLIC
	// Points to itself
	public static InGamePlay Script;
	public float TimeCounter = 0.0f;
	
	// PRIVATE

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {

		Script = this;
	}

	// Use this for initialization
	void Start () {

		// DEBUG
		Debug.Log("Spawning player.");
		// FIXME:
		NetworkGame.Script.InitializeSpawnPoint();
		NetworkGame.Script.SpawnPlayers();

		// I'm not controlling this player
		if(!networkView.isMine) {

			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Network.isServer) {

			// Updates the timer
			TimeCounter += Time.deltaTime;
		}
	
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

	public float GetTimeCounter() {

		return TimeCounter;
	}
}
