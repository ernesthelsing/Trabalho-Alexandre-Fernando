using UnityEngine;
using System.Collections;

public class PlatformCrumble : MonoBehaviour {

	// PUBLIC
	public float maxStayTime = 3.0f;
	public float strenght = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(strenght < 0.0f) {

			// DEBUG
			Debug.Log("Platform crumbling!!!");
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider hit) {

		if(hit.tag == "Player") {

			strenght -= Time.deltaTime;
		}
		
	}

	void OnTriggerStay(Collider hit) {

		if(hit.tag == "Player") {

			strenght -= Time.deltaTime;
		}
	}

	void OnTriggerExit(Collider hit) {

	}
}
