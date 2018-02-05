using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour {

    public int char_x;
    public int char_z;
    public int char_atk;
    public int char_speed;
    public int char_def;
    public int[] char_equip;
    public int char_mass;

	// Use this for initialization
	void Start () {
        this.char_x = 0;
        this.char_z = 0;
        this.char_atk = 0;
        this.char_speed = 0;
        this.char_def = 0;
        this.char_mass = 1;
        this.char_equip = new int[5];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateCharPosition(GameObject hexTile)
    {
        this.transform.position = new Vector3(hexTile.transform.position.x,4,hexTile.transform.position.z);
        char_x = hexTile.GetComponent<HexTileClass>().getAddress()[0];
        char_z = hexTile.GetComponent<HexTileClass>().getAddress()[1];
    }

    public void updateCharPosition(float x, float y)
    {

    }

    public void doAction(int action, GameObject hexTile)
    {
                
    }


}
