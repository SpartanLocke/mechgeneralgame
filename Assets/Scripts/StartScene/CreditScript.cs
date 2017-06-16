using UnityEngine;
using System.Collections;

public class CreditScript : MonoBehaviour {

	public bool CreditsOn;
    bool clicked = false;

	void Start () {
        if (!clicked) { 
            gameObject.SetActive (false);
		    CreditsOn = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
	}

	public void FlipCredits(){
        clicked = true;
		if (CreditsOn == false) {
			gameObject.SetActive (true);
			CreditsOn = true;
			return;
		}
		if (CreditsOn == true){
			gameObject.SetActive (false);
			CreditsOn = false;
			return;
		}

	}
}
