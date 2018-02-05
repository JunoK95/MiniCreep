using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Figure Script: handles player input and converts them into commands to the gameState
///     - additionally, the player 'instantiates' a selected cell object to indicate where there player ouls like to move
/// </summary>
public class EnemyPatrolMovement : Figure
{

    // Player needs a "selected" tile object
    private adjHex selectedHex = adjHex.None;
    private GameBoard.HexDirection currentDirection;
    public GameObject _itemDrop;
    public adjHex[] moveList;
    private int currentMove = 0;

    private new void Update()
    {
        // from parent (see Figure class)
        if (!this.onBoard)
        {
            theGame.addFigure(gameObject, startRow, startCol);
            theGame.DeathPhase += this.Death;
            theGame.MovementPhase += this.Movement;
            onBoard = true;

        }
        if (currentMove >= moveList.Length - 1)
        {
            currentMove = 0;
        }
        UpdatePos();
        // Listen for Inputs from user: moves the "selected" object to an adjacent cell
        // if (Input.anyKeyDown && health > 0)
        // {
        //     handleInput();
        // }

    }

    // move in the direction of the selection object
    public override void Movement()
    {
        adjHex[] newMoveList = moveList;
        for (int i = 0; i < moveList.Length; i++)
        {
            currentDirection = convertAdjToDir(moveList[currentMove]);
            if (checkNextMove(convertDirToAdj(currentDirection)))
            {
                theGame.moveDirection(gameObject, currentDirection);
                selectedHex = adjHex.None;
                break;
            }
            currentMove++;
        }
    }

    private GameBoard.HexDirection convertAdjToDir(adjHex adj)
    {
        if (adj == adjHex.Left)
        {
            return GameBoard.HexDirection.Left;
        }
        else if (adj == adjHex.Right)
        {
            return GameBoard.HexDirection.Right;
        }
        else if (adj == adjHex.DownLeft)
        {
            return GameBoard.HexDirection.DownLeft;
        }
        else if (adj == adjHex.DownRight)
        {
            return GameBoard.HexDirection.DownRight;
        }
        else if (adj == adjHex.UpLeft)
        {
            return GameBoard.HexDirection.UpLeft;
        }
        else if (adj == adjHex.UpRight)
        {
            return GameBoard.HexDirection.UpRight;
        }
        else
        {
            throw new Exception("not a valid adj");
        }
    }

    private adjHex convertDirToAdj(GameBoard.HexDirection direction)
    {
        if (direction == GameBoard.HexDirection.Left)
        {
            return adjHex.Left;
        }
        else if(direction == GameBoard.HexDirection.Right)
        {
            return adjHex.Right;
        }
        else if (direction == GameBoard.HexDirection.DownLeft)
        {
            return adjHex.DownLeft;
        }
        else if (direction == GameBoard.HexDirection.UpLeft)
        {
            return adjHex.UpLeft;
        }
        else if (direction == GameBoard.HexDirection.DownRight)
        {
            return adjHex.DownRight;
        }
        else if (direction == GameBoard.HexDirection.UpRight)
        {
            return adjHex.UpRight;
        }
        else
        {
            return adjHex.None;
        }
    }

    private bool checkNextMove(adjHex dir)
    {
        int[] result = null;

        switch (dir)
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

        try
        {
            HexClass nextHex = theGame.TheGameBoard._HexGrid[result[0],result[1]];
            if (!nextHex.GetPassable())
            {
                throw new Exception();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     - determine which hex gets attacked
    ///     - attack that hexTile
    /// </summary>
    public override void Action()
    {
        Debug.Log("Ker-POW!! " + gameObject.name + " attacked " + selectedHex);

        // reset selectedHex to None
        selectedHex = adjHex.None;

        // remove action from phase
        theGame.ActionPhase -= Action;
    }

    // check if the player is dead
    public override void Death()
    {
        if (health <= 0)
        {
            theGame.DeathPhase -= Death;
            spawnItem();
            theGame.TheGameBoard.clearHex(GridCoords);
            gameObject.SetActive(false);
        }
    }

    public void spawnItem()
    {
        _itemDrop = Instantiate(_itemDrop, gameObject.transform.position, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
        theGame.addFigure(_itemDrop, GridCoords[0], GridCoords[1]);
    }


    /// <summary>
    /// Looks for key presses and re-assigns the selectedHex variable
    /// </summary>
    private void handleInput()
    {
        // "RETURN" key indicates a move request
        if (Input.GetKeyDown(KeyCode.Return) && selectedHex != adjHex.None)
        {
            // register the move to the GameState
            theGame.MovementPhase += Movement;
            Debug.Log("movementAdded");

            // trigger roundStart to gameState
        }
        // "A" key indicates an attack request
        else if (Input.GetKeyDown(KeyCode.A) && selectedHex != adjHex.None)
        {
            // register the action to the gameState
            theGame.MovementPhase += Movement;

            // trigger roundStart to gameState
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (selectedHex)
            {
                case adjHex.Left:
                    selectedHex = adjHex.UpLeft;
                    break;
                case adjHex.DownLeft:
                    selectedHex = adjHex.Left;
                    break;
                case adjHex.None:
                case adjHex.Right:
                    selectedHex = adjHex.UpRight;
                    break;
                case adjHex.DownRight:
                    selectedHex = adjHex.Right;
                    break;
            }

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (selectedHex)
            {
                case adjHex.None:
                case adjHex.Left:
                    selectedHex = adjHex.DownLeft;
                    break;
                case adjHex.UpLeft:
                    selectedHex = adjHex.Left;
                    break;
                case adjHex.Right:
                    selectedHex = adjHex.DownRight;
                    break;
                case adjHex.UpRight:
                    selectedHex = adjHex.Right;
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switch (selectedHex)
            {
                case adjHex.None:
                case adjHex.UpLeft:
                case adjHex.DownLeft:
                    selectedHex = adjHex.Left;
                    break;
                case adjHex.UpRight:
                    selectedHex = adjHex.UpLeft;
                    break;
                case adjHex.DownRight:
                    selectedHex = adjHex.DownLeft;
                    break;
                case adjHex.Right:
                    selectedHex = adjHex.None;
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (selectedHex)
            {
                case adjHex.None:
                case adjHex.UpRight:
                case adjHex.DownRight:
                    selectedHex = adjHex.Right;
                    break;
                case adjHex.UpLeft:
                    selectedHex = adjHex.UpRight;
                    break;
                case adjHex.DownLeft:
                    selectedHex = adjHex.DownRight;
                    break;
                case adjHex.Left:
                    selectedHex = adjHex.None;
                    break;
            }
        }
        // for debugging - Kills the Figure
        else if (Input.GetKeyDown(killKey))
        {
            health = 0;
            theGame.DeathPhase -= Death;
            theGame.TheGameBoard.clearHex(GridCoords);
            spawnItem();
            gameObject.SetActive(false);
        }

    }

}
