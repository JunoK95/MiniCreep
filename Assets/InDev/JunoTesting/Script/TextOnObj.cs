using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOnObj : MonoBehaviour {

    private TextMesh textBox;

	// Use this for initialization
	void Start () {
        textBox = this.GetComponentInChildren<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateText(string text)
    {
        this.GetComponentInChildren<TextMesh>().text = text;
    }
}
