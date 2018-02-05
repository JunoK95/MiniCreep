using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileClass : MonoBehaviour {

    public bool isHighlighted;

    private GameObject _board;
    private int[] _address = new int[2];


	// Use this for initialization
	void Start () {
        isHighlighted = false;
        _board = GameObject.Find("Board");	
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void UpdateHexHighlight(bool isLit)
    {
        if (isLit)
        {

        }
        else
        {

        }
    }

    public int[] getAddress()
    {
        return _address;
    }

    public void setAddress(int x, int z)
    {
        _address[0] = x;
        _address[1] = z;
    }

}
