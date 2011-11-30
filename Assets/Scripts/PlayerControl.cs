using UnityEngine;
using System.Collections;

public class PlayerControl: MonoBehaviour {
	
	private Vector3 startingPosition;
	
	public AudioClip jump;
	
	public int lives;

	// Use this for initialization
	void Start () {
		
		startingPosition = transform.position;
		lives = 3;
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown("space"))
		{
			if(LevelControl.gameStarted)
				audio.PlayOneShot(jump);
			
		}
	
	}
	
	
	public void SetMotionStatus(bool status){
		
		GetComponent<FPSInputController>().enabled = status;
		GetComponent<MouseLook>().enabled = status;
		GetComponent<CharacterMotor>().canControl = status;
		//GetComponent<CharacterController>().enabled = status;
		Camera.mainCamera.GetComponent<MouseLook>().enabled = status;
		
	}
	
		
	public Vector3 StartingPosition{
	
		get{ return startingPosition; }
		
	}
	
	
}
