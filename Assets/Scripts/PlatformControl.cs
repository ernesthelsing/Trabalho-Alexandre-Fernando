using UnityEngine;
using System.Collections;

public class PlatformControl : MonoBehaviour {
	
	private float timeTaken;
	private float randomTime;
	
	private bool goingDown;

	// Use this for initialization
	void Start () {
	
		timeTaken = 0.0f;
		goingDown = false;
		randomTime = Random.Range(5,10);

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(!LevelControl.gameStarted) return;
		
		
		if(timeTaken > randomTime || this.transform.position.y > 36.0f)
		{
			goingDown = !goingDown;
			timeTaken = 0.0f;
			randomTime = Random.Range(5,10);
		}
				
		if(goingDown)
		{
			transform.Translate(new Vector3(0,-1.5f * Time.deltaTime,0));
		}else
		{
			transform.Translate(new Vector3(0,1.5f * Time.deltaTime,0));
		}
		
		timeTaken += Time.deltaTime;
	}
}
