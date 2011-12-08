using UnityEngine;
using System.Collections;

public class NetworkCharacterControl : MonoBehaviour {

	private bool currentStatus;
	public Camera myCamera;

	// Use this for initialization
	void Start () {
		
		// Disables player's movement
		SetMotionStatus(false);

		if(networkView.isMine) {

			LevelControl.Script.SetPlayerGameObject(gameObject);
			SetMotionStatus(true);

			// DEBUG
			Debug.Log("[NetworkCharacterControl] Setting player to " + gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(networkView.isMine){

			if(!currentStatus) {


				if(gameObject.GetComponent<PlayerControl>().PlayerMotionStatus != false) {

					// Enables players movement
					SetMotionStatus(true);
				}
			}
		}
		else{

			//transform.position = posLocal;
			//transform.rotation = rotLocal;

		}//Camera.mainCamera.enabled = false;


	}

	/*void OnNetworkInstantiate(NetworkMessageInfo info){
		if(networkView.isMine){

		Camera.mainCamera.transform.position = transform.position;
		Camera.mainCamera.transform.rotation = transform.rotation;

		}

		}*/

	void OnSerializedNetworkView(BitStream stream, NetworkMessageInfo info){

		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;

		if(stream.isWriting){
			
			pos = transform.position;
		    rot = transform.rotation;
			
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			
		}else{
			
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			
			pos = transform.position;
		    rot = transform.rotation;
			//posLocal = pos;
			//rotLocal = rot;
		}
	}
	
	public void SetMotionStatus(bool status){
		
		GetComponent<CharacterController>().enabled = status;
		GetComponent<FPSInputController>().enabled = status;
		GetComponent<MouseLook>().enabled = status;
		//GetComponent<CharacterMotor>().canControl = status;
		GetComponent<CharacterMotor>().enabled = status;
		GetComponentInChildren<Camera>().enabled = status;
		GetComponentInChildren<AudioListener>().enabled = status;
		GetComponentInChildren<MouseLook>().enabled = status;
		//myCamera.enabled = status;
		currentStatus = status;
		
	}

	
}
