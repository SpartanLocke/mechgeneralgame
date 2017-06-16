using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour {

    // Use this for initialization


    public List<Text> currentActionTexts;
    public List<Image> currentActionImage;

	public Color RightMove;
	public Color RightDash;
	public Color RightShoot;
	public Color RightPulse;
	public Color RightGrenade;

	public Button ExecuteButton;
	public GameObject ExecuteDisabler;
    public GameObject ExecuteConfirmPanel;

    public CameraFlipper cm;

	public Button Dash;
	public Button Pulse;
	public Button Laser;
    public Button Grenade;
	public GameObject DashDisabler;
	public GameObject PulseDisabler;
	public GameObject LaserDisabler;
    public GameObject GrenadeDisabler;

	public GameObject VictoryScreen;
	public GameObject DefeatScreen;
	public GameObject DrawScreen;
    public GameObject DisconnectScreen;

	void Start () {

		VictoryScreen.SetActive (false);
		DrawScreen.SetActive (false);
		DefeatScreen.SetActive (false);
    /*
        firstActionText = firstAction.GetComponent<Text>();
        secondActionText = secondAction.GetComponent<Text>();
        thirdActionText = thirdAction.GetComponent<Text>();
        */
	}
	
	// Update is called once per frame
	void Update () {
        // Bad performance. Fix code in actrual front-end implmentation.
        clearActionQueueGraphics();
        Optional<SoldierManager> optionalCurrentSoldier = GameManager.gameManager.currentSelectedSoldier;
        if (optionalCurrentSoldier.HasValue)
        {
            SoldierManager currentSoldier = optionalCurrentSoldier.Value;
            //Replacing ugly code with better code.
            for(int i = 0; i < currentSoldier.queuedActions.Count; i++)
            {
                Action currentAction = currentSoldier.queuedActions[i];
                currentActionTexts[i].text = currentAction.text;
                currentActionImage[i].enabled = true;
                currentActionImage[i].sprite = currentAction.sprite;
                currentActionImage[i].color = currentAction.UIPanelColor;

                if (currentAction.direction != Vector2.zero)
                {
                    int offset = (GameManager.gameManager.getFaction() == 1 ? 180 : 0);
                    int sign = - Math.Sign(currentAction.direction.x + 0.001f);
                    currentActionImage[i].gameObject.transform.localEulerAngles = new Vector3(0, 0, sign * ( offset + Vector2.Angle(Vector2.up, currentAction.direction)));
                }else
                {
                    currentActionImage[i].gameObject.transform.localEulerAngles = Vector3.zero;
                }
            }

        } else
        {
        }
		//Code to disable button based on limited actions
        if (GameManager.gameManager.currentSelectedSoldier.HasValue) {
		    if (GameManager.gameManager.currentSelectedSoldier.Value.currentLimitedActionCount < GameManager.gameManager.currentSelectedSoldier.Value.maxLimitedAction) {

			    Dash.interactable=true;
			    DashDisabler.SetActive(false);
			    Laser.interactable=true;
			    LaserDisabler.SetActive(false);
			    Pulse.interactable=true;
			    PulseDisabler.SetActive(false);
                Grenade.interactable = true;
                GrenadeDisabler.SetActive(false);
		    } 

		    else {
			    Dash.interactable=false;
			    DashDisabler.SetActive(true);
			    Laser.interactable=false;
			    LaserDisabler.SetActive(true);
			    Pulse.interactable=false;
			    PulseDisabler.SetActive(true);
                Grenade.interactable = false;
                GrenadeDisabler.SetActive(true);

            }
        }
        checkEndGame();
    }

	
	void clearActionQueueGraphics()
    {
        foreach (Text t in currentActionTexts)
        {
            t.text = "";
        }
        foreach (Image img in currentActionImage)
        {
            img.enabled = false;
        }
    }

    private void queueMove(Vector2 direction, String actionName)
    {
        queueAction(Action.move(new Vector2(direction.x, direction.y), actionName));

    }

    private void queueDash(Vector2 direction, String actionName)
    {
        queueAction(Action.move(new Vector2(direction.x, direction.y), actionName));
    }

    private void queueAction(Action action)
    {
        if (!GameManager.gameManager.isInOrderPhase)
        {
            return;
        }

        Optional<SoldierManager> soldier = GameManager.gameManager.currentSelectedSoldier;
        if (soldier.HasValue)
        {

            SoldierManager soldierManager = soldier.Value.GetComponent<SoldierManager>();
            if (soldierManager.queuedActions.Count < GameManager.maxActions)
            {
                if (!action.isLimitedAction || soldierManager.currentLimitedActionCount < soldierManager.maxLimitedAction)
                {
                    soldierManager.queuedActions.Add(action);
                    if (GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count == GameManager.maxActions)
                    {
                        GameManager.gameManager.board.ResetBoardColors();
                    }

                    if (action.isLimitedAction)
                    {
                        soldierManager.currentLimitedActionCount += 1;
                    }
                }
            }
        }
    }

	//Dash Ques
	public void QueDashUp(){
        string text = GameManager.gameManager.getFaction() == 0 ? "Dash Up" : "Dash Down";
        queueDash (new Vector2 (0, 2), text);
        Debug.LogError ("b4 checkign to rque move up");
//		if (GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count < GameManager.maxActions) {
//			Debug.LogError("action count less than max action, requeing move");
//			GameObject.FindWithTag ("Plane").GetComponent<Board>().MoveHighLight();
//		}
	}

	public void QueDashDown(){
        string text = GameManager.gameManager.getFaction() == 0 ? "Dash Down": "Dash Up";
        queueDash (new Vector2 (0, -2), text);
	}

	public void QueDashRight(){
        string text = GameManager.gameManager.getFaction() == 0 ? "Dash Right" : "Dash Left";
        queueDash (new Vector2 (2, 0), text);
    }

	public void QueDashLeft(){
        string text = GameManager.gameManager.getFaction() == 0 ? "Dash Left" : "Dash Right";
        queueDash (new Vector2 (-2, 0), text);
    }


    public void QueUp()
    {
        string text = GameManager.gameManager.getFaction() == 0 ? "Move Up" : "Move Down";
        queueMove(new Vector2(0, 1), text);
        GameManager.gameManager.board.MoveHighLight ();

    }

    public void QueDown()
    {
        string text = GameManager.gameManager.getFaction() == 0 ? "Move Down" : "Move Up";
        queueMove(new Vector2(0, -1), text);

    }

    public void QueRight()
    {
        string text = GameManager.gameManager.getFaction() == 0 ? "Move Right" : "Move Left";
        queueMove(new Vector2(1, 0), text);
    }

    public void QueLeft()
    {
        string text = GameManager.gameManager.getFaction() == 0 ? "Move Left" : "Move Right";
        queueMove(new Vector2(-1, 0), text);
    }

    public void QueShoot(Vector2 vec)
    {
        queueAction(Action.shoot(vec));
    }

    public void QueGrenade(Vector2 vec)
    {
        queueAction(Action.grenade(vec));
    }

    public void QueKnife()
    {
        queueAction(Action.knife());
    }

    public void QueReload()
    {
        queueAction(Action.reload());
    }
    public void Undo()
    {
        Optional<SoldierManager> soldier = GameManager.gameManager.currentSelectedSoldier;
        if (soldier.HasValue)
        {
            SoldierManager soldierManager = soldier.Value;
            int numActionsQueued = soldierManager.queuedActions.Count;
            if (numActionsQueued > 0)
            {
                Action removed = soldierManager.queuedActions[numActionsQueued - 1];
                soldierManager.queuedActions.RemoveAt(numActionsQueued - 1);
                if (removed.isLimitedAction)
                {
                    soldierManager.currentLimitedActionCount -= 1;
                }
            }
        }
		if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
			GameManager.gameManager.board.MoveHighLight();
        clearActionQueueGraphics();

    }

    public void executeClick()
    {
        int emptyActions = 0;
        foreach (SoldierManager s in GameManager.gameManager.soldiers)
        {
            if (s.faction == (GameManager.gameManager.getFaction()))
            {
                if (s.queuedActions.Count != GameManager.maxActions)
                {
                    emptyActions += 1;
                }
            }
        }
        
        if (emptyActions != 0)
        {
            ExecuteConfirmPanel.SetActive(true);
        } else
        {
            nextTurn();
        }
    }

    public void nextTurn()
    {
		Dash.interactable=true;
		DashDisabler.SetActive(false);
		Laser.interactable=true;
		LaserDisabler.SetActive(false);
		Pulse.interactable=true;
		PulseDisabler.SetActive(false);
        cm.FlipCamera();
		GameManager.gameManager.endTurn();
    }

	public void Menu(){
		Application.LoadLevel ("Start");
	}

	public void QuitApplication(){
		Application.Quit ();
	}

	public void CheckExecuteButton(){
		if (GameManager.gameManager.isNetworkPlay) {
			if (GameManager.gameManager.hasSentOrders==true){
				if (GameManager.gameManager.hasReceivedOrders==false) {
				ExecuteDisabler.SetActive(true);
				ExecuteButton.GetComponent<Button>().interactable =  false;
				ExecuteButton.GetComponentInChildren<Text>().text = "WAITING";
	
			}

				if (GameManager.gameManager.hasReceivedOrders==true) {
					ExecuteDisabler.SetActive(true);
					ExecuteButton.GetComponent<Button>().interactable =  false;
					ExecuteButton.GetComponentInChildren<Text>().text = "RESOLVING";
					
			}
			}
			else{
				ExecuteDisabler.SetActive(false);
				ExecuteButton.GetComponent<Button>().interactable =  true;
				ExecuteButton.GetComponentInChildren<Text>().text = "EXECUTE";
				
			}

		}

	}

    void checkEndGame()
    {
        int yourSoldiersAlive = 0;
        int enemySoldiersAlive = 0;
        foreach (SoldierManager s in GameManager.gameManager.soldiers)
        {
            if (s.health > 0)
            {
                if (s.faction == (GameManager.gameManager.getFaction()))
                {
                    yourSoldiersAlive += 1;
                }else
                {
                    enemySoldiersAlive += 1;
                }
            }
        }

        if (yourSoldiersAlive == 0 && GameManager.gameManager.isInOrderPhase)
        {
            if (enemySoldiersAlive == 0)
            {
                Draw();
            } else
            {
                Defeat();
            }
        } else if (enemySoldiersAlive == 0 && GameManager.gameManager.isInOrderPhase)
        {
            Victory();
        }
    }

	public void Victory(){
		VictoryScreen.SetActive (true);
	}
	
	public void Defeat(){
		DefeatScreen.SetActive (true);
	}
	public void Draw(){
		DrawScreen.SetActive (true);
	}

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        DisconnectScreen.SetActive(true);
    }


}
