using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClass : MonoBehaviour
{
    public GameObject basicTile;
    public int boardWidth = 3;
    public int boardLength = 3;

    private GameObject[,] _listOfHex;
    private GameObject _hexTile;
    private Vector3 _spawningPos;

    // Use this for initialization
    void Start()
    {
        _hexTile = basicTile;
        _listOfHex = new GameObject[boardWidth,boardLength];
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardLength; j++)
            {
                if (j % 2 != 0) //is odd
                {
                    _spawningPos = new Vector3(i * 20f + 10f, 2f, j * 20f);
                }
                else
                {
                    _spawningPos = new Vector3(i * 20f, 2f, j * 20f);
                }
                _listOfHex[i, j] = Instantiate(_hexTile, _spawningPos, Quaternion.Euler(0,90,0));
                _listOfHex[i, j].GetComponent<HexTileClass>().setAddress(i, j);
                _listOfHex[i, j].transform.parent = this.transform;
                _listOfHex[i, j].GetComponent<TextOnObj>().updateText(i.ToString() + "," + j.ToString());
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject[,] getHexList()
    {
        return _listOfHex;
    }
}
