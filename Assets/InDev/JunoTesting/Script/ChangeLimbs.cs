using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLimbs : MonoBehaviour {

    public int[] equipmentList = new int[5];

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void replaceLimb(int limb, int equipmentID)
    {
        equipmentList[limb] = equipmentID;
        GameObject b = GameObject.Find(equipmentID.ToString());

    }
}
