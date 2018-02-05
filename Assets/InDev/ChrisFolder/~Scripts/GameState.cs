using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chris: Singleton Game State Manager. Contains a reference to the Gameboard, the List of Figures, etc...
///    - Contains Events for figures to subscribe to (ie: takeAction, checkDeath) to manage gameflow from round to round 
/// </summary>
public class GameState : MonoBehaviour
{

    [SerializeField]
    public GameObject HexTilePrefab;
    [SerializeField]
    public int boardWidth = 4, boardLength = 6;

    public delegate void ActionEvent(); // defining function signature for action phase
    public event ActionEvent MovementPhase; // Figures subscribe to this with the movement they'd like to envoke for the round
    public event ActionEvent BounceBack;    // Figures that collide may need to bounce back
    public event ActionEvent ActionPhase; // Figures subscribe to this with the action function they'd like to envoke for the round
    public event ActionEvent DeathPhase; // Figures MUST subscribe to this event, informs figures to check if they've died
                                         //Paul: I added a sperate movement so that activates first

    public GameBoard TheGameBoard;

    [HideInInspector]
    public bool phaseRunning = false, ismoving = false, isaction = false;
    [HideInInspector]
    public int phaseTimer = 0, phaseDelay = 20;

    #region unity

    // Chris: Instantiate a new Game Board
    void Start()
    {
        GameObject board;
        if (TheGameBoard ==null)
        {
            board = new GameObject("Board");


            TheGameBoard = board.AddComponent<GameBoard>();
        }
        else
        {
            board = TheGameBoard.gameObject;
        }


        TheGameBoard.boardWidth = this.boardWidth;
        TheGameBoard.boardLength = this.boardLength;

        TheGameBoard.LoadGrid(board.transform, HexTilePrefab);

    }

    // Chris: debug update function. The phase sequence will be activated by the player or the timer.
    void Update()
    {
        // This needs to be replaced with an even subscription (to user selection or timer expire)
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.A)) && !phaseRunning)
        {
            phaseRunning = true;
        }

        if (phaseRunning)
        {
            startPhases();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Evoke when phases need to begin
    /// </summary>
    public void startPhases()
    {
        if (!ismoving)
        {
            ismoving = true;
            if (MovementPhase != null)
            {
                MovementPhase(); // activate all movement requests
                TheGameBoard.resolveConflicts(); // resolve conflicts

                if (BounceBack != null) // bounce back if there were conflicts
                {
                    BounceBack();
                }
                phaseTimer = phaseDelay; // milliseconds
            }
        }

        if (phaseTimer < 1 && ismoving && !isaction)
        {
            isaction = true;
            if (ActionPhase != null)
            {
                ActionPhase(); // activate all action requests
                phaseTimer = 10; // milliseconds
            }
        }

        if (phaseTimer < 1 && ismoving && isaction)
        {
            if (DeathPhase != null)
            {
                DeathPhase(); // all figures need to check if they've died
            }
            ismoving = false;
            isaction = false;
            phaseRunning = false;
        }
        phaseTimer -= 1;
        if (phaseTimer < 0)
            phaseTimer = 0;
    }

    /// <summary>
    /// Cause damage to specified Hex Tile
    /// </summary>
    /// <param name="HexCoords">coordinates for target hex</param>
    /// <param name="damage"> amount of damage to do to hex contents</param>
    internal void damageOnHex(int[] HexCoords, int damage)
    {
        TheGameBoard.damageOnHex(HexCoords, damage);
    }

    /// <summary>
    /// Add an object with a Figure component to the Grid
    ///     - intended for startup only, not for moving figures
    /// </summary>
    /// <param name="piece">piece to add to the game board</param>
    /// <param name="GridCoords">Grid Coordinates to place the piece</param>
    /// <returns>True if the figure was successfully added to the hex tile</returns>
    public bool addFigure(GameObject piece, int row, int col)
    {
        // forward request to game board
        return TheGameBoard.addFigure(piece, row, col);
    }

    /// <summary>
    /// Move a given figure to the given hex
    /// </summary>
    /// <param name="piece">the piece that needs to move</param>
    /// <param name="HexCoords">where to place the piece</param>
    public void moveToHex(GameObject piece, int[] HexCoords)
    {
        // forward request to game board
        TheGameBoard.moveToHex(piece, HexCoords);
    }

    /// <summary>
    /// Move a given figure to the hex in the given direction
    /// </summary>
    /// <param name="piece">the piece that needs to move</param>
    /// <param name="dir">which way to move the piece</param>
    public void moveDirection(GameObject piece, GameBoard.HexDirection dir)
    {
        // forward request to game board
        TheGameBoard.moveDirection(piece, dir);
    }

    #endregion

}

