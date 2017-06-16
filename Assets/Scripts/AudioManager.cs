using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {


    public static bool created = false;


    void Awake () {
        if (!created)
        {
            DontDestroyOnLoad(transform.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
