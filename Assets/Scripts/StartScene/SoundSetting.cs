using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour {

    public Toggle soundToggle;


	// Use this for initialization
	void Start () {
        soundToggle.isOn = !AudioListener.pause;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle(bool enabled)
    {
        AudioListener.pause = !enabled;
    }
}
