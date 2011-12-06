using UnityEngine;
using System.Collections;

public class NetPlatformControl : MonoBehaviour {

	/*
	 * @brief	New way to control the platforms over the network, using Christian's tips.
	 * Former way: the platform start with a direction and a random timer between 5 and 10 seconds. When the
	 * timer is over, the platform switches direction
	 *
	 * NEW WAY:
	 * Using sine function, we can always position the platform over time between the range [-1,1]
	 *
	 * @author		Alexandre Ramos Coelho
	 * @revision	27-11-2011
	 */

	// PUBLIC
	public float aAmplitude = 1.0f;
	public float bPeriod = 1.0f;
	public float cPhaseShift = 0.0f;
	public float dVerticalShift = 0.0f;
	
	// PRIVATE
	private Vector3 v3Movement = Vector3.zero;
	private Vector3 v3StartPos = Vector3.zero;
	private float timeStarted;


	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	// When the script is instantiated
	void Awake() {

		// Keeps the initial height
		dVerticalShift = transform.position.y;
	}

	// Use this for initialization
	void Start () {
	
		timeStarted = Time.time;
	}
	
	// Using fixed update, so the player on the platform doesn't jitter 
	void FixedUpdate () {

		v3Movement.y = MovePlatform(LevelControl.Script.GetTimeCounter());
		v3Movement.x = transform.position.x;
		v3Movement.z = transform.position.z;

		// Moves the platform
		transform.position = v3Movement;
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * SCRIPT STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	float MovePlatform(float fX) {

		float fY = aAmplitude * Mathf.Sin(bPeriod * fX) + dVerticalShift;

		return fY;
	}
}
