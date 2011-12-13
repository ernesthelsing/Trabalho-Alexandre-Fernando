using UnityEngine;
using System.Collections;

// make it follow player or make it move horizontally, have to fix the model so ill let it be static for now
public class NetCannonControl : MonoBehaviour {
	
	// PUBLIC
	public GameObject bullet;
	public float speed = 12.0f;
	
	public AudioClip blast;
	public int minShootTime = 3;
	public int maxShootTime = 6;
	public int myOwnIndex = -1;
	
	// PRIVATE
	private GameObject bulletClone;
	private float shootTime;
	private float timeTaken;
	private Vector3 offsetVec;
	private ParticleEmitter pEmitter = null;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {

 		//pEmitter = GetComponent<ParticleEmitter>();
 		pEmitter = gameObject.GetComponentInChildren<ParticleEmitter>();

		if(pEmitter == null) {

			// DEBUG
			Debug.Log("[NetCannonControl] ParticleEmitter not found!");
		}
		else {

			// Disables the particle emission
			pEmitter.emit = false; 
		}
	}
	// Use this for initialization
	void Start () {
		
		offsetVec = new Vector3(0,2,-2);

		// Only the server cannons controls something
		if(Network.isServer) {

			shootTime = Random.Range(minShootTime,maxShootTime);
			timeTaken = 0.0f;
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		if(myOwnIndex < 0) {

			// DEBUG
			Debug.LogError("Index for " + this + "not set yet");
			return;
		}

		if(Network.isServer) {
		
			if(timeTaken > shootTime) {

				networkView.RPC("DoTheShootStuff", RPCMode.All, myOwnIndex);
			}

			//networkView.RPC("StopTheShootStuff", RPCMode.All, myOwnIndex);
			StopTheShootStuff(myOwnIndex);
		}
	
	}
	
	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	IEnumerator Shoot(float waitTime){
		
			yield return new WaitForSeconds(waitTime);

			// NEW FEATURE!
			if(pEmitter != null) {
			
				pEmitter.Emit();
			}

			audio.PlayOneShot(blast);
//			bulletClone = Instantiate(bullet,transform.position + offsetVec,transform.rotation) as GameObject;
			bulletClone = Network.Instantiate(bullet, transform.position + offsetVec, transform.rotation, 1) as GameObject;
			bulletClone.rigidbody.velocity = transform.right * speed;
	}

	[RPC]
	void DoTheShootStuff(int netCannonIdx) {

		if(netCannonIdx == myOwnIndex) {

			animation.Play("Take 001");
			//yield return new WaitForSeconds(animation.GetClip("Take 001").length);

			// Server only
			if(Network.isServer) {

				StartCoroutine(Shoot(animation.GetClip("Take 001").length));
				timeTaken = 0.0f;
				shootTime = Random.Range(minShootTime, maxShootTime);
			}
		}
	}

//	[RPC]
	void StopTheShootStuff(int netCannonIdx) {

		if(netCannonIdx == myOwnIndex) {

			StopCoroutine("Shoot");
			timeTaken += Time.deltaTime;
		}
	}

	/*
	 * @brief		Set the unique identifier (an index number) for this cannon. This is send by network, all
	 *					the same cannons in all clients will receive the same index
	 * @param		idx	An integer with the position of this cannon in the cannons array created in the server
	 * @return	void
	 */
	public void SetMyIndex(int idx) {

		myOwnIndex = idx;
	}	

}
