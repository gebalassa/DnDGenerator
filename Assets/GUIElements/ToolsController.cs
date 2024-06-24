using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ToolsController : MonoBehaviour
{
    [SerializeField] Tools activeTool;
    [SerializeField] GridManager gridManager;
    [SerializeField] ManagerReferences references;

    [Header("Select Tool")]
    SelectToolFunction selectDoing = SelectToolFunction.None;
    [SerializeField] GameObject selectionSquare = null;
    Vector3? startPosition;
    [SerializeField] GameObject assetPreview = null;
    DraggableAsset assetSelected = null;

    [Header("Drag Map Tool")]
    [SerializeField] float dragSpeed = 1f;
    [SerializeField] float zoomSpeed = 1f;
    Vector3? prevMousePosition;

    private void Awake()
    {
        startPosition = null;
        prevMousePosition = null;
    }


    void Update()
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
        EraseFunctions();
    }



    #region TOOL FUNCTIONS
    //Drag across the screen to move the camera
    void SelectToolFunctions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPosition = new Vector3(Input.mousePosition.x / Camera.main.pixelWidth, Input.mousePosition.y / Camera.main.pixelHeight);

            List<RaycastResult> results = MouseRaycast();
            if(results.Count == 0) 
            {
                selectDoing = SelectToolFunction.SelectingTiles;
            }
            else
            {
                foreach(RaycastResult result in results)
                {
                    if(result.gameObject.GetComponentInParent<DraggableAsset>() != null)
                    {
                        assetSelected = result.gameObject.GetComponentInParent<DraggableAsset>();
                        assetPreview.GetComponent<Image>().sprite = assetSelected.Thumbnail();
                        selectDoing = SelectToolFunction.DraggingAsset;
                    }
                }
            }
        }

        switch (selectDoing)
        {
            case SelectToolFunction.SelectingTiles:
                SelectingTiles();
                break;
            case SelectToolFunction.DraggingAsset:
                DraggingAsset();
                break;
        }
    }

    //Drag across the screen to move the camera
    void DragToolFunctions()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 position = Input.mousePosition;
            if (prevMousePosition != null)
            {
                Camera.main.transform.position -= (position - prevMousePosition.Value) * dragSpeed * Camera.main.orthographicSize / 5;
            }
            prevMousePosition = position;
        }
        else if (prevMousePosition != null)
        {
            prevMousePosition = null;
        }
    }

    //Put walls in the map
    void WallFunctions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPosition = new Vector3(Input.mousePosition.x / Camera.main.pixelWidth, Input.mousePosition.y / Camera.main.pixelHeight);

            List<RaycastResult> results = MouseRaycast();
            if (results.Count == 0)
            {

            }
            else
            {
                foreach (RaycastResult result in results)
                {
                }
            }
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

    //Erase content from assetsMap
    void EraseFunctions()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            for(int i = 0; i < gridManager.GridClass().width; i++)
            {
                for (int j = 0; j < gridManager.GridClass().height; j++)
                {
                    if (gridManager.GridClass().Grid[i, j].selected)
                    {
                        gridManager.EraseAssetTile(i,j);
                    }
                }
            }
        }
    }
    #endregion

    #region SELECT TOOL FUNCTIONS
    void SelectingTiles()
    {
        if (Input.GetKey(KeyCode.Mouse0) && startPosition != null && selectionSquare != null)
        {
            Vector3 currentPosition = new Vector3(Input.mousePosition.x / Camera.main.pixelWidth, Input.mousePosition.y / Camera.main.pixelHeight);

            float minX = Math.Min(startPosition.Value.x, currentPosition.x);
            float maxX = Math.Max(startPosition.Value.x, currentPosition.x);
            float minY = Math.Min(startPosition.Value.y, currentPosition.y);
            float maxY = Math.Max(startPosition.Value.y, currentPosition.y);

            selectionSquare.GetComponent<RectTransform>().anchorMin = new Vector2(minX, minY);
            selectionSquare.GetComponent<RectTransform>().anchorMax = new Vector2(maxX, maxY);

        }
        else if (startPosition != null)
        {
            //Turn screen position to world position
            Camera cam = Camera.main;
            Vector3 worldStartPosition = cam.ScreenToWorldPoint(new Vector3(startPosition.Value.x * Camera.main.pixelWidth, startPosition.Value.y * Camera.main.pixelHeight));
            Vector3 worldCurrentPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            //Create Bounds of select box
            float minX = Math.Min(worldStartPosition.x, worldCurrentPosition.x);
            float maxX = Math.Max(worldStartPosition.x, worldCurrentPosition.x);
            float minY = Math.Min(worldStartPosition.y, worldCurrentPosition.y);
            float maxY = Math.Max(worldStartPosition.y, worldCurrentPosition.y);

            Bounds bounds = new Bounds(new Vector3((minX + maxX) / 2, (minY + maxY) / 2), new Vector3((maxX - minX), (maxY - minY)));

            Tilemap map = gridManager.GetBackgroundMap();
            Vector2 dimensions = gridManager.GetDimensions();

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    Vector3 tilePosition = map.CellToWorld(new Vector3Int(i, j));
                    bool isInside = tilePosition.x >= minX && tilePosition.x <= maxX && tilePosition.y >= minY && tilePosition.y <= maxY;

                    if(isInside)
                    {
                        gridManager.ChangeTileState(i, j, true);
                    }
                    else
                    {
                        gridManager.ChangeTileState(i, j, false);
                    }
                }
            }

            //Update the view of the grid
            gridManager.PaintBackgroundMap();

            //Reset select square dimensions
            selectionSquare.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            selectionSquare.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);

            //Reset aux values
            startPosition = null;
            selectDoing = SelectToolFunction.None;
        }
    }

    void DraggingAsset()
    {
        if (Input.GetKey(KeyCode.Mouse0) && startPosition != null && assetPreview != null && assetSelected != null)
        {
            float tilePixelSize = Camera.main.pixelWidth / (3.8f * Camera.main.orthographicSize);

            Vector3 currentPosition = Input.mousePosition;
            Vector3 maxPosition = currentPosition + new Vector3(assetSelected.Columns() * tilePixelSize, assetSelected.Rows() * tilePixelSize);

            Vector3 normCurrentPosition = new Vector3(currentPosition.x / Camera.main.pixelWidth, currentPosition.y / Camera.main.pixelHeight);
            Vector3 normMaxPosition = new Vector3(maxPosition.x / Camera.main.pixelWidth, maxPosition.y / Camera.main.pixelHeight);
            Vector3 offset = new Vector3((normCurrentPosition.x - normMaxPosition.x) / 2, (normCurrentPosition.y - normMaxPosition.y) / 2);

            assetPreview.GetComponent<RectTransform>().anchorMin = normCurrentPosition + offset;
            assetPreview.GetComponent<RectTransform>().anchorMax = normMaxPosition + offset;

        }
        else if (startPosition != null)
        {
            //Reset asset preview dimensions
            assetPreview.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            assetPreview.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);

            //Reset aux values
            startPosition = null;
            selectDoing = SelectToolFunction.None;

            //Get grid position
            Tilemap map = gridManager.GetBackgroundMap();
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int centerGridPos = map.WorldToCell(worldMousePos);

            //1-1 case
            if (assetSelected.Columns() == 1 && assetSelected.Rows() == 1)
            {
                //Skip out of range cases
                if (centerGridPos.x < 0 || centerGridPos.x > gridManager.GetDimensions().x ||
                    centerGridPos.y < 0 || centerGridPos.y > gridManager.GetDimensions().y)
                {
                    Debug.LogWarning("Asset placement out of range");
                }
                //Place image in tile
                else
                {
                    gridManager.PaintAssetTile(centerGridPos.x, centerGridPos.y, assetSelected.ImageClass());
                }
            }
            //other cases
            else
            {
                //See if the image will need an adjust (for usability)
                bool xEven = assetSelected.Columns() % 2 == 0;
                bool yEven = assetSelected.Rows() % 2 == 0;

                ImageDnd[,] subImages = references.database.GetImageArray(assetSelected.ImageClass().Id);
                
                //Set upperLeft corner reference
                Vector3Int upperLeft = centerGridPos - new Vector3Int(assetSelected.Columns() / 2, assetSelected.Rows() / 2);

                if (xEven && worldMousePos.x > map.CellToWorld(centerGridPos).x + map.cellSize.x / 2)
                {
                    upperLeft.x += 1;
                }
                if (yEven && worldMousePos.y > map.CellToWorld(centerGridPos).y + map.cellSize.y / 2)
                {
                    upperLeft.y += 1;
                }

                //Place images in tiles
                for (int i = 0; i < assetSelected.Columns(); i++)
                {
                    int iGrid = upperLeft.x + i;
                    //Skip out of range cases
                    if (iGrid < 0 || iGrid > gridManager.GetDimensions().x - 1) { Debug.Log("Out of tilemap range"); continue; }

                    for (int j = 0; j < assetSelected.Rows(); j++)
                    {
                        int jGrid = upperLeft.y + j;
                        //Skip out of range cases
                        if (jGrid < 0 || jGrid > gridManager.GetDimensions().y - 1) { Debug.Log("Out of tilemap range"); continue; }

                        gridManager.PaintAssetTile(iGrid, jGrid, subImages[assetSelected.Rows() - 1 - j, i]);
                    }
                }
            }
        }
    }
    #endregion

    #region INTERACTION FUNCTIONS
    //To use in buttons and stuff

    //Activate a specific Tool (requires int because of Unity shenanigans)
    public void ActivateTool(int t)
    {
        activeTool = (Tools) t;
        Debug.Log(t + " " + (Tools)t + " " + activeTool);
    }

    //Fill selected tiles with WFC generated content
    public void GenerateWFC()
    {
        gridManager.GridClass(references.wfcManager.GetWFC(gridManager.GridClass()));
        gridManager.PaintAssetMap();
    }

    #endregion


    Bounds MakeBounds(Vector3 pos1, Vector3 pos2)
    {
        Bounds bounds = new Bounds();
        return bounds;
    }

    List<RaycastResult> MouseRaycast()
    {
        //Set up the new Pointer Event
        PointerEventData _pointerEventData = new PointerEventData(references.eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        _pointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        references.gRaycaster.Raycast(_pointerEventData, results);

        return results;
    }

    enum SelectToolFunction
    {
        None = 0,
        SelectingTiles = 1,
        DraggingAsset = 2
    }
}

[Serializable]
public enum Tools
{
    None = 0,
    SelectTool = 1,
    DragTool = 2
}
