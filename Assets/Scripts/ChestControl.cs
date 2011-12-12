using UnityEngine;
using System.Collections;

public class ChestControl : MonoBehaviour {

	// PUBLIC
	// Points to itself
	public static ChestControl Script;
	public AudioClip chestReachedSound = null;

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
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	
	/*
	 * @brief		When someone hits the collider of this chest, means an extra point
	 * @param		other		Collider that triggered this
	 * @return	void
	 */
	void OnTriggerEnter(Collider other){
		
		// DEBUG
		Debug.Log("[ChestControl] Triggered by " + other.gameObject.name);
		
		if(other.gameObject.tag.Equals("Player")) {

			PlayerReachedChest(other.gameObject);

			// Player reached the objective!
			LevelControl.Script.PlayerReachedEndArea(other.gameObject);
		}
	}

	/*
	 * @brief		Play a sound when the player hits the chest
	 * @param		goPlayer	GameObject for the player
	 * @return	void
	 */
	void PlayerReachedChest(GameObject goPlayer) {

		// Play a sound
		if(chestReachedSound != null) {

			audio.PlayOneShot(chestReachedSound);
		}
		else {

			// DEBUG
			Debug.LogError("No sound defined.");
		}

	}
}
