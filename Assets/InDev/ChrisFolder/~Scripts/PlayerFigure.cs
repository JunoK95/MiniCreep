using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Figure Script: handles player input and converts them into commands to the gameState
///     - additionally, the player 'instantiates' a selected cell object to indicate where there player ouls like to move
/// </summary>
public class PlayerFigure : Figure {
    [SerializeField]
    private GameObject _Selector; // This is the object that indicates the players selection

    public GameObject _attackPrefab = null;
    public string[] equipmentList = new string[5];
    public bool testEquipment = false;

    // Player needs a "selected" tile object
    private adjHex selectedHex = adjHex.None;

    private new void Update()
    {
        // from parent (see Figure class)
        if (!this.onBoard)
        {
            theGame.addFigure(gameObject, startRow, startCol);
            theGame.DeathPhase += this.Death;
            onBoard = true;

            _Selector = Instantiate(_Selector, gameObject.transform.position, new Quaternion(0.0f, 1.0f, 0.0f, Mathf.Deg2Rad * 15.0f));
            _Selector.SetActive(false); // hide the selector for now
        }
        UpdatePos();
        // Listen for Inputs from user: moves the "selected" object to an adjacent cell
        if (Input.anyKeyDown && health > 0)
        {
            handleInput();
        }

    }


    // move in the direction of the selection object
    public override void Movement()
    {
        switch (selectedHex)
        {
            case adjHex.Left:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.Left);
                break;
            case adjHex.UpLeft:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.UpLeft);
                break;
            case adjHex.UpRight:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.UpRight);
                break;
            case adjHex.Right:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.Right);
                break;
            case adjHex.DownRight:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.DownRight);
                break;
            case adjHex.DownLeft:
                theGame.moveDirection(gameObject, GameBoard.HexDirection.DownLeft);
                break;
        }

        // reset selectedHex to None
        selectedHex = adjHex.None;
        moveSelection();

        // remove movement from phase
        theGame.MovementPhase -= Movement;
    }

    /// <summary>
    ///     - determine which hex gets attacked
    ///     - attack that hexTile
    /// </summary>
    public override void Action()
    {
        Debug.Log("Ker-POW!! " + gameObject.name + " attacked " + selectedHex);
        int[] targetHex = directionToCoords(selectedHex);
        theGame.damageOnHex(targetHex, 1);
        Transform targetPosition =  theGame.TheGameBoard._HexGrid[targetHex[0], targetHex[1]]._Contents[0].transform;

        GameObject slam = Instantiate(_attackPrefab, targetPosition);
        GameObject.Destroy(slam, 5);

        // reset selectedHex to None
        selectedHex = adjHex.None;
        moveSelection();

        // remove action from phase
        theGame.ActionPhase -= Action;
    }

    // check if the player is dead
    public override void Death()
    {
        if (health <= 0)
        {
            theGame.DeathPhase -= Death;

            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// limbIndex. 1:Head, 2:L Arm, 3:R Arm, 4:Torso, 5:Legs
    /// 
    /// </summary>
    public void updateItems(string itemName, int limbIndex)
    {
        equipmentList[limbIndex] = itemName;
        Debug.Log(itemName + " in " + limbIndex.ToString());
    }

    public void updateItems(int itemID, int limbIndex)
    {
        equipmentList[limbIndex] = itemID.ToString();
        Debug.Log(itemID + " in " + limbIndex.ToString());
    }

    /// <summary>
    /// If the selection changes, move the "select" object
    ///     - this is done by digging into the gameBoard to determine the coordinates for the selected hex
    /// </summary>
    private void moveSelection()
    {
        Vector3 centerPos = MoveTo;
        _Selector.SetActive(true);

        switch (selectedHex)
        {
            case adjHex.None:
                _Selector.SetActive(false);
                break;
            case adjHex.Left:
                _Selector.transform.position = new Vector3(centerPos.x - 17.0f, centerPos.y-2f, centerPos.z);
                break;
            case adjHex.UpLeft:
                _Selector.transform.position = new Vector3(centerPos.x - 8.5f, centerPos.y - 2f, centerPos.z + 15);
                break;
            case adjHex.UpRight:
                _Selector.transform.position = new Vector3(centerPos.x + 8.5f, centerPos.y - 2f, centerPos.z + 15);
                break;
            case adjHex.Right:
                _Selector.transform.position = new Vector3(centerPos.x + 17.0f, centerPos.y - 2f, centerPos.z);
                break;
            case adjHex.DownRight:
                _Selector.transform.position = new Vector3(centerPos.x + 8.5f, centerPos.y - 2f, centerPos.z - 15);
                break;
            case adjHex.DownLeft:
                _Selector.transform.position = new Vector3(centerPos.x - 8.5f, centerPos.y - 2f, centerPos.z - 15);
                break;
        }
        bool hit = true;
        
        while (hit)
        {
            hit = false;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f);
            foreach(Collider c in hitColliders)
            {   
                if(c.gameObject.name.Contains("pcyl"))
                {
                    _Selector.transform.position += new Vector3(0, 1f, 0);
                    hit = true;
                }
            }

        }
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

            // trigger roundStart to gameState
        }
        // "A" key indicates an attack request
        else if (Input.GetKeyDown(KeyCode.A) && selectedHex != adjHex.None)
        {
            // register the action to the gameState
            theGame.ActionPhase += Action;

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
            theGame.TheGameBoard.clearHex(GridCoords);
            theGame.DeathPhase -= Death;
            gameObject.SetActive(false);
        }

        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (testEquipment)
            {
                for (int i = 0; i <= equipmentList.Length - 1; i++)
                {
                    GameObject limb = Resources.Load(equipmentList[i]) as GameObject;
                    Instantiate(Resources.Load(equipmentList[i]) as GameObject,this.gameObject.transform);
                }
            }
        }
        moveSelection();

    }

}
