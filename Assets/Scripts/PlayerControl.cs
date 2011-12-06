using UnityEngine;
using System.Collections;

public class PlayerControl: MonoBehaviour {
	
	// PRIVATE
	private Vector3 startingPosition;
	
	private GameObject goHit;
	
	// PUBLIC
	// Points to itself
	public static PlayerControl Script;
	public AudioClip jump;

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
		
		startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown("space"))
		{
			if(LevelControl.gameStarted)
				audio.PlayOneShot(jump);
			
		}
		
		//RaycastingHighlight();
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Disable and enable the player movement when the game has not started yet
	 * @param		status	A boolean, true allow the player to move
	 * @return	void
	 */
	public void SetMotionStatus(bool status){
		
		GetComponent<FPSInputController>().enabled = status;
		GetComponent<MouseLook>().enabled = status;
		GetComponent<CharacterMotor>().canControl = status;
		//GetComponent<CharacterController>().enabled = status;
		Camera.mainCamera.GetComponent<MouseLook>().enabled = status;
		
	}
	
		
	/*
	 * @brief		The player starting position
	 * @return	A vector3 with the player's starting position
	 */
	public Vector3 StartingPosition{
	
		get{ return startingPosition; }
		
	}

	/*
	 * @brief		Move this player back to the start (spawn) point
	 * @param		void
	 * @return	void
	 */
	public void MoveBackToStart() {


		// Moving back
		Vector3 start = StartingPosition;
		transform.position = start; 

		// DEBUG
		Debug.Log("Moving back..." + transform.position + "/"+ startingPosition);
	}
	
	//Fernando//
	public void RaycastingHighlight(){
		
		RaycastHit hit;
		
		if(Physics.Raycast(transform.position + new Vector3(0,5,0),transform.forward,out hit,15.0f)){
			hit.collider.gameObject.renderer.material.color = Color.green;
		}
		
	}
	//End Fernando//
}
