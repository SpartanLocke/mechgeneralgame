using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Cell: MonoBehaviour{

	public Occupant here = Occupant.Empty;
	private SoldierManager soldier;
	public enum Occupant{Empty,Wall,Soldier,ActionUp,DamageUp,Grenade,Portal};
    public Vector2 shootVec = new Vector2(0,0);
    public Vector2 grenadeTarget = new Vector2(-1, -1);
    GameObject powerObj;

    //Move Cell variables (triggered to highlight)
    public bool UpCell = false;
	public bool DownCell=false;
	public bool RightCell = false;
	public bool LeftCell =  false;

	public bool DashUpCell = false;
	public bool DashDownCell = false;
	public bool DashLeftCell = false;
	public bool DashRightCell = false;


    public void ResetCell(){
		UpCell = false;
		DownCell = false;
		RightCell = false;
		LeftCell = false;
		DashUpCell = false;
		DashDownCell = false;
		DashLeftCell = false;
		DashRightCell = false;

        shootVec = new Vector2(0, 0);
        grenadeTarget = new Vector2(-1, -1);
    }

	public void OnMouseDown(){// Grade A+ for efficienct coding! senpai pls
        if (UpCell)
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueUp();
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
			GameManager.gameManager.board.MoveHighLight();
            return;
        }
        if (DownCell)
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueDown();
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
            return;
        }

        if (RightCell)
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueRight();
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
            return;
        }
        if (LeftCell)
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueLeft();
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
            return;
        }

		if (DashUpCell) {
			GameObject.FindWithTag ("MainCanvas").GetComponent<UIManager> ().QueDashUp ();
			GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
			return;
		}
		if (DashDownCell) {
			GameObject.FindWithTag ("MainCanvas").GetComponent<UIManager> ().QueDashDown ();
			GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
			return;
		}

		if (DashRightCell) {
			GameObject.FindWithTag ("MainCanvas").GetComponent<UIManager> ().QueDashRight ();
			GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
			return;
		}

		if (DashLeftCell) {
			GameObject.FindWithTag ("MainCanvas").GetComponent<UIManager> ().QueDashLeft ();
			GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
			return;
		}

        if (grenadeTarget != new Vector2(-1, -1))
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueGrenade(grenadeTarget);
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
            if (GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count < 3)
                GameManager.gameManager.board.MoveHighLight();
            return;
        }

		if (shootVec != new Vector2(0, 0))
        {
            GameObject.FindWithTag("MainCanvas").GetComponent<UIManager>().QueShoot(shootVec);
            GameObject.FindWithTag("Plane").GetComponent<Board>().ResetBoardColors();
			if(GameManager.gameManager.currentSelectedSoldier.Value.queuedActions.Count<3)
				GameManager.gameManager.board.MoveHighLight();
            return;
        }
    }

	public bool isHighlighted(){
		return this.gameObject.GetComponent<SpriteRenderer> ().color == GameManager.gameManager.board.CellMoveColor || this.transform.FindChild ("Canvas").FindChild ("Panel").GetComponent<Image> ().enabled;
	}

	public SoldierManager getSoldier(){
		if (here != Occupant.Soldier) {
			Debug.LogError("get soldier called when soldier doesn't exist in cell");
		}
		return soldier;
	}

    public void setSoldier(SoldierManager soldier)
    {
        this.soldier = soldier;
        this.here = Occupant.Soldier;
    }

    public void setWall()
    {
        this.here = Occupant.Wall;
    }

    public void setEmpty(){
        if (this.powerObj != null)
        {
            Destroy(this.powerObj);
        }
		this.here = Occupant.Empty;
	}

    public void setActionUp(GameObject o)
    {
        this.powerObj = o;
        this.here = Occupant.ActionUp;
    }

    public void setGrenade(GameObject o)
    {
        this.powerObj = o;
        this.here = Occupant.Grenade;
    }

    public void setDamageUp(GameObject o)
    {
        this.powerObj =o;
        this.here = Occupant.DamageUp;
    }

    public bool isPassable()
    {
        return !(this.here == Occupant.Soldier || this.here == Occupant.Wall);
    }

}
