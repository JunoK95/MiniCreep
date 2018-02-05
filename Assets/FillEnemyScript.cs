using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Figure Script: handles player input and converts them into commands to the gameState
///     - additionally, the player 'instantiates' a selected cell object to indicate where there player ouls like to move
/// </summary>
public class FillEnemyScript : Figure
{

    // Player needs a "selected" tile object
    private adjHex selectedHex = adjHex.None;
    private GameBoard.HexDirection currentDirection = GameBoard.HexDirection.Left;
    public GameObject _itemDrop;

    //Viscosity handles how many turns it takes for the lava to move
    public int viscosity = 1;
    private int viscCounter = 0;

    //Current coordinates
    private int currRow = 0;
    private int currCol = 0;

    //current ring
    private int ring = 0;

    private new void Update()
    {
        // from parent (see Figure class)
        if (!this.onBoard)
        {
            theGame.addFigure(gameObject, startRow, startCol);
            theGame.ActionPhase += Action;
            onBoard = true;
            if(viscosity < 0)
            {
                viscosity = 0;
            }
            viscCounter = viscosity;
        }
        UpdatePos();
        // Listen for Inputs from user: moves the "selected" object to an adjacent cell
        //if (Input.anyKeyDown && health > 0)
        //{
        //    handleInput();
        //}

    }

    // move in the direction of the selection object

    public override void Movement()
    {
        if (this.GridCoords[0] == 0)
        {
            currentDirection = GameBoard.HexDirection.Right;
        }
        else if (this.GridCoords[0] == theGame.TheGameBoard.boardWidth - 1)
        {
            currentDirection = GameBoard.HexDirection.Left;
        }
        theGame.moveDirection(gameObject, currentDirection);
        // reset selectedHex to None
        selectedHex = adjHex.None;

        // remove movement from phase
        theGame.MovementPhase -= Movement;
    }

    /// <summary>
    ///     - This is where the lava's logic of movement should be handled. Every action phase
    ///     it should look for an unfilled hex, then fill it.
    /// </summary>
    public override void Action()
    {
        if(viscCounter != 0)
        {
            viscCounter--;
            return;
        }
        viscCounter = viscosity;

        Debug.Log("Lava Spreads to: " + this.GridCoords[0] + " " + this.GridCoords[1]);

        //Look for neighbors
        int[] neighbor;
        foreach(GameBoard.HexDirection h in Enum.GetValues(typeof(GameBoard.HexDirection)))
        {
            neighbor = theGame.TheGameBoard.getAdjacent(this.gameObject, h);
            if(neighbor == null)
            {
                continue;
            }
            //What type of tile is it?
            HexClass contents = theGame.TheGameBoard._HexGrid[neighbor[0], neighbor[1]];
            //Find unfilled, default neighbors
            //0 is an default tile type?
            if (contents.tileType != -1 && contents.tileType != 2)
            {
                //Fill empty neighbors.
                contents.tileType = 2;
                contents.RebuildStack();
                GameObject newFill = Instantiate(_itemDrop);
                contents.addFigure(newFill);
                //Damage anyone unfortunate enough to get caught in lava
                theGame.TheGameBoard.damageOnHex(neighbor, 999);
            }
        }

        //Damage idiots who walk into/thrown into lava.
        theGame.TheGameBoard.damageOnHex(this.GridCoords, 999);

        //Extra reference code below.
        // remove action from phase
        //theGame.ActionPhase -= Action;
    }

    // check if the player is dead
    public override void Death()
    {
        if (health <= 0)
        {
            theGame.DeathPhase -= Death;
            spawnItem();
            gameObject.SetActive(false);
        }
    }


    public void spawnItem()
    {        
        Vector3 _spawningPos = new Vector3();
        //If the current row is off it is offset by some value.
        //This Instantiates the rows at the top and bottom.
        //Note that the x coordinate is the column
        //This means that the z coordinate is the row
        for (int i = ring + 1; i < theGame.boardWidth - ring - 1; i++)
        {
            if (ring % 2 != 0) //is odd
            {
                //Draw the first side
                Debug.Log("odd row");
                _spawningPos = new Vector3(i * 17f + 5.5f, 2.5f, ring * 15f);
            }
            else
            {
                //Draw first side
                _spawningPos = new Vector3(i * 17f - 2.9f, 2.5f, ring * 15f);
                
            }
        //Spawn item on top of tile.
            _itemDrop = Instantiate(_itemDrop, _spawningPos, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
            theGame.addFigure(_itemDrop, GridCoords[0], GridCoords[1]);
            //Then do the opposite row
            if ((theGame.TheGameBoard.boardLength - ring) % 2 != 0)
            {
                //Debug.Log("odd row");
                _spawningPos = new Vector3(i * 17f - 2f, 2.5f, (theGame.TheGameBoard.boardLength - 1 - ring) * 15f);
            }
            else
            {
                //Debug.Log("even row");
                _spawningPos = new Vector3(i * 17f + 6f, 2.5f, (theGame.TheGameBoard.boardLength - 1 - ring) * 15f);
            }
            _itemDrop = Instantiate(_itemDrop, _spawningPos, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
            theGame.addFigure(_itemDrop, GridCoords[0], GridCoords[1]);
        }
        //The next step is to draw the "ends", on every row.
        //It's 1 for the last and first row, two for the second and second to last, and so on.
        for (int i = ring; i < theGame.boardLength - ring; i++)
        {
            //draw the left end
            if (i % 2 != 0) //is odd
            {
                //Spawn the tile on one side.
                _spawningPos = new Vector3(ring * 17f + 6f, 2.5f, i * 15);
            }
            else
            {
                //Spawn the tile on one side.
                _spawningPos = new Vector3(ring * 17f - 3f, 2.5f, i * 15f);

            }
            _itemDrop = Instantiate(_itemDrop, _spawningPos, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
            theGame.addFigure(_itemDrop, GridCoords[0], GridCoords[1]);

            //draw the right end
            if (i % 2 != 0)
            {
                //Spawn the tile on one side.
                _spawningPos = new Vector3((theGame.TheGameBoard.boardLength - ring) * 17f - 11.5f, 2.5f, i * 15 - 1f);
            }
            else
            {
                //Spawn the tile on one side.
                _spawningPos = new Vector3((theGame.TheGameBoard.boardLength - ring) * 17f - 19.75f, 2.5f, i * 15f - 1f);
            }

            _itemDrop = Instantiate(_itemDrop, _spawningPos, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
            theGame.addFigure(_itemDrop, GridCoords[0], GridCoords[1]);
        }
        ring++;
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
            //theGame.MovementPhase += Movement;

            // trigger roundStart to gameState
            spawnItem();
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
        else if (Input.GetKeyDown(KeyCode.J))
        {
            health = 0;
        }

    }


}
