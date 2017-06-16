using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Action
{
    public string text;
    public Sprite sprite;
    public Color UIPanelColor = new Color(1, 1, 1, 0.5f);
    public Vector2 direction = Vector2.zero;   //Replace this with Tuple<int> later.
    public SoldierManager soldier; //the one issuing the order;
    public bool isLimitedAction = false;
    public int damage = 0;
    public int orderInExecution;    //lower values fire first. Same values are fired "simultaneously"
    public bool finishedAnimationExecution = false; //Execution can take time. update this variable so that the game will proceed with next actions.
    public bool finishedLogicExecution = false; //This matters because logic take place at execute() and therefore different soldier are on a differnet timeline.
    public bool BulletCollided = false;
    public Vector2 target;
    public bool isValid;



    public virtual string serialize()
    {
        return GetType().Name + "|" + (int)direction.x + "|" + (int)direction.y + "|" + text;
    }


    public int priority(string s)
    {
        foreach (string action in GameManager.gameManager.priorityList)
        {
            if (action.Contains(s))
            {
                return (int)(action[action.Length - 1] - '0');
            }
        }
        Debug.Log("action did not have priority value");
        return 100;
    }

    public static Action deserialize(string actionString, SoldierManager soldier)
    {
        string[] inputs = actionString.Split('|');
        object[] parameters = new object[3];
        parameters[0] = soldier;
        parameters[1] = new Vector2(float.Parse(inputs[1]), float.Parse(inputs[2]));
        parameters[2] = inputs[3];

        // <Magic> 
        return (Action)System.Activator.CreateInstance(System.Type.GetType(inputs[0]), parameters);
        // </Magic>

    }

    public virtual void execute()
    {
        //Overridden by subclasses
    }

    public virtual Vector2 predictPosition(Vector2 sourcePosition)
    {
        //Overridden by Move. (and charge in theroy);
        return sourcePosition;
    }

    public static Move move(Vector2 direction)
    {
        return new Move(direction);
    }

    public static Shoot shoot(Vector2 direction)
    {
        return new Shoot(direction);
    }

    public static Grenade grenade(Vector2 target)
    {
        return new Grenade(target);
    }

    public static Move move(Vector2 direction, string nameOverride)
    {
        return new Move(GameManager.gameManager.currentSelectedSoldier.Value, direction, nameOverride);
    }

    public static Knife knife()
    {
        return new Knife();
    }

    public static Reload reload()
    {
        return new Reload(GameManager.gameManager.currentSelectedSoldier.Value);
    }
}

public class Move : Action
{
    public Move(Vector2 direction) : this(GameManager.gameManager.currentSelectedSoldier.Value, direction, "move")
    {

    }

    public override Vector2 predictPosition(Vector2 currentPosition)
    {
        return currentPosition + direction;
    }

    public Move(SoldierManager soldier, Vector2 direction, string nameOverride)
    {
        this.soldier = soldier;
        this.direction = direction;
        this.text = nameOverride;
        this.isValid = true;
        bool isDash = false;
        if (direction.magnitude > 1)
        {
            this.isLimitedAction = true;
            isDash = true;
        }
        if (isDash)
        {
            this.orderInExecution = priority("dash");
            this.sprite = Resources.Load<Sprite>("RightSidePanel/dash");
            this.UIPanelColor = new Color(62 / 255f, 0f, 1f, 82 / 255f);

           // Debug.Log(this.sprite);
        }
        else
        {
            this.orderInExecution = priority("move");
            this.sprite = Resources.Load<Sprite>("RightSidePanel/move");
            this.UIPanelColor = new Color(0f, 1f, 23 / 255f, 112 / 255f);
        }


    }

    bool targetWithinBoard(Vector2 targetCoordinate)
    {
        return 0 <= targetCoordinate.x && targetCoordinate.x < GameManager.gameManager.board.width && 0 <= targetCoordinate.y && targetCoordinate.y < GameManager.gameManager.board.height;
    }

    bool hasClearPath(Vector2 targetCoordinate,List<SoldierManager> movSoldiers)
    {
        Vector2 normDir = direction.normalized;
        Vector2 curCell = this.soldier.position;
        for (int i = 1; curCell != targetCoordinate; i++)
        {
            //this checks for walls and other soldiers who aren't dashing/moving
            curCell = i * normDir + this.soldier.position;
            if (!targetWithinBoard(curCell))
            {
                return false;
            }
            if (GameManager.gameManager.board.board[(int)curCell.x][(int)curCell.y].here == Cell.Occupant.Wall)
            {
                return false;
            }
            if (GameManager.gameManager.board.board[(int)curCell.x][(int)curCell.y].here == Cell.Occupant.Soldier)
            {
                //    if (!movSoldiers.Contains(GameManager.gameManager.board.board[(int)curCell.x][(int)curCell.y].getSoldier()))
                //    {
                return false;
            //    }
            }
        }
        return true;
    }

    bool canMoveToTarget(Vector2 targetCoordinate)
    {
        if (!targetWithinBoard(targetCoordinate) || !isValid)
        {
            return false; 
        }
        List<Move> movActions = new List<Move>();
        List<SoldierManager> movSoldiers = new List<SoldierManager>();
        foreach (Action action in GameManager.gameManager.currentRunningActions)
        {
            if (action != this && action is Move ) {
                if ( !action.finishedLogicExecution)
                {
                    Move movAct = (Move)action;
                    movActions.Add(movAct);
                }
                movSoldiers.Add(action.soldier);
            }
        }

        if (!hasClearPath(targetCoordinate, movSoldiers))
        {
            return false;
        }
        //checks for other moving soldiers not landing on your square
        bool noConflict = true;
        foreach (Move move in movActions)
        {
            SoldierManager otherSoldier = move.soldier;
            Vector2 otherTarget = move.soldier.position + move.direction;
            //ignore moves that will fail
            Debug.Log(soldier.position + " " + move.soldier.position);
            Debug.Log(otherTarget + " " + targetCoordinate);
            if (!move.hasClearPath(otherTarget, movSoldiers))
            {
                otherTarget = move.soldier.position;
                move.isValid = false;
            }

            if (otherTarget == targetCoordinate && this.soldier.health <= move.soldier.health)
            {
                if (this.soldier.health == move.soldier.health) move.isValid = false;
                noConflict = false;
            }
        }
        return noConflict;
    }

    public override void execute()
    {
        Vector2 targetCoordinate = new Vector2(soldier.position.x + direction.x, soldier.position.y + direction.y);
        if (canMoveToTarget(targetCoordinate))
        {
            soldier.StartCoroutine(moveToCell(targetCoordinate));
            GameManager.gameManager.board.board[(int)soldier.position.x][(int)soldier.position.y].setEmpty();
            Cell target = GameManager.gameManager.board.board[(int)targetCoordinate.x][(int)targetCoordinate.y];
            if (target.here == Cell.Occupant.ActionUp)
            {
                soldier.maxLimitedAction++;
                target.setEmpty();
            }
            if (target.here == Cell.Occupant.DamageUp)
            {
                soldier.damageMod++;
                target.setEmpty();
            }
            if (target.here == Cell.Occupant.Grenade)
            {
                soldier.grenadeCount++;
                target.setEmpty();
            }
            target.setSoldier(soldier);

            soldier.position = targetCoordinate;
        }
        else
        {
            soldier.StartCoroutine(moveToCell(soldier.position));
            Debug.Log("robot movement failed, target was " + targetCoordinate.x + ", " + targetCoordinate.y);
        }
        finishedLogicExecution = true;
    }

    IEnumerator moveToCell(Vector2 targetCoordinate)
    {
        Vector3 targetPosition = GameManager.gameManager.getWorldPosition(targetCoordinate);
        Vector3 originalPosition = soldier.gameObject.transform.position;
        int tickCounter = 0;

        //Rotation Crap
        Quaternion OriginalRotation = soldier.gameObject.transform.rotation;
        Vector3 targetWorldPosition = GameManager.gameManager.getWorldPosition(soldier.position + direction);



        //Borrowed from http://forum.unity3d.com/threads/slow-rotation-from-point-to-point.45649/
        //our natural rotation for the soldier prefab is terribly broken.
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(0, 0, 1), targetWorldPosition - soldier.gameObject.transform.position);



        while (tickCounter < 20)
        {
            tickCounter += 1;
            soldier.transform.rotation = Quaternion.Slerp(OriginalRotation, newRotation, tickCounter / 20.0f);
            yield return new WaitForSeconds(0.02f);
        }

        tickCounter = 0;

        while (tickCounter < 11)
        {
            Vector3 currentPosition = Vector3.Lerp(originalPosition, targetPosition, tickCounter / 10.0f);
            soldier.gameObject.transform.position = currentPosition;
            tickCounter += 1;
            yield return new WaitForSeconds(0.02f);
        }
        finishedAnimationExecution = true;
    }
}

public class Shoot : Action
{
    public Shoot(Vector2 direction) : this(GameManager.gameManager.currentSelectedSoldier.Value, direction, "Cannon")
    {

    }

    public Shoot(SoldierManager soldier, Vector2 direction, string nameOverride)
    {
        this.soldier = soldier;
        this.direction = direction;
        this.text = nameOverride;
        this.orderInExecution = priority("shoot");
        this.isLimitedAction = true;
        this.damage = 1;
        this.sprite = Resources.Load<Sprite>("RightSidePanel/shoot");
        this.UIPanelColor = new Color(1, 49 / 255f, 39 / 255f, 179 / 255f);

    }


    public override void execute()
    {

        if (false) // (soldier.ammo <= 0) ammo is no more
        {
            this.finishedAnimationExecution = true;
            this.finishedLogicExecution = true;
            Debug.Log("soldier tried to shoot but failed due to no ammo");
            return;
        }
        else
        {
            soldier.ammo -= 1;
        }
        if (soldier.faction == 0)
        {
            GameObject bullet = Object.Instantiate(soldier.bulletPrefab0, soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
            soldier.StartCoroutine(bulletMover(bullet, direction));
        }
        else
        {
            GameObject bullet = Object.Instantiate(soldier.bulletPrefab1, soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
            soldier.StartCoroutine(bulletMover(bullet, direction));
        }
        //fill in better animation when available;
    }
    IEnumerator bulletMover(GameObject bullet, Vector2 direction)
    {
        soldier.StartCoroutine(SoldierRotater(direction));
        int tickCounter = 0;
        bullet.GetComponent<ParticleSystem>().startSize = 0;
        while (tickCounter < 20)
        {
            tickCounter += 1;
            bullet.GetComponent<ParticleSystem>().startSize += 0.5f;
            yield return new WaitForSeconds(0.05f);
        }
        tickCounter = 0;

        Vector2 xy = new Vector2(soldier.position.x + direction.x, soldier.position.y + direction.y);
        Vector2 destination = new Vector2(0, 0);
        bool hit = false;
        for (; 0 <= xy.x && xy.x < GameManager.gameManager.board.width && 0 <= xy.y && xy.y < GameManager.gameManager.board.height; xy += direction)
        {
            Cell cell = GameManager.gameManager.board.board[(int)xy.x][(int)xy.y];
            if (cell.here == Cell.Occupant.Wall || cell.here == Cell.Occupant.Soldier)
            {
                destination = xy;
                hit = true;
                break;
            }
            destination = xy;
        }
        if (!hit)
        {
            destination += direction * 2;
        }
        Vector3 bulletTarget = GameManager.gameManager.getWorldPosition(destination);
        float bulletTime = Vector3.Distance(bulletTarget, soldier.transform.position) / 75;
        float t = 0;

        while (t < bulletTime)
        {
            bullet.gameObject.transform.position = Vector3.Lerp(soldier.transform.position, bulletTarget, t / bulletTime);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Object.Destroy(bullet);
        finishedAnimationExecution = true;
        if (hit)
        {
            Cell hitCell = GameManager.gameManager.board.board[(int)destination.x][(int)destination.y];
            if (hitCell.here == Cell.Occupant.Soldier)
            {
                SoldierManager cellSoldier = hitCell.getSoldier();
                bool friend = cellSoldier.faction == soldier.faction;
                if (!friend)
                {
                    cellSoldier.health -= this.damage + soldier.damageMod;
                }
            }
        }
        finishedLogicExecution = true;

    }

    IEnumerator SoldierRotater(Vector2 direction)
    {
        int tickCounter = 0;
        Quaternion OriginalRotation = soldier.gameObject.transform.rotation;
        Vector3 targetWorldPosition = GameManager.gameManager.getWorldPosition(soldier.position + direction);

       // Debug.Log(targetWorldPosition);
       // Debug.Log(soldier.gameObject.transform.position);

        //Borrowed from http://forum.unity3d.com/threads/slow-rotation-from-point-to-point.45649/
        //our natural rotation for the soldier prefab is terribly broken.
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(0, 0, 1), targetWorldPosition - soldier.gameObject.transform.position);



        while (tickCounter < 20)
        {
            tickCounter += 1;
            soldier.transform.rotation = Quaternion.Slerp(OriginalRotation, newRotation, tickCounter / 20.0f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}

public class Knife : Action
{
    public Knife() : this(GameManager.gameManager.currentSelectedSoldier.Value, "Pulse")
    {

    }

    public Knife(SoldierManager soldier, string nameOverride)
    {
        this.soldier = soldier;
        this.text = nameOverride;
        this.orderInExecution = priority("knife");
        this.isLimitedAction = true;
        this.damage = 2;
        this.sprite = Resources.Load<Sprite>("RightSidePanel/pulse");
        this.UIPanelColor = new Color(1, 49 / 255f, 39 / 255f, 179 / 255f);

    }


    public Knife(SoldierManager soldier, Vector2 direction, string nameOverride) : this(soldier, nameOverride) { }

    public override void execute()
    {
        //fill in logic when multiplayer features become available
        for (int x = (int)(soldier.position.x - 1); x <= soldier.position.x + 1; x++)
        {
            for (int y = (int)(soldier.position.y - 1); y <= soldier.position.y + 1; y++)
            {
                if (x < 0 || y < 0 || x >= GameManager.gameManager.board.width || y >= GameManager.gameManager.board.height)
                {
                    continue;
                }
                Cell cell = GameManager.gameManager.board.board[x][y];
                if (cell.here == Cell.Occupant.Soldier)
                {
                    SoldierManager cellSoldier = cell.getSoldier();
                    bool friend = cellSoldier.faction == soldier.faction;
                    if (!friend)
                    {
                        cellSoldier.health -= this.damage + soldier.damageMod;
                    }
                }
            }
        }
        //fill in better animation when available;
        //GameObject knifePrefab = Object.Instantiate(Resources.Load("KnifeEffectPrefab"), soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
        if (soldier.faction == 0)
        {
            GameObject PulsePrefab = Object.Instantiate(soldier.PulsePrefabFaction1, soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
            soldier.StartCoroutine(knifeEffect(PulsePrefab));

        }
        else
        {
            GameObject PulsePrefab = Object.Instantiate(soldier.PulsePrefabFaction2, soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
            soldier.StartCoroutine(knifeEffect(PulsePrefab));

        }
        finishedLogicExecution = true;
    }


    IEnumerator knifeEffect(GameObject PulsePrefab)
    {
        int tickCounter = 0;
        while (tickCounter < 50)
        {
            tickCounter += 1;
            yield return new WaitForSeconds(0.05f);
        }
        Object.Destroy(PulsePrefab);
        finishedAnimationExecution = true;
    }
}

public class Grenade : Action
{
    public Grenade(Vector2 direction) : this(GameManager.gameManager.currentSelectedSoldier.Value, direction, "Grenade")
    {

    }
    public Grenade(SoldierManager soldier, Vector2 direction, string nameOverride)
    {
        this.soldier = soldier;
        this.text = "grenade";
        this.damage = 1;
        this.orderInExecution = priority("grenade");
        this.direction = direction;
        this.isLimitedAction = true;
        this.sprite = Resources.Load<Sprite>("RightSidePanel/grenade");
        this.UIPanelColor = new Color(156/255f, 108 / 255f, 35 / 255f, 1f);
        


    }

    public override void execute()
    {
        if (soldier.grenadeCount == 0)
        {
            this.finishedAnimationExecution = true;
            this.finishedLogicExecution = true;
            Debug.Log("soldier tried to grenade but failed due to having 0 grenade count");
            return;
        }
        soldier.grenadeCount--;
        //no extra logic needed for multiplayer support.

        //fill in better animation when available;
        //GameObject knifePrefab = Object.Instantiate(Resources.Load("KnifeEffectPrefab"), soldier.gameObject.transform.position, Quaternion.identity) as GameObject;
        Vector3 grenadeTarget = GameManager.gameManager.board.boardToWorld(direction) + new Vector3(0, 0, -30);
        Vector3 grenadeSource = GameManager.gameManager.board.boardToWorld(soldier.position) + new Vector3(0, 0, -30); 
        soldier.StartCoroutine(grenadeEffect(grenadeSource, grenadeTarget));
    }

    IEnumerator grenadeEffect(Vector3 source, Vector3 target)
    {
        //better iteration code that is smoother!;
        float grenadeSpeed = 30;
        float time = Vector3.Distance(source, target) / grenadeSpeed;
        float t = 0;
        GameObject grenadeProjectile = Object.Instantiate(Resources.Load("grenadePrefab"), source, Quaternion.identity) as GameObject;
        while (t < time)
        {
            t += Time.deltaTime;
            grenadeProjectile.transform.position = Vector3.Lerp(source, target, t / time);
            yield return new WaitForEndOfFrame();
        }
        Object.Destroy(grenadeProjectile);
        Object.Instantiate(Resources.Load("grenadeExplosion"), target, Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
        for (int x = (int)(direction.x - 1); x <= direction.x + 1; x++)
        {
            for (int y = (int)(direction.y - 1); y <= direction.y + 1; y++)
            {
                if (x < 0 || y < 0 || x >= GameManager.gameManager.board.width || y >= GameManager.gameManager.board.height)
                {
                    continue;
                }
                Cell cell = GameManager.gameManager.board.board[x][y];
                if (cell.here != Cell.Occupant.Wall) {
                    Object.Instantiate(Resources.Load("Explosions/Explosion03b"), cell.gameObject.transform.position, Quaternion.identity);
                }
                if (cell.here == Cell.Occupant.Soldier)
                {
                    SoldierManager cellSoldier = cell.getSoldier();
                    bool friend = cellSoldier.faction == soldier.faction;
                    if (!friend)
                    {
                        cellSoldier.health -= this.damage + soldier.damageMod;
                    }
                }
            }
        }
        finishedLogicExecution = true;
        finishedAnimationExecution = true;

    }
}

public class Reload : Action
{
    public Reload(SoldierManager soldier)
    {
        this.soldier = soldier;
        this.text = "Idle";
        this.orderInExecution = priority("reload");
        this.isLimitedAction = false;
        this.sprite = Resources.Load<Sprite>("RightSidePanel/recharge");
        this.UIPanelColor = new Color(239 / 255f, 255 / 255f, 94 / 255f, 0.8f);

    }

    public Reload(SoldierManager soldier, Vector2 direction, string nameOverride) : this(soldier) { }

    public override void execute()
    {
        finishedAnimationExecution = true;
        finishedLogicExecution = true;
    }
}