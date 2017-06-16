using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour {


	RectTransform StartPanel;
    public static bool isNetworkPlay = false;
    public static bool isPrivateMatch = false;
    public static int localPlayerFaction;
    public static bool created = false;

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void QuickPlay(){
		isNetworkPlay = false;
        isPrivateMatch = false;
        Application.LoadLevel ("Main");

	}

    public void NetworkPlay()
    {
        isNetworkPlay = true;
        isPrivateMatch = false;
        Application.LoadLevel("Connect");
    }

    public void PrivatePlay()
    {
        isNetworkPlay = true;
        isPrivateMatch = true;
        Application.LoadLevel("Connect");
    }

	public void CollapseStartPanel(){
	}

	public void QuitGame(){
		Application.Quit ();
	}
}
