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
		
		if(collided.gameObject.name.Equals ("First Person Controller") )
		{
			Vector3 start = collided.gameObject.GetComponent<PlayerControl>().StartingPosition;
			collided.gameObject.transform.position = start;
			// FIXME: there no more lives. The player should restart the level
			//collided.gameObject.GetComponent<PlayerControl>().lives -= 1;
			Debug.Log("Hit Player died");
		}
		
		if(collided.gameObject.name.Equals ("Cannonball(Clone)") )
		{
			GameObject.Destroy(collided.gameObject);
			GameObject.Destroy(transform.gameObject);
			//Debug.Log("Hit another cannonball died");
		}
		
		if(collided.gameObject.tag.Equals("Platform") )
			startCount = true;
			//Debug.Log("Hit a platform");
		
	}
	
	
}
