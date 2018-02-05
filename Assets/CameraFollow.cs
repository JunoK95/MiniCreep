using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    //Transform to follow
    public Transform toFollow;

    //offset that stores the offset distance between the player and the camera.
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        //Calculate and store the offset value by getting the distance between the object and the camera
        offset = transform.position - toFollow.transform.position;
	}

	void LateUpdate () {
        //Changing the cameras position
        transform.position = toFollow.transform.position + offset;
	}
}
