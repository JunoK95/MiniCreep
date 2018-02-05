using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour {

    public string itemName;
    public int limbPos = 0;
	// Use this for initialization
	void Start () {
		if (itemName == null)
        {
            itemName = "stick";
        }
        if (limbPos == null)
        {
            limbPos = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider player)
    {
        Debug.Log("collision");
        //player.gameObject.GetComponent<PlayerFigure>().updateItems(this.itemName, this.limbPos);
        if (player.gameObject.tag == "Player")
            Destroy(this.gameObject);
    }
}
