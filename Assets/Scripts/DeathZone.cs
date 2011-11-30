using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){
		
		if(other != null)
		{
			if(other.gameObject.name.Equals("First Person Controller"))
			{
				Vector3 start = other.gameObject.GetComponent<PlayerControl>().StartingPosition;
				other.gameObject.transform.position = start;
				other.gameObject.GetComponent<PlayerControl>().lives -= 1;
				Debug.Log("Player died");
			}
			
			if(other.gameObject.name.Equals("Cannonball(Clone)"))
				GameObject.Destroy(other.gameObject);
		
		}
		
	}
	
	void OnDrawGizmos(){
		
		Gizmos.DrawIcon(transform.position,"Delete.ico");
		
	}
}
