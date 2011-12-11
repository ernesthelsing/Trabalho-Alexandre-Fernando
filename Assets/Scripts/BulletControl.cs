using UnityEngine;
using System.Collections;

public class BulletControl : MonoBehaviour {
	
	//public GameObject parentCannon;
	//private float traveledDistance = 0;
	private float timeToDissapear = 2.0f;
	private float timeTaken = 0;
	//private Vector3 lastPosition;
	private bool startCount = false;

	// Use this for initialization
	void Start () {
	
		//lastPosition = transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(startCount)
		{
			
			if(timeTaken > timeToDissapear)
				GameObject.Destroy(transform.gameObject);
			
			
			timeTaken += Time.deltaTime;
		}
	
	}
	
	void OnCollisionEnter (Collision collided){
		
		// Cannonball VS player
		if(collided.gameObject.tag.Equals("Player") )
		{
			Vector3 start = collided.gameObject.GetComponent<PlayerControl>().StartingPosition;
			collided.gameObject.transform.position = start;
			
			// DEBUG
			Debug.Log("Hit Player died");
		}
		
		// Cannonball VS Cannonball (!?!?)
		if(collided.gameObject.tag.Equals("Cannonball") )
		{
			GameObject.Destroy(collided.gameObject);
			GameObject.Destroy(transform.gameObject);
			//Debug.Log("Hit another cannonball died");
		}
		
		// Cannonball VS Platform
		if(collided.gameObject.tag.Equals("Platform") ) {

			startCount = true;
			//Debug.Log("Hit a platform");
		}
		
	}
	
	
}
