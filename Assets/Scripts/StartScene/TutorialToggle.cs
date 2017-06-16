using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialToggle : MonoBehaviour {

    public Toggle tutorialToggle;

	// Use this for initialization
	void Start () {
        tutorialToggle.isOn = (PlayerPrefs.GetInt("tutorial", 2) >= 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle(bool enabled)
    {
        PlayerPrefs.SetInt("tutorial", enabled ? 1 : 0);
    }
}
