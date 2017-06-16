using UnityEngine;
using System.Collections;

public class CameraFlipper : MonoBehaviour {

	public Quaternion StandardOrientation;
	public Quaternion FlippedOrientation;
	public float orthoZoomSpeed = 0.2f;        // The rate of change of the orthographic size in orthographic mode.

    public static bool flipped = false;
	
	void Update()
	{
		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			
			// If the camera is orthographic...
			if (GetComponent<Camera>().orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				
				// Make sure the orthographic size never drops below zero.
				GetComponent<Camera>().orthographicSize = Mathf.Clamp (GetComponent<Camera>().orthographicSize, 52f,70f);
			}

		}
	}




void Start () {
	
	StandardOrientation = gameObject.transform.rotation;
	FlippedOrientation = StandardOrientation;
    flipped = false;
    if (ApplicationManager.isNetworkPlay && ApplicationManager.localPlayerFaction == 1)
    {
        transform.Rotate(0, 0, 180);
        flipped = true;
    }
}


public void FlipCamera(){
	if (!GameManager.gameManager.isNetworkPlay) {
		StartCoroutine (CameraFlipperHelper ());
	}
}

//edit to flip camera once if in network and second faction

	IEnumerator CameraFlipperHelper(){
		while(GameManager.gameManager.isInExecutionPhase==true){
			Debug.Log ("calling wait");
			yield return new WaitForSeconds(0.5f);
		}
			transform.Rotate (0, 0, 180);
        flipped = !flipped;
	}
}

