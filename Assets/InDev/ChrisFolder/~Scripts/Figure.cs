using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Chris: This is a dummy class for now, and will be replaced with a proper definition.
///     - Player and Emenies should inherit from this class
/// Paul: added some defs
/// </summary>
public abstract class Figure : MonoBehaviour
{
    public enum figSize { SMALL, MEDIUM, LARGE}
    public KeyCode killKey;

    public GameObject theGameObject; // game object that contains the Game state (may replace with a tag lookup)
    [SerializeField]
    protected int startRow = 0, startCol = 0; // starting Hex Location
    public figSize _SIZE; // figure size
    public int _MaxHitPoints = 1;
    public Vector3 MoveTo, MoveFrom,MoveCenter;
    public float MoveStep = .1f;//per second
    protected float MovePercent;
    [HideInInspector]
    public int[] GridCoords, PrevCoords;
    [HideInInspector]
    public int health;
    
    public enum adjHex { None, Left, UpLeft, UpRight, Right, DownRight, DownLeft }

    protected bool onBoard = false;

    protected GameState theGame;

    // First thing, get a reference to the GameState and add self to starting hex
    private void Start()
    {
        health = _MaxHitPoints;
        theGame = theGameObject.GetComponent<GameState>();
    }

    // Need to do this in update instead of start (race condition). I'm sure there's a better way
    public void Update()
    {
        if (!onBoard)
        {
            theGame.addFigure(gameObject, startRow, startCol );
            theGame.DeathPhase += this.Death;
            onBoard = true;
        }
        UpdatePos();
    }


    public void SetMove(GameObject tile)
    {
        MovePercent = 0;
        MoveTo = tile.transform.position + new Vector3(0f, 3f, 0f) ;
      //  Debug.Log(MoveTo.x+" "+ MoveTo.y+" "+ MoveTo.z);
       MoveFrom = this.transform.position;

        MoveCenter = new Vector3(MoveFrom.x + (MoveTo.x - MoveFrom.x) / 2, MoveFrom.y + (MoveTo.y - MoveFrom.y) / 2+2, MoveFrom.z + (MoveTo.z - MoveFrom.z) / 2);
        gameObject.transform.LookAt(MoveTo);
    }
    public void UpdatePos()
    {
      //  Debug.Log("update");
      if(MovePercent<2)
        {
            if(MovePercent<1f)
            {
                this.transform.position = Vector3.Slerp(MoveFrom, MoveCenter, MovePercent);
            }
           
            else
            {
                this.transform.position = Vector3.Slerp(MoveCenter, MoveTo, MovePercent-1f);
            }
               
            MovePercent += MoveStep;
        }

       
    }
    // Chris: debugging "Movement" simply logs event then removes itself from the MovementPhase
    public abstract void Movement();

    public void NeedBounceBack()
    {
        theGame.BounceBack += BounceMove;
    }

    public int[] directionToCoords(adjHex direction)
    {
        int[] result = null;

        switch (direction)
        {
            case adjHex.Left:
                result = new int[2] { GridCoords[0] - 1, GridCoords[1] };
                break;
            case adjHex.UpLeft:
                result = new int[2] { GridCoords[0] - 1, GridCoords[1] + 1 };
                if (result[1] % 2 == 0) { result[0]++; }
                break;
            case adjHex.UpRight:
                result = new int[2] { GridCoords[0], GridCoords[1] + 1 };
                if (result[1] % 2 == 0) { result[0]++; }
                break;
            case adjHex.Right:
                result = new int[2] { GridCoords[0] + 1, GridCoords[1] };
                break;
            case adjHex.DownRight:
                result = new int[2] { GridCoords[0] + 1, GridCoords[1] - 1 };
                if (result[1] % 2 == 1) { result[0]--; }
                break;
            case adjHex.DownLeft:
                result = new int[2] { GridCoords[0], GridCoords[1] - 1 };
                if (result[1] % 2 == 1) { result[0]--; }
                break;
        }

        return result;
    }

    internal void applyDamage(int damage)
    {
        if(damage > 0)
        {
            health -= damage;
        }

        if(health < 0)
        {
            health = 0;
        }
    }

    public void BounceMove()
    {
        theGame.moveToHex(this.gameObject, PrevCoords);
        theGame.BounceBack -= BounceMove;
    }

    // Chris: debugging "Action" simply logs event then removes itself from the ActionPhase
    public abstract void Action();

    // Chris: debugging "Death" simply logs event
    public abstract void Death();
}
