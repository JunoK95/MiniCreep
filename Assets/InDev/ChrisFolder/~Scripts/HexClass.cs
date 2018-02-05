using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure for a single Hex. Contains all information about that Hex
/// </summary>
public class HexClass:MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> _Contents = new List<GameObject>(); // List of objects "on" the hex (index 0 is the hex tile object)
    public GameObject tile { get { return _Contents[0]; } } // access to the tile itself
    public int[] Address { get { return new int[2] { _GridCoordinates[0], _GridCoordinates[1] }; } } // copy of grid's 2D coordinates [row,col]
    public int height;
    public int tileType;
    public LinkedList<GameObject> TileStack = new LinkedList<GameObject>();
    private bool passable;

   

    private int[] _GridCoordinates = new int[2]; // grid's 2D coordinates [row,col]

    /// <summary>
    /// Creates a Hex Tile Object.  
    /// 
    /// Contains a Grid Address and List of Game objects associated with tile. (index 0 is tile itself)
    /// 
    /// </summary>
    /// <param name="row">Row Coordinate in Grid</param>
    /// <param name="col">Column Coordinate in Grid</param>
    /// <param name="tile">Game Object for Tile</param>
    public void CreateHexClass(int row, int col, GameObject tile)
    {
        _GridCoordinates[0] = row;
        _GridCoordinates[1] = col;
        if (col % 2 != 0) //is odd
        {
            transform.position = new Vector3(row * 17f + 8.5f, 2f, col * 15);
        }
        else
        {
            transform.position = new Vector3(row * 17f, 2f, col * 15f);
        }

    }
    /// <summary>
    /// builds tile stack with given material up to height
    /// </summary>
    private void BuildStack()
    {
        GameObject tileset = GameObject.Find("Board").GetComponent<GameBoard>().Tiles[tileType];
        int i = _GridCoordinates[0];
        int j = _GridCoordinates[1];
        Vector3 _spawningPos = transform.position + new Vector3(0,2f,0);
        
        GameObject _hexTile = null;
        for (int z = 0; z <= height; z++)
        {
            _hexTile = Instantiate(tileset, _spawningPos, Quaternion.Euler(0, 90, 0));
            _hexTile.name = "tile:" + i + "_" + j + "_" + z;
            //_hexTile.GetComponent<TextOnObj>().updateText(i.ToString() + "," + j.ToString());

            _hexTile.transform.parent = gameObject.transform;
            _spawningPos += new Vector3(0f, 2f, 0f);
            TileStack.AddFirst(_hexTile);
        }
        if(height<0)
        {
            passable = false;
        }
        else
        {
            passable = true;
        }
        try
        {
            _Contents[0] = _hexTile;
        }
        catch
        {
            if(_hexTile ==null)
            {
                _Contents.Insert(0, gameObject);
            }
            else
            {
                _Contents.Insert(0, _hexTile);
            }
           
        }
        
    }
    /// <summary>
    /// Clears the tile stack then rebuilds it
    /// </summary>
    public void RebuildStack()
    {
        foreach(GameObject t in TileStack)
        {
            GameObject.Destroy(t);
        }
        BuildStack();
    }
    /// <summary>
    /// Adds a Figure to a HexClass' contents list
    /// </summary>
    /// <param name="piece">Figure to add to HexClass</param>
    public void addFigure(GameObject piece)
    {
        _Contents.Add(piece);
        piece.GetComponent<Figure>().PrevCoords = (int[])piece.GetComponent<Figure>().GridCoords.Clone();
       piece.GetComponent<Figure>().GridCoords = this.Address;
        piece.GetComponent<Figure>().SetMove(_Contents[0]);


    }

    /// <summary>
    /// Removes a Figure from a HexClass' contents list
    /// </summary>
    /// <param name="piece"></param>
    public void removeFigure(GameObject piece)
    {
        if (_Contents.Contains(piece))
        {
            _Contents.Remove(piece);
            piece.GetComponent<Figure>().PrevCoords = this.Address;
        }
    }

    public void removeAll()
    {
        List<GameObject> items = new List<GameObject>();
        for ( int i = 1; i < _Contents.Count; i++)
        {
            items.Add(_Contents[i]);
        }

        foreach(GameObject item in items)
        {
            removeFigure(item);
        }
    }

    public void damageOnHex(int damage)
    {
        if (_Contents.Count < 2)
            return; // nothing to damage here

        for(int i = 1; i < _Contents.Count; i++)
        {
            Figure target = _Contents[i].GetComponent<Figure>();
            if(target != null)
            {
                target.applyDamage(damage);
            }
        }
    }
    public bool GetPassable()
    {
        return passable;
    }

    private void SetPassable(bool value)
    {
        passable = value;
    }




}