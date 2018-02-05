using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private GameObject _currentHexTile;
    private int _currentx;
    private int _currentz;
    private GameObject _board;
    private GameObject _player;
    private GameObject _gameState;
	// Use this for initialization
	void Start () {
        _currentx = 0;
        _currentz = 0;
        _board = GameObject.Find("Board");
        _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx,_currentz];
        _player = GameObject.Find("Character");
        _gameState = GameObject.Find("GameState");
        Debug.Log(_player);
    }
	
	// Update is called once per frame
	void Update () {

        if(_board==null)
        {
            _board = GameObject.Find("Board");
            _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx, _currentz];
            
        }
        if(_player==null)
        {
            _player = GameObject.Find("Character");
        }

        if (Input.GetKeyDown("right"))
        {
            if (_currentx < _board.GetComponent<BoardClass>().boardWidth - 1)
            {
                _currentx += 1;
            }
            _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx, _currentz];
            _player.GetComponent<CharacterClass>().updateCharPosition(_currentHexTile);
            Debug.Log(_currentx.ToString() + "," + _currentz.ToString());
        }
        if (Input.GetKeyDown("left"))
        {
            if (_currentx > 0)
            {
                _currentx -= 1;
            }
            _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx, _currentz];
            _player.GetComponent<CharacterClass>().updateCharPosition(_currentHexTile);
            Debug.Log(_currentx.ToString() + "," + _currentz.ToString());
        }
        if (Input.GetKeyDown("up"))
        {
            if (_currentz < _board.GetComponent<BoardClass>().boardLength - 1)
            {
                _currentz += 1;
            }
            _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx, _currentz];
            _player.GetComponent<CharacterClass>().updateCharPosition(_currentHexTile);
            Debug.Log(_currentx.ToString() + "," + _currentz.ToString());
        }
        if (Input.GetKeyDown("down"))
        {
            if (_currentz > 0)
            {
                _currentz -= 1;
            }
            _currentHexTile = _board.GetComponent<BoardClass>().getHexList()[_currentx, _currentz];
            _player.GetComponent<CharacterClass>().updateCharPosition(_currentHexTile);
            Debug.Log(_currentx.ToString() + "," + _currentz.ToString());
        }
    }
}
