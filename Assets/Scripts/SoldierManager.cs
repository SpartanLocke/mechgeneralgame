using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SoldierManager : MonoBehaviour {

	public bool isSelected;
    public List<Action> queuedActions = new List<Action>();
    public Vector2 position;
    public GameObject bulletPrefab0;
	public GameObject bulletPrefab1;
	public GameObject RobotGlowingHead;
	public Color Faction0HeadEmissionColor;
	public Color Faction1HeadEmissionColor;
	public GameObject PulsePrefabFaction1;
	public GameObject PulsePrefabFaction2;

    public int health = 3;
    public int ammo = 2;

	public int previousGrenadeCount;
	public int previousDamageMod;
	public int previousMaxLimitedAction;// keep track of when power up picked up


    public int maxLimitedAction = 1;
    public int currentLimitedActionCount;
    public int damageMod = 0;
    public int grenadeCount = 0;
	public RenderTexture FaceCamTexture;
	public RenderTexture BlankTexture;

    public int faction; // to be implmented

	public Color Faction0Camera;
	public Color Faction1Camera;

	public Vector3 OriginalPosition;

	// Use this for initialization
	void Awake () {
        isSelected = false;
		RobotGlowingHead.SetActive (false);
	//GetComponent<Renderer> ().material.SetColor=( Color.red;
    }
	
    void Start()
    {

        GameManager.gameManager.addSoldier(this);
		if (faction == 0) {
			RobotGlowingHead.GetComponent<ParticleSystem>().startColor=Faction0HeadEmissionColor;
			gameObject.GetComponentInChildren<Camera>().backgroundColor = Faction0Camera;
		}

		if (faction == 1) {
			RobotGlowingHead.GetComponent<ParticleSystem>().startColor=Faction1HeadEmissionColor;
			gameObject.GetComponentInChildren<Camera>().backgroundColor = Faction1Camera;

		}

		previousDamageMod = damageMod;
		previousMaxLimitedAction = maxLimitedAction;
		previousGrenadeCount = grenadeCount;
    }

    // Update is called once per frame
    void Update () {

		// power up stuff, needs to be coded again to account for multiple power ups etc)
		if (previousDamageMod!=damageMod) {
			previousDamageMod= damageMod;
			GetComponentInChildren<SoldierCircularFill>().DamagePickedUp();
		}
		if (previousMaxLimitedAction != maxLimitedAction) {
			previousMaxLimitedAction=maxLimitedAction;
			GetComponentInChildren<SoldierCircularFill>().ActionPickedUp();
		}
		if (previousGrenadeCount < grenadeCount) {
			previousGrenadeCount=grenadeCount;
			GetComponentInChildren<SoldierCircularFill>().GrenadePickedUp();
		} else if (previousGrenadeCount > grenadeCount)
        {
            previousGrenadeCount = grenadeCount;
            GetComponentInChildren<SoldierCircularFill>().GrenadeUsed();

        }


        //
        if (isSelected != true) {
			gameObject.GetComponentInChildren<Camera>().enabled=false;
		}
		else
			gameObject.GetComponentInChildren<Camera>().enabled=true;
	}


	public void OnMouseDown(){

        if (GameManager.gameManager.board.board[(int)position.x][(int)position.y].isHighlighted())
        {
            Debug.Log("hi");

            //This line not actrually necessary <magic>
            // GameManager.gameManager.board.board[(int)position.x][(int)position.y].OnMouseDown();
            //The aboe line not actrually necessary </magic>

            //magic explained: what actrually happens is that the click always goes through. what we used to do is reset the board highlight too soon,
            //because the soldier gets clicked before the cell, which means the action fails to cue. 
            return;
        }
        if (GameManager.gameManager.currentActiveFaction != this.faction)
        {
            return;
        }
		GameManager.gameManager.selectSoldier(this);

	}

    private IEnumerator moveHighlightAfterOneFrame()
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.board.MoveHighLight();
    }


    // no longer obsolete! good for front-end/back-end seperation.
    public void FlipColor(){
        if (isSelected == true)
        {
            RobotGlowingHead.SetActive(false);
            isSelected = false;
            gameObject.GetComponentInChildren<Camera>().targetTexture = BlankTexture;
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
            return;
        }
        else
        {
            RobotGlowingHead.SetActive(true);
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
            isSelected = true;
            gameObject.GetComponentInChildren<Camera>().targetTexture = FaceCamTexture;
            if (this.queuedActions.Count < 3)
                StartCoroutine(moveHighlightAfterOneFrame());
            return;
        }
    }

    public void fillWithReload()
    {
        while (queuedActions.Count < GameManager.maxActions)
        {
            queuedActions.Add(new Reload(this));
        }
    }


    public Vector2 getPredictedPosition(int numLookAhead = GameManager.maxActions)
    {
        Vector2 predictedPosition = position;
        foreach(Action action in queuedActions)
        {
            if (numLookAhead == 0)
            {
                return predictedPosition;
            }
            predictedPosition = action.predictPosition(predictedPosition);
            numLookAhead -= 1;
        }
        return predictedPosition;
    }


    [PunRPC]
    void remoteQueueAction(string actionString)
    {
        Action action = Action.deserialize(actionString, this);
        queuedActions.Add(action);
        Debug.Log("soldier " + gameObject.GetPhotonView().viewID + "received orders");
    }


    public void sendActions()
    {
        PhotonView photonView = PhotonView.Get(this);
        foreach (Action action in queuedActions) {
            photonView.RPC("remoteQueueAction", PhotonTargets.Others, action.serialize());
        }
    }
	
}
