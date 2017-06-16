using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class Board : MonoBehaviour
{

    public List<List<Cell>> board = new List<List<Cell>>();
    List<List<string>> boardStrings = new List<List<string>>();
    public int width;
    public int height;
    public GameObject wallPrefab;
    public GameObject soldierPrefab;
    public GameObject cellPrefab;
    public GameObject damageUpPrefab;
    public GameObject actionUpPrefab;
    public GameObject grenadePrefab;

	private Vector3 OriginalWallRotation;
    int soldierID = 1;

    //Cellhighlting stuff
    public Color CellNormalColor;
    public Color CellMoveColor;
    void Awake()
    {
        //string s = "";
        //for (int i = 0; i < 9; i++)
        //{

        //    for (int j = 0; j < 9 -1; j++)
        //    {
        //        s += " |";
        //    }
        //    s += " \n";
        //}

        //Debug.Log(s.Substring(0,s.Length-1));
    }

    void Start()
    {
        loadBoard();
        GameManager.gameManager.addBoard(this);
        for (int x = 0; x < width; x++)
        {
            board.Add(new List<Cell>());
            for (int y = 0; y < height; y++)
            {
                GameObject obj = Instantiate(cellPrefab, boardToWorld(new Vector2(x, y)), Quaternion.identity) as GameObject;
                Cell cell = obj.GetComponent<Cell>();
                board[x].Add(cell);
                string cellMem = boardStrings[y][x]; //reverse index
                if (cellMem[0] == 'b')  //Comparing chars seems to work better for unknown reasons.
                {
                    GameObject obj2 = Instantiate(soldierPrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.AngleAxis(180, Vector3.forward)) as GameObject;
                    SoldierManager s = obj2.GetComponent<SoldierManager>();
                    setupSoldier(s, x, y, 1);
                    cell.setSoldier(s);

                }
                else if (cellMem[0] == 'r')
                {
                    GameObject obj2 = Instantiate(soldierPrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.identity) as GameObject;
                    SoldierManager s = obj2.GetComponent<SoldierManager>();
                    setupSoldier(s, x, y, 0);
                    cell.setSoldier(s);

                }
                else if (cellMem[0] == '#')
                {
                    GameObject obj2 = Instantiate(wallPrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.identity) as GameObject;
					OriginalWallRotation = new Vector3(obj2.transform.eulerAngles.x, obj2.transform.eulerAngles.y, obj2.transform.eulerAngles.z);
					OriginalWallRotation.z = UnityEngine.Random.Range (0,359);
					obj2.transform.eulerAngles = OriginalWallRotation;
						board[x][y].setWall();
                }
                else if (cellMem[0] == 'd')
                {
                    GameObject obj2 = Instantiate(damageUpPrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.identity) as GameObject;
                    board[x][y].setDamageUp(obj2);
                }
                else if (cellMem[0] == 'a')
                {
                    GameObject obj2 = Instantiate(actionUpPrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.identity) as GameObject;
                    board[x][y].setActionUp(obj2);
                }
                else if (cellMem[0] == 'g')
                {
                    GameObject obj2 = Instantiate(grenadePrefab, boardToWorld(new Vector2(x, y), 1), Quaternion.identity) as GameObject;
                    board[x][y].setGrenade(obj2);
                }

            }
        }
        ResetBoardColors();

    }

    void setupSoldier(SoldierManager soldier, int xCoord, int yCoord, int faction)
    {
        soldier.position.x = xCoord;
        soldier.position.y = yCoord;
        soldier.OriginalPosition = boardToWorld(new Vector2(xCoord, yCoord), 1);
        soldier.faction = faction;
        //a bit hacky

        soldier.gameObject.GetPhotonView().viewID = soldierID;
        soldierID++;
    }

    void loadBoard()
    {
        TextAsset map = Resources.Load("bloodarena") as TextAsset;
        string[] lines = map.text.Split('\n');

        height = lines.Length;
        width = lines[0].Length / 2; // because of | and newline
        for (int i = height - 1; i >= 0; i--)
        {
            string[] entries = lines[i].Split('|');
            boardStrings.Add(new List<string>(entries));
            if (width != entries.Length)
            {
                Debug.Log("improper format. rows must be same length");
                break;
            }

        }

        //for (int h = 0; h < height; h++)
        //{
        //    string sr="";
        //    for (int i = 0; i < width; i++)
        //    {
        //        sr += boardStrings[h][i];
        //    }
        //    Debug.Log(sr);
        //}
    }

    public Vector3 boardToWorld(Vector2 pos, int t = 0)
    {
        return new Vector3(pos.x * 10 - 35, pos.y * 10 - 45, -2 - (t * 18));
    }

    public void ResetBoardColors()
    {
        int ResetX = 0;
        int ResetY = 0;
        for (ResetX = 0; ResetX < GameManager.gameManager.board.width; ResetX++)
        {
            for (ResetY = 0; ResetY < GameManager.gameManager.board.height; ResetY++)
            {
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.GetComponent<SpriteRenderer>().color = CellNormalColor;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").Find("Panel").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").FindChild("Image").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").Find("DashUp").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").Find("DashDown").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").Find("DashLeft").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.transform.FindChild("Canvas").Find("DashRight").GetComponent<Image>().enabled = false;
                GameManager.gameManager.board.board[ResetX][ResetY].gameObject.GetComponent<Cell>().ResetCell();

            }
        }
    }

    public void MoveHighLight()
    {// Sloppy code help pls senpai.
        ResetBoardColors();
        int CellX = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().x;
        int CellY = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().y;

        //Check Right
        if (CellX + 1 < GameManager.gameManager.board.width)
        {
            if (GameManager.gameManager.board.board[CellX + 1][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX + 1][CellY].gameObject.GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX + 1][CellY].gameObject.GetComponent<Cell>().RightCell = true;
            }
        }
        //Check up
        if (CellY + 1 < GameManager.gameManager.board.height)
        {
            if (GameManager.gameManager.board.board[CellX][CellY + 1].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX][CellY + 1].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY + 1].GetComponent<Cell>().UpCell = true;

            }
        }

        //Check Left
        if (CellX - 1 >= 0)
        {
            if (GameManager.gameManager.board.board[CellX - 1][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX - 1][CellY].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX - 1][CellY].GetComponent<Cell>().LeftCell = true;

            }
        }

        //Check Down
        if (CellY - 1 >= 0)
        {
            if (GameManager.gameManager.board.board[CellX][CellY - 1].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX][CellY - 1].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY - 1].GetComponent<Cell>().DownCell = true;

            }
        }
    }

    public void ShootHighLight()
    {
        ResetBoardColors();
        int CellX = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().x;
        int CellY = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().y;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int ShootTileX = CellX + x;
                int ShootTileY = CellY + y;

                while ((x != 0 || y != 0) && ShootTileX >= 0 && ShootTileX < GameManager.gameManager.board.width && ShootTileY >= 0 && ShootTileY < GameManager.gameManager.board.height)
                {
                    Cell cell = GameManager.gameManager.board.board[ShootTileX][ShootTileY];
                    cell.transform.FindChild("Canvas").FindChild("Panel").GetComponent<Image>().enabled = true;
                    cell.transform.FindChild("Canvas").FindChild("Image").GetComponent<Image>().enabled = true;
                    cell.GetComponent<Cell>().shootVec = new Vector2(x, y);
                    if (!cell.isPassable())
                    {
                        break;
                    }
                    ShootTileX += x;
                    ShootTileY += y;
                }
            }
        }
    }

    public void GrenadeHighLight()
    {
        if (GameManager.gameManager.currentSelectedSoldier.Value.grenadeCount == 0)
        {
            return;
        }
        ResetBoardColors();
        int CellX = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().x;
        int CellY = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().y;
        int grenadeRange = 4;
        for (int x = -grenadeRange; x <= grenadeRange; x++)
        {
            for (int y = -grenadeRange; y <= grenadeRange; y++)
            {
                int ShootTileX = CellX + x;
                int ShootTileY = CellY + y;

                if ((Math.Abs(x) + Math.Abs(y) <= grenadeRange) && ShootTileX >= 0 && ShootTileX < GameManager.gameManager.board.width && ShootTileY >= 0 && ShootTileY < GameManager.gameManager.board.height)
                {
                    Cell cell = GameManager.gameManager.board.board[ShootTileX][ShootTileY];
                    if (cell.here != Cell.Occupant.Wall)
                    {
                        cell.transform.FindChild("Canvas").FindChild("Panel").GetComponent<Image>().enabled = true;
                        cell.transform.FindChild("Canvas").FindChild("Image").GetComponent<Image>().enabled = true;
                        cell.GetComponent<Cell>().grenadeTarget = new Vector2(ShootTileX, ShootTileY);
                    }
                }
            }
        }
    }

    public void DashHighLight()
    {
        ResetBoardColors();
        int CellX = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().x;
        int CellY = (int)GameManager.gameManager.currentSelectedSoldier.Value.getPredictedPosition().y;

        //Check Right
        if (CellX + 2 < GameManager.gameManager.board.width)
        {
            if (GameManager.gameManager.board.board[CellX + 2][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall &&
                GameManager.gameManager.board.board[CellX + 1][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX + 2][CellY].gameObject.GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY].gameObject.transform.FindChild("Canvas").Find("DashRight").GetComponent<Image>().enabled = true;
                GameManager.gameManager.board.board[CellX + 2][CellY].gameObject.GetComponent<Cell>().DashRightCell = true;
            }
        }
        //Check up
        if (CellY + 2 < GameManager.gameManager.board.height)
        {
            if (GameManager.gameManager.board.board[CellX][CellY + 2].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall &&
                GameManager.gameManager.board.board[CellX][CellY + 1].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX][CellY + 2].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY].gameObject.transform.FindChild("Canvas").Find("DashUp").GetComponent<Image>().enabled = true;
                GameManager.gameManager.board.board[CellX][CellY + 2].GetComponent<Cell>().DashUpCell = true;

            }
        }

        //Check Left
        if (CellX - 2 >= 0)
        {
            if (GameManager.gameManager.board.board[CellX - 2][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall &&
                GameManager.gameManager.board.board[CellX - 1][CellY].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX - 2][CellY].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY].gameObject.transform.FindChild("Canvas").Find("DashLeft").GetComponent<Image>().enabled = true;
                GameManager.gameManager.board.board[CellX - 2][CellY].GetComponent<Cell>().DashLeftCell = true;

            }
        }

        //Check Down
        if (CellY - 2 >= 0)
        {
            if (GameManager.gameManager.board.board[CellX][CellY - 2].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall &&
                GameManager.gameManager.board.board[CellX][CellY - 1].gameObject.GetComponent<Cell>().here != Cell.Occupant.Wall)
            {
                GameManager.gameManager.board.board[CellX][CellY - 2].GetComponent<SpriteRenderer>().color = CellMoveColor;
                GameManager.gameManager.board.board[CellX][CellY].gameObject.transform.FindChild("Canvas").Find("DashDown").GetComponent<Image>().enabled = true;
                GameManager.gameManager.board.board[CellX][CellY - 2].GetComponent<Cell>().DashDownCell = true;

            }
        }
    }
}



