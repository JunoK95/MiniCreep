using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bsmapeditor : MonoBehaviour {
    public GameBoard gb;
    private HexClass hex;
    public InputField x;
    public InputField y ;
    public Slider h;
    public Slider t;
    public Camera camera;
	// Use this for initialization
	void Start () {
        //  t.maxValue = gb.Tiles.Length;
       
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log("try");
                Transform objectHit = hit.transform;
                Debug.Log(objectHit.gameObject.name);
                if(objectHit.gameObject.name.StartsWith("pCy"))
                    {
                    
                    HexClass c = objectHit.gameObject.transform.parent.parent.gameObject.GetComponent<HexClass>();
                    Debug.Log(c.Address[0]);
                    x.text = c.Address[0]+"";
                    y.text = c.Address[1] + "";
                    hex = c;
                    transform.position = hex._Contents[0].transform.position + new Vector3(0, 5f, 0);
                }

                // Do something with the object that was hit by the raycast.
            }
        }


           
        t.value = hex.tileType;
        h.value = hex.height;

    }
    void OnMouseDown()
    {
       
    }
    private void FixedUpdate()
    {
        try
        {
            hex = gb._HexGrid[int.Parse(x.text), int.Parse(y.text)];
            transform.position = hex._Contents[0].transform.position + new Vector3(0, 5f, 0);
        }
        catch
        {

        }
    }
    public void Changeselect()
    {
        hex = gb._HexGrid[int.Parse(x.text),int.Parse( y.text)];
        
    }
    public void Altertile()
    {
       hex.tileType =(int) t.value;
        hex.height = (int)h.value ;
        hex.RebuildStack();
    }
}
