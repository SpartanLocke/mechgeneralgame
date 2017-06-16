using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TutorialManager : MonoBehaviour {

    public List<GameObject> tutorialItems = new List<GameObject>();
    List<ReturnTrueOnce> conditions = new List<ReturnTrueOnce>(); //manually constructed and populated;
    int index = 0;
    int counter = 0;

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetInt("tutorial", 1) == 0)
        {
            this.gameObject.SetActive(false);
            return;
        }
        deactivateAll();
        tutorialItems[0].SetActive(true);
        conditions.Add(new ReturnTrueOnce(() => { return GameManager.gameManager.currentSelectedSoldier.HasValue; }));
        conditions.Add(new ReturnTrueOnce(() => { return GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count >= 1; }));
        conditions.Add(new ReturnTrueOnce(() => { return GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count != 1; }));
        conditions.Add(new ReturnTrueOnce(() =>
        {
            if (Input.touchCount == 0) { counter = 1; }
            return (counter != 0 && Input.touchCount > 1 || Input.GetMouseButtonDown(0));
        }));
        conditions.Add(new ReturnTrueOnce(() => {
            int notQueuedActions = 0;
            foreach (SoldierManager s in GameManager.gameManager.soldiers)
            {
                if (s.faction == (GameManager.gameManager.isNetworkPlay ? GameManager.gameManager.playerFaction : GameManager.gameManager.currentActiveFaction))
                {
                    notQueuedActions += GameManager.maxActions - s.queuedActions.Count;
                }
            }
            return notQueuedActions == 0;}));
        conditions.Add(new ReturnTrueOnce(() =>
        {
            if (Input.touchCount == 0) { counter = 1; }
            return (counter != 0 && Input.touchCount > 1 || Input.GetMouseButtonDown(0));
        }));
    }

    // Update is called once per frame
    void Update () {
	    for (int i = 0; i <= index; i++)
        {
            if (i < conditions.Count) { 
                if (conditions[i].evaluate())
                {
                    counter = 0;
                    advance();
                    break;
                }
            } else
            {
                PlayerPrefs.SetInt("tutorial", 0);
            }
        }
	}


    public void advance()
    {
        index++;
        deactivateAll();
        if (tutorialItems.Count > index)
        {
            tutorialItems[index].SetActive(true);
        }
    }

    void deactivateAll()
    {
        foreach(GameObject o in tutorialItems)
        {
            o.SetActive(false);
        }
    }
}

//returns true on a condition once and only once;
class ReturnTrueOnce
{
    Func<bool> condition;
    public ReturnTrueOnce(Func<bool> f)
    {
        condition = f;
    }
    bool returnedTrue = false;
    public bool evaluate()
    {
        if (returnedTrue)
        {
            return false;
        } else
        {
            returnedTrue = condition();
            return returnedTrue;
        }
    }
}
