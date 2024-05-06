using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolsController : MonoBehaviour
{
    [SerializeField] Tools activeTool;
    [SerializeField] GridManager manager;

    [Header("Select Tool")]
    [SerializeField] GameObject selectionSquare = null;
    //Vector3 nullPosition = new Vector3(-1000, -1000, -1000);
    Vector3? nullPosition = null;
    Vector3 startPosition;

    [Header("Drag Tool")]
    [SerializeField] float dragSpeed = 1f;
    [SerializeField] float zoomSpeed = 1f;
    Vector3 prevMousePosition;

    private void Awake()
    {
        startPosition = nullPosition.Value;
        prevMousePosition = nullPosition.Value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (activeTool)
        {
            case Tools.None:
                break;

            case Tools.SelectTool:
                SelectToolFunctions();
                break;

            case Tools.DragTool:
                DragToolFunctions();
                break;
        }
        ZoomFunctions();
    }



    #region TOOL FUNCTIONS
    //Drag across the screen to move the camera
    void SelectToolFunctions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPosition = new Vector3(Input.mousePosition.x / Camera.main.pixelWidth, Input.mousePosition.y / Camera.main.pixelHeight);
        }

        if (Input.GetKey(KeyCode.Mouse0) && startPosition != nullPosition && selectionSquare != null)
        {
            Vector3 currentPosition = new Vector3(Input.mousePosition.x / Camera.main.pixelWidth, Input.mousePosition.y / Camera.main.pixelHeight);
            Bounds bounds = new Bounds();
            float minX = Math.Min(startPosition.x, currentPosition.x);
            float maxX = Math.Max(startPosition.x, currentPosition.x);
            float minY = Math.Min(startPosition.y, currentPosition.y);
            float maxY = Math.Max(startPosition.y, currentPosition.y);

            selectionSquare.GetComponent<RectTransform>().anchorMin = new Vector2(minX, minY);
            selectionSquare.GetComponent<RectTransform>().anchorMax = new Vector2(maxX, maxY);

        }
        else if (startPosition != nullPosition)
        {
            startPosition = nullPosition.Value;
            selectionSquare.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            selectionSquare.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);

            Vector3 size = manager.GetMap().cellSize;
            Vector2 dimensions = manager.GetDimensions();
            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    //if()
                }
            }
        }
    }

    //Drag across the screen to move the camera
    void DragToolFunctions()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 position = Input.mousePosition;
            if (prevMousePosition != nullPosition)
            {
                Camera.main.transform.position -= (position - prevMousePosition) * dragSpeed * Camera.main.orthographicSize / 5;
            }
            prevMousePosition = position;
        }
        else if (prevMousePosition != nullPosition)
        {
            prevMousePosition = nullPosition.Value;
        }
    }

    //Zoom In/Out
    void ZoomFunctions()
    {
        if (Input.mouseScrollDelta.y != 0f)
        {
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
            if (Camera.main.orthographicSize < 1)
            {
                Camera.main.orthographicSize = 1;
            }
        }
    }
    #endregion

    #region INTERACTION FUNCTIONS
    //Activate a specific Tool (requires int because of Unity shenanigans)
    public void ActivateTool(int t)
    {
        activeTool = (Tools) t;
        Debug.Log(t + " " + (Tools)t + " " + activeTool);
    }
    #endregion

    Bounds MakeBounds(Vector3 pos1, Vector3 pos2)
    {
        Bounds bounds = new Bounds();
        return bounds;
    }
}

[Serializable]
public enum Tools
{
    None = 0,
    SelectTool = 1,
    DragTool = 2
}
