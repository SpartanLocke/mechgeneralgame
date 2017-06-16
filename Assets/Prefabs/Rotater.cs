using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {
	public int RotateSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<Transform> ().Rotate (0, 0, RotateSpeed);
	}
}
