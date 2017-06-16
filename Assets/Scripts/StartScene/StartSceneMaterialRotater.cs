using UnityEngine;
using System.Collections;

public class StartSceneMaterialRotater : MonoBehaviour {


	public Material Material1;
	public Material Material2;
	public Material Material3;
	public Material Material4;
	public Material Material5;

	public bool Mat1On=false;
	public bool Mat2On=false;
	public bool Mat3On=false;
	public bool Mat4On=false;
	public bool Mat5On=false;



	// Use this for initialization
	void Start () {
		gameObject.GetComponent<SkinnedMeshRenderer>().material= Material1;
		Mat1On = true;
		InvokeRepeating ("MaterialCycler", 2, 2);
	}

	void MaterialCycler(){
		if (Mat1On==true) {
			gameObject.GetComponent<SkinnedMeshRenderer>().material = Material2;
			Mat1On=false;
			Mat2On=true;
			return;
		}

		if (Mat2On==true) {
			gameObject.GetComponent<SkinnedMeshRenderer>().material = Material3;
			Mat2On=false;
			Mat3On=true;
			return;

		}

		if (Mat3On==true) {
			gameObject.GetComponent<SkinnedMeshRenderer>().material = Material4;
			Mat3On=false;
			Mat4On=true;
			return;
		}

		if (Mat4On==true) {
			gameObject.GetComponent<SkinnedMeshRenderer>().material = Material5;
			Mat4On=false;
			Mat5On=true;
			return;
		}

		if (Mat5On==true) {
			gameObject.GetComponent<SkinnedMeshRenderer>().material = Material1;
			Mat5On=false;
			Mat1On=true;
			return;
		}
	}
}
