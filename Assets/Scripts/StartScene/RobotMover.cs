using UnityEngine;
using System.Collections;

public class RobotMover : MonoBehaviour {

	public Vector3 CurrentPosition;
	public Vector3 OriginalPosition;

	// Use this for initialization
	void Start () {
		OriginalPosition = gameObject.GetComponent<Transform> ().position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (CurrentPosition.z >35) {
			gameObject.GetComponent<Transform> ().position = OriginalPosition;
		}
		CurrentPosition = gameObject.GetComponent<Transform> ().position;
		CurrentPosition.x-=0.02f;
		//CurrentPosition.y += 0.2f;
		CurrentPosition.z += 0.02f;
		gameObject.GetComponent<Transform> ().position=CurrentPosition;
	}
}
