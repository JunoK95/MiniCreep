using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Figure Script: handles player input and converts them into commands to the gameState
///     - additionally, the player 'instantiates' a selected cell object to indicate where there player ouls like to move
/// </summary>
public class SimpleEnemyMovement : Figure
{

    // Player needs a "selected" tile object
    private enum adjHex { None, Left, UpLeft, UpRight, Right, DownRight, DownLeft }
    private adjHex selectedHex = adjHex.None;
    private GameBoard.HexDirection currentDirection = GameBoard.HexDirection.Left;
    public GameObject _itemDrop;

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
        if(currentDirection == GameBoard.HexDirection.Left)
        {
            try
            {
                HexClass nextHex = theGame.TheGameBoard._HexGrid[this.GridCoords[0] - 1, this.GridCoords[1]];
                if (!nextHex.GetPassable())
                {
                    throw new Exception();
                }
                currentDirection = GameBoard.HexDirection.Left;
                theGame.moveDirection(gameObject, currentDirection);
                selectedHex = adjHex.None;
            }
            catch
            {
                currentDirection = GameBoard.HexDirection.Right;
                HexClass nextHex = theGame.TheGameBoard._HexGrid[this.GridCoords[0], this.GridCoords[1]];
                Debug.Log("Null Detected" + nextHex.Address[0].ToString() + nextHex.Address[1].ToString() + "catch");
                theGame.moveDirection(gameObject, currentDirection);
                selectedHex = adjHex.None;
            }
        }

        else if (currentDirection == GameBoard.HexDirection.Right)
        {
            try
            {
                HexClass nextHex = theGame.TheGameBoard._HexGrid[this.GridCoords[0] + 1, this.GridCoords[1]];
                if (!nextHex.GetPassable())
                {
                    throw new Exception();
                }
                currentDirection = GameBoard.HexDirection.Right;
                theGame.moveDirection(gameObject, currentDirection);
                selectedHex = adjHex.None;
            }
            catch
            {
                currentDirection = GameBoard.HexDirection.Left;
                HexClass nextHex = theGame.TheGameBoard._HexGrid[this.GridCoords[0], this.GridCoords[1]];
                Debug.Log("Null Detected" + nextHex.Address[0].ToString() + nextHex.Address[1].ToString() + "catch");
                theGame.moveDirection(gameObject, currentDirection);
                selectedHex = adjHex.None;
            }
        }


        //if (this.GridCoords[0] == 0)
        //{
        //    currentDirection = GameBoard.HexDirection.Right;
        //
        //}
        //else if (this.GridCoords[0] == theGame.TheGameBoard.boardWidth - 1)
        //{
        //    currentDirection = GameBoard.HexDirection.Left;
        //}
        //theGame.moveDirection(gameObject, currentDirection);
        //// reset selectedHex to None
        //selectedHex = adjHex.None;
        //
        //// remove movement from phase
        //theGame.MovementPhase -= Movement;
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
