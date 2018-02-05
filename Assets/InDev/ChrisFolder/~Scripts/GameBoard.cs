using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameBoard Class contains the 2D grid array of Hex Class Objects. 
///     - Contains helper functions: moveFigure
/// </summary>
public class GameBoard : MonoBehaviour
{
    GameObject _hexTile;
    public GameObject[] Tiles = new GameObject[1];
    //public GameObject[,]
    public enum HexDirection { Left, UpLeft, UpRight, Right, DownRight, DownLeft }
    public string file = "map.csv";

    public int boardWidth = 4, boardLength = 6;

    public HexClass[,] _HexGrid;

    /// <summary>
    /// Create the Grid here. Using the hexTilePrefab
    /// </summary>
    internal void generateGrid(Transform parent,GameObject hexTilePrefab = null)
    {
        if(hexTilePrefab ==null)
        {
            hexTilePrefab = Tiles[0];
        }
        _HexGrid = new HexClass[boardWidth, boardLength];

        for (int j = 0; j < boardLength; j++)
        {
            for (int i = 0; i < boardWidth; i++)
            {
                
                GameObject slot = new GameObject("Grid:" + i + "_" + j);
                _HexGrid[i, j] = slot.AddComponent<HexClass>();
               
                _HexGrid[i, j].height = 1;
                _HexGrid[i, j].tileType = 0;
                slot.transform.parent = parent;
                _HexGrid[i, j].RebuildStack();
                _HexGrid[i, j].CreateHexClass(i, j, _HexGrid[i,j].TileStack.First.Value);
            }
        }
    }
    
    internal void LoadGrid(Transform parent, GameObject hexTilePrefab = null)
    {
        
        Debug.Log(System.IO.Directory.GetCurrentDirectory());

        string doc = System.IO.Directory.GetCurrentDirectory() + "\\" + "Assets";
        //Directory.CreateDirectory(doc);
        System.IO.StreamReader readfile = null;
        try
        {
            Debug.Log("try " + doc + "\\" + file);
            readfile = new System.IO.StreamReader(doc + "\\" +file);
            Debug.Log("open");
            string line;
            line = readfile.ReadLine();
            Debug.Log(line);
            string[] parts = line.Split(',');
            boardWidth = int.Parse(parts[0]);
            boardLength = int.Parse(parts[1]);
            _HexGrid = new HexClass[boardWidth, boardLength];
            for (int i = 0; i < boardWidth; i++)
            {
                Debug.Log(i);
                if ((line = readfile.ReadLine()) != null)
                {
                    parts = line.Split(',');
                    for (int j = 0; j < boardLength; j++)
                    {
                       
                        string[] tparts = parts[j].Split(':');
                        int t = int.Parse(tparts[0]);
                        int h = int.Parse(tparts[1]);
                        GameObject slot = new GameObject("Grid:" + i + "_" + j);
                        _HexGrid[i, j] = slot.AddComponent<HexClass>();
                        _HexGrid[i, j].CreateHexClass(i, j, slot);
                        _HexGrid[i, j].height = h;
                        _HexGrid[i, j].tileType = t;
                        slot.transform.parent = parent;
                        _HexGrid[i, j].RebuildStack();
                        


                    }
                }

            }

            readfile.Close();
        }
        catch (Exception e)
        {
            //readfile.Close();
            Debug.Log(e.Message);
            Debug.Log("catch");
            if(hexTilePrefab==null)
            {
                generateGrid(parent);
            }
            else
            {
                generateGrid(parent, hexTilePrefab);
            }
           
        }
    }

    internal void damageOnHex(int[] hexCoords, int damage)
    {
        try
        {
            _HexGrid[hexCoords[0], hexCoords[1]].damageOnHex(damage);
        }
        catch
        {
            Debug.Log("Failed to apply damage to Hex");
        }
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
        int[] GridCoords = new int[2] { row, col };
        if (GridCoords.Length == 2 && piece.GetComponent<Figure>() != null)
        {
            HexClass tile = _HexGrid[GridCoords[1], GridCoords[0]];
            if (tile._Contents.Count == 1)
            {
                tile.addFigure(piece);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Move a given figure to the given hex
    /// </summary>
    /// <param name="piece">the piece that needs to move</param>
    /// <param name="newCoords">where to place the piece</param>
    public void moveToHex(GameObject piece, int[] newCoords)
    {
        int[] current = piece.GetComponent<Figure>().GridCoords;
        _HexGrid[current[0], current[1]].removeFigure(piece);
        _HexGrid[newCoords[0], newCoords[1]].addFigure(piece);
    }

    public void clearHex(int[] target)
    {
        _HexGrid[target[0], target[1]].removeAll();
    }

    /// <summary>
    /// Move a given figure to the hex in the given direction
    /// </summary>
    /// <param name="piece">the piece that needs to move</param>
    /// <param name="dir">which way to move the piece</param>
    public void moveDirection(GameObject piece, HexDirection dir)
    {
        int[] newCoords = getAdjacent(piece, dir);
        if (newCoords != null)
        {
            moveToHex(piece, newCoords);
        } // else do nothing
    }

    /// <summary>
    /// Gets the adjacent tile in the given direction from the given piece
    /// </summary>
    /// <param name="piece">Piece to reference</param>
    /// <param name="dir">direction of adjacent tile desired</param>
    /// <returns></returns>
    public int[] getAdjacent(GameObject piece, HexDirection dir)
    {
        int[] current = piece.GetComponent<Figure>().GridCoords;
        int[] newCoords = null;

        switch (dir)
        {
            case HexDirection.Left:
                newCoords = new int[2] { current[0] - 1, current[1] };
                break;
            case HexDirection.UpLeft:
                newCoords = new int[2] { current[0] - 1, current[1] + 1 };
                if (newCoords[1] % 2 == 0) { newCoords[0]++; }
                break;
            case HexDirection.UpRight:
                newCoords = new int[2] { current[0], current[1] + 1 };
                if (newCoords[1] % 2 == 0) { newCoords[0]++; }
                break;
            case HexDirection.Right:
                newCoords = new int[2] { current[0] + 1, current[1] };
                break;
            case HexDirection.DownRight:
                newCoords = new int[2] { current[0] + 1, current[1] - 1 };
                if (newCoords[1] % 2 == 1) { newCoords[0]--; }
                break;
            case HexDirection.DownLeft:
                newCoords = new int[2] { current[0], current[1] - 1 };
                if (newCoords[1] % 2 == 1) { newCoords[0]--; }
                break;
        }

        // if out of bounds, do nothing
        if (newCoords[0] < 0 || newCoords[0] >= boardWidth || newCoords[1] < 0 || newCoords[1] >= boardLength) { return null; }
        // if Tile is not Passible; do nothing
        if (!_HexGrid[newCoords[0], newCoords[1]].GetPassable()) { return null; }
        return newCoords;
    }

    /// <summary>
    /// Scan through all hexes in the grid and resolve any conflicts found
    ///     (intended for use after movement Phase)
    /// </summary>
    public void resolveConflicts()
    {
        // look through all hexTiles in _HexGrid
        foreach (HexClass tile in _HexGrid)
        {
            if (tile._Contents.Count > 2)
            {   // Conflict Found, try and resolve it
                Figure.figSize biggest = Figure.figSize.SMALL;
                int[] sizeCount = new int[3] { 0, 0, 0 };

                foreach (GameObject piece in tile._Contents)
                { // First find the biggest figure
                    Figure pieceScript = piece.GetComponent<Figure>();
                    if (pieceScript != null && !(pieceScript is FillEnemyScript))
                    { // piece IS A Figure!
                        switch (pieceScript._SIZE)
                        {
                            case Figure.figSize.SMALL:
                                sizeCount[0]++;
                                break;
                            case Figure.figSize.MEDIUM:
                                sizeCount[1]++;
                                break;
                            case Figure.figSize.LARGE:
                                sizeCount[2]++;
                                break;
                        }
                    }
                }
                
                foreach (GameObject piece in tile._Contents)
                { // mark all pieces, smaller or equal to biggest size as "bounceBack"
                    Figure pieceScript = piece.GetComponent<Figure>();
                    if (pieceScript != null)
                    { // piece IS A Figure!
                        switch (pieceScript._SIZE)
                        {
                            case Figure.figSize.SMALL:
                                pieceScript.NeedBounceBack();
                                break;
                            case Figure.figSize.MEDIUM:
                                if(sizeCount[1] > 1 || sizeCount[2] > 0)
                                {
                                    pieceScript.NeedBounceBack();
                                }
                                break;
                            case Figure.figSize.LARGE:
                                if(sizeCount[2] > 1)
                                {
                                    pieceScript.NeedBounceBack();
                                }
                                break;
                        }
                    }
                }

            }
        }
    }

    Color HexToColor(string hex)
    {
        if (hex.Length == 3)
        {
            hex = hex.Substring(0, 1) + hex.Substring(0, 1) + hex.Substring(1, 1) + hex.Substring(1, 1) + hex.Substring(2, 1) + hex.Substring(2, 1);
        }
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

}
