using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//borrowed from http://forum.unity3d.com/threads/singleton-monobehaviour-script.99971/
//Note that this class is probably not threadsafe. Be careful. Fix if you can. (shouldn't really be a problem.)
//Uses the singleton Pattern. Contains global pointers.

public sealed class GameManager : MonoBehaviour {
    public List<SoldierManager> soldiers = new List<SoldierManager>();
    public Optional<SoldierManager> currentSelectedSoldier;

    //Made public. Actions query this for concurrent actions.
    public List<Action> currentRunningActions = new List<Action>();

    //Each set of actions are executed together, anmiation happening at the same time.
    //Different sets are executed at different times.
    //Each turn populates this queue once.
    private Queue<List<Action>> currentTurnActionQueue = new Queue<List<Action>>();

    public bool isInOrderPhase;
    public bool isInExecutionPhase;

    //===============================================================
    //networking only
    public bool isNetworkPlay = false;
    public int playerFaction; //undefined for singleplayer sessions.
    public bool hasSentOrders = false;
    public bool hasReceivedOrders = false;

    //===============================================================

	public Board board;

    public const int maxActions = 3;
    public const int maxAmmo = 2;
    public const int maxHealth = 3; //TODO: get rid of all hard-coded "3" 's in other files.
    public int currentActiveFaction = 0;
    public List<string> priorityList;

    private static GameManager instance;
    static GameManager() { }
    private GameManager() { }

	public Color Player1Color;
	public Color Player2Color;
	public Color StandardColor;
    public static GameManager gameManager
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("container")).AddComponent<GameManager>();
            }
            return instance;
        }
    }

    public Vector3 getWorldPosition(Vector2 coordinate)
    {
        return new Vector3(coordinate.x * 10 - 35, coordinate.y * 10 - 45, -20);
    }

    //Starts tracking a soldier
    public void addSoldier(SoldierManager soldier)
    {
        soldiers.Add(soldier);
        board.board [(int)soldier.position.x] [(int) soldier.position.y].setSoldier (soldier);
    }

	public void addBoard(Board board)
	{
		this.board = board;
    }

    public void selectSoldier(SoldierManager soldier)
    {
        //Not all of these checks are necessary, this is added for precautions.
        if (soldier.faction != currentActiveFaction || !isInOrderPhase || hasSentOrders)
        {
            return;
        }
        if (currentSelectedSoldier.HasValue)
        {
            currentSelectedSoldier.Value.FlipColor();

        }
        soldier.FlipColor();
        currentSelectedSoldier = new Optional<SoldierManager>(soldier);
    }

    public void deselectSoldier()
    {
        if (currentSelectedSoldier.HasValue)
        {
            currentSelectedSoldier.Value.FlipColor();
        }
        currentSelectedSoldier = new Optional<SoldierManager>();
    }

    public void endTurn()
    {
	    deselectSoldier();
        board.ResetBoardColors();
        Debug.Log(currentActiveFaction);
        foreach (SoldierManager soldierManager in soldiers){
            if (soldierManager.queuedActions.Count < maxActions && soldierManager.health > 0 && soldierManager.faction == currentActiveFaction)
            {
                soldierManager.fillWithReload();
                Debug.Log("Not all actions queued. Filled empty actions with Reloads."); //Temperory implementation
            }
        }

        if (!isNetworkPlay) { 
            if (currentActiveFaction == 0)
            {
				GameObject.FindWithTag("PlayerMessager").GetComponent<PlayerMessageManager>().TextAnimationCaller("Player 2 turn", Player2Color);

				currentActiveFaction = 1;
				return;
            }

            isInOrderPhase = false;
            isInExecutionPhase = true;
        } 
		else //networking code below. fast implmentation.
        {
            if (hasSentOrders || isInExecutionPhase)
            {
                return;//prohibits sending orders multiple times
            }
            foreach (SoldierManager soldier in soldiers)
            {
                if (soldier.faction == playerFaction)
                {
                    soldier.sendActions();
                }
            }
            hasSentOrders = true;
			GameObject.FindWithTag ("PlayerMessager").GetComponent<PlayerMessageManager>().TextAnimationCaller("Orders Sent!", StandardColor);
            if (hasReceivedOrders)
            {
                isInOrderPhase = false;
                isInExecutionPhase = true;
            }
        }
    }

    void Start()
    {
        Debug.Log("Initializing GameManager Singleton. You should see this line only once.");
        isInOrderPhase = true;
        priorityList = new List<string>()
        {
            "dash1",
            "move2",
            "shoot3",
            "reload4",
            "knife3",
            "grenade3"
        };
        //Networking code below.
        isNetworkPlay = ApplicationManager.isNetworkPlay;
        if (isNetworkPlay)
        {
            playerFaction = ApplicationManager.localPlayerFaction;
            currentActiveFaction = playerFaction;
            PhotonNetwork.room.open = false;    //close the room once a game starts.
        }


		// Getting Player Text messages colors
		Player1Color = GameObject.FindWithTag ("PlayerMessager").GetComponent<PlayerMessageManager> ().Player1Color;
		Player2Color = GameObject.FindWithTag ("PlayerMessager").GetComponent<PlayerMessageManager> ().Player2Color;
		StandardColor = GameObject.FindWithTag ("PlayerMessager").GetComponent<PlayerMessageManager> ().StandardColor;

	}

    void Update()
    {
		GameObject.FindWithTag ("MainCanvas").GetComponent<UIManager> ().CheckExecuteButton ();
        if (isInOrderPhase)
        {
            checkReceivedOrders();
        }
        if (hasSentOrders && hasReceivedOrders && isInOrderPhase)
        {
            isInOrderPhase = false;
            isInExecutionPhase = true;
        }
        if (isInExecutionPhase)
        {
            checkRunningActions();
            //Don't do anything if there are animations running
            if (currentRunningActions.Count != 0)
            {
                return;  
            }
            else //no actions are currently running. 
            {
                if (currentTurnActionQueue.Count > 0) //If there are actions queued, fire them.
                {
                    resolveEndPhase();
                    fireNextPhaseActions();
                } else // there are no more actions queued.
                {
                    if (getFirstAliveSoldierIfExists().queuedActions.Count == 0) //No more actions left. Quitting Execution.
                    {
                        resolveEndPhase();
                        quitExecutionPhase();
                    } else
                    {
                        resolveEndPhase();
                        extractNextTurnActions(); //populates the action queue. Actions are fired at the next frame.
                    }
                }
            }
        }
    }

    //used for skipping dead soldiers in checking actions left
    SoldierManager getFirstAliveSoldierIfExists()
    {
        foreach (SoldierManager soldier in soldiers){
            if (soldier.health > 0)
            {
                return soldier;
            }
        }
        return new SoldierManager(); //very, very hacky solution, but heh, we've got 2 days left.
    }

    void fireNextPhaseActions()
    {
        List<Action> actionsToRun = currentTurnActionQueue.Dequeue();

        //It is VERY IMPORTANT that the currentRunningActions is processed first,
        //Then the actions are fired.

        //Because the actions that run may query the currentRunningActions for other actions running at the same time.
        //This is used in concurrent move resolution.
        foreach (Action action in actionsToRun)
        {
            currentRunningActions.Add(action);
        }



        foreach (Action action in currentRunningActions)
        {
            action.execute();
        }

    }

    void resolveEndPhase()
    {
        for(int i = 0; i < soldiers.Count; i++)
        {
            SoldierManager soldier = soldiers[i];
            if (soldier.health <= 0)
            {
                /*
                Renderer[] renderers = soldier.gameObject.transform.GetComponentsInChildren<Renderer>();
                foreach(Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                 }

                soldier.gameObject.GetComponent<Renderer>().enabled = false;
                */
                board.board[(int) soldier.position.x][(int) soldier.position.y].setEmpty();

                soldiers.Remove(soldier);
                i -= 1;
                Destroy(soldier.gameObject);
            }
        }
        //deal with dead soldiers here.
    }

    void quitExecutionPhase()
    {
        foreach(SoldierManager soldier in soldiers)
        {
            soldier.currentLimitedActionCount = 0;
        }

        if (!isNetworkPlay)
        {
            currentActiveFaction = 0;
			isInExecutionPhase = false;
            isInOrderPhase = true;
			GameObject.FindWithTag("PlayerMessager").GetComponent<PlayerMessageManager>().TextAnimationCaller("Player 1 turn", Player1Color);

		} else
        {
            currentActiveFaction = playerFaction;
            isInExecutionPhase = false;
            isInOrderPhase = true;
            hasSentOrders = false;
            hasReceivedOrders = false;
        }
    }

    void extractNextTurnActions()
    {
        List<Action> nextTurnAction = new List<Action>();
        foreach (SoldierManager soldierManager in soldiers)
        {
            nextTurnAction.Add(soldierManager.queuedActions[0]);
            soldierManager.queuedActions.RemoveAt(0); //Extract All actions for the turn
        }
        nextTurnAction.Sort(
            delegate (Action a1, Action a2)
            {
                return a1.orderInExecution.CompareTo(a2.orderInExecution);
            }); //sort them by execution order
        List<Action> nextActions = new List<Action>();

        foreach (Action action in nextTurnAction)
        {
            if (nextActions.Count == 0 || nextActions[0].orderInExecution == action.orderInExecution)
            {
                nextActions.Add(action);
            }
            else
            {
                currentTurnActionQueue.Enqueue(nextActions);
                nextActions = new List<Action>();
                nextActions.Add(action);
            }
        }
        currentTurnActionQueue.Enqueue(nextActions);

    }

    void checkRunningActions()
    {
        for(int i = 0; i < currentRunningActions.Count; i++)
        {
            Action action = currentRunningActions[i];
            if (action.finishedAnimationExecution)
            {
                currentRunningActions.Remove(action);
                i -= 1;
            }
        }
    }

    //should be called with networking only.
    void checkReceivedOrders()
    {
        foreach (SoldierManager soldier in soldiers)
        {
            if (soldier.queuedActions.Count != maxActions && soldier.faction != playerFaction)
            {
                return;
            }
        }
        hasReceivedOrders = true;

    }

    public int getFaction()
    {
        if (isNetworkPlay)
        {
            return playerFaction;
        } else
        {
            return currentActiveFaction;
        }
    }


}
