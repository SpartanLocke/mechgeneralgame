using UnityEngine;
using System.Collections;

public class SoldierGhost : MonoBehaviour {

    public int ghostSteps = 3;
    public SoldierManager soldier;
	// Use this for initialization
	void Start () {
		toggleChildActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (GameManager.gameManager.currentActiveFaction == gameObject.GetComponentInParent<SoldierManager> ().faction && GameManager.gameManager.isInOrderPhase && soldier.position != soldier.getPredictedPosition(ghostSteps)) {
				gameObject.transform.position = GameManager.gameManager.getWorldPosition (soldier.getPredictedPosition (ghostSteps));
				toggleChildActive (true);
		}
		else {
				toggleChildActive (false);
			}
		
	}

    void OnMouseDown()
    {
        soldier.OnMouseDown();
    }

    void toggleChildActive(bool active)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}
