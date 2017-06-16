using UnityEngine;
using System.Collections;

public class HideGrenadeWhenNotAvailable : MonoBehaviour {

    public GameObject grenadeIcon;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (GameManager.gameManager.currentSelectedSoldier.HasValue && GameManager.gameManager.currentSelectedSoldier.Value.grenadeCount > 0)
        {
            grenadeIcon.SetActive(true);
        } else
        {
            grenadeIcon.SetActive(false);
        }
    }
}
