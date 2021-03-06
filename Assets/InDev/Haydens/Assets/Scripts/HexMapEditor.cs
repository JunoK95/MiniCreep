﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    int activeElevation;

	public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void SetElevation (float elevation)
    {
        activeElevation = (int)elevation;
    }

    void Awake()
    {
        SelectColor(0);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()
            )
        {
            HandleInput();
        }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    /// <summary>
    /// This method will take care of all of the editing of a cell.
    /// </summary>
    /// <param name="cell"></param>
    void EditCell(HexCell cell)
    {
        cell.color = activeColor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh();
    }
}
