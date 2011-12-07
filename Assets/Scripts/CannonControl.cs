using UnityEngine;
using System.Collections;

// make it follow player or make it move horizontally, have to fix the model so ill let it be static for now
public class CannonControl : MonoBehaviour {
	
	public GameObject bullet;
	public float speed = 10.0f;
	
	//public GameObject player;
	
	public AudioClip blast;
	public int minShootTime = 5;
	public int maxShootTime = 15;
	
	private GameObject bulletClone;
	
	private float shootTime;
	private float timeTaken;
	
	private Vector3 offsetVec;

	// Use this for initialization
	void Start () {
		
		offsetVec = new Vector3(0,2,-2);
		shootTime = Random.Range(minShootTime,maxShootTime);
		timeTaken = 0.0f;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//if(player.transform.position.x > 18.0f)
			//transform.LookAt(player.transform);
		//if(!LevelControl.gameStarted) return;
		
		if(timeTaken > shootTime)
		{
			animation.Play("Take 001");
			//yield return new WaitForSeconds(animation.GetClip("Take 001").length);
			StartCoroutine(Shoot(animation.GetClip("Take 001").length));
			timeTaken = 0.0f;
			shootTime = Random.Range(minShootTime, maxShootTime);
		}
		
		StopCoroutine("Shoot");
		timeTaken += Time.deltaTime;
	
	}
	
	IEnumerator Shoot(float waitTime){
		
		yield return new WaitForSeconds(waitTime);
		
		audio.PlayOneShot(blast);
		bulletClone = Instantiate(bullet,transform.position + offsetVec,transform.rotation) as GameObject;
		bulletClone.rigidbody.velocity = transform.right * speed;
			
	}
}
