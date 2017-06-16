using UnityEngine;
using System.Collections;

public class DisconnectOnMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.Disconnect();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
