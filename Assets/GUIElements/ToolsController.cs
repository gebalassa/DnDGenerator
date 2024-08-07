using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
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
    [SerializeField] TileSelectionMode selectionMode;
    SelectToolFunction selectDoing = SelectToolFunction.None;
    [SerializeField] GameObject selectionSquare = null;
    Vector3? startPosition;
    [SerializeField] GameObject assetPreview = null;
    DraggableAsset assetSelected = null;

    [Header("Drag Map Tool")]
    [SerializeField] float dragSpeed = 1f;
    [SerializeField] float zoomSpeed = 1f;
    Vector3? prevMousePosition;

    [Header("Wall Placement Tool")]
    Vector3? wallStartPosition;
    [SerializeField] LineRenderer lineRenderer = null;

    private void Awake()
    {
        startPosition = null;
        prevMousePosition = null;
        wallStartPosition = null;
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

            case Tools.MapTool:
                DragToolFunctions();
                break;

            case Tools.WallTool:
                WallFunctions();
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
            List<RaycastResult> results = MouseRaycast();
            if (results.Count == 0)
            {
                wallStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 pointPos = new Vector3(wallStartPosition.Value.x, wallStartPosition.Value.y, -6);

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, pointPos);
                lineRenderer.SetPosition(1, pointPos);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && wallStartPosition != null && lineRenderer != null)
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = -6;
            lineRenderer.SetPosition(1, currentPosition);
        }
        else if (wallStartPosition != null)
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Get Tiles under the line
            Tilemap map = gridManager.GetAssetMap();
            Vector2 dimensions = gridManager.GetDimensions();

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    Vector3 tilePosition = map.CellToWorld(new Vector3Int(i, j));
                    bool isUnder = LineRectCollision(wallStartPosition.Value, currentPosition, tilePosition, map.cellSize.x, map.cellSize.y);

                    if (isUnder)
                    {
                        gridManager.PaintAssetWall(i, j);
                    }
                }
            }

            //Reset aux values
            wallStartPosition = null;
            lineRenderer.positionCount = 0;
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

            Tilemap map = gridManager.GetBackgroundMap();
            Vector2 dimensions = gridManager.GetDimensions();

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    Vector3 tilePosition = map.CellToWorld(new Vector3Int(i, j));
                    bool isInside = IsTileInsideBounds(minX, maxX, minY, maxY, tilePosition, map.cellSize);

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

    #region UTILITIES
    bool IsTileInsideBounds(float minX, float maxX, float minY, float maxY, Vector3 tilePosition, Vector3 tileSize)
    {
        if(selectionMode == TileSelectionMode.FromCenter)
        {
            Vector3 tileCenter = tilePosition + tileSize / 2;
            if(tileCenter.x >= minX && tileCenter.x <= maxX && tileCenter.y >= minY && tileCenter.y <= maxY)
            {
                return true;
            }
        }
        else if(selectionMode == TileSelectionMode.AnyCollision)
        {
            if (maxX >= tilePosition.x && minX <= tilePosition.x + tileSize.x && maxY >= tilePosition.y && minY <= tilePosition.y + tileSize.y)
            {
                return true;
            }
        }
        return false;
    }

    bool LineRectCollision(Vector2 lineStart, Vector2 lineEnd, Vector2 rBottomLeft, float rWidth, float rHeight)
    {
        // check if the line has hit any of the rectangle's sides
        // uses the Line/Line function below
        bool left = LineLineCollision(lineStart, lineEnd, rBottomLeft, rBottomLeft + new Vector2(0,rHeight));
        bool right = LineLineCollision(lineStart, lineEnd, rBottomLeft + new Vector2(rWidth, 0), rBottomLeft + new Vector2(rWidth, rHeight));
        bool top = LineLineCollision(lineStart, lineEnd, rBottomLeft + new Vector2(0, rHeight), rBottomLeft + new Vector2(rWidth, rHeight));
        bool bottom = LineLineCollision(lineStart, lineEnd, rBottomLeft, rBottomLeft + new Vector2(rWidth, 0));

        // if ANY of the above are true, the line
        // has hit the rectangle
        if (left || right || top || bottom)
        {
            return true;
        }
        return false;
    }

    bool LineLineCollision(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
    {
        // calculate the direction of the lines
        float uA = ((point4.x - point3.x) * (point1.y - point3.y) - (point4.y - point3.y) * (point1.x - point3.x)) / ((point4.y - point3.y) * (point2.x - point1.x) - (point4.x - point3.x) * (point2.y - point1.y));
        float uB = ((point2.x - point1.x) * (point1.y - point3.y) - (point2.y - point1.y) * (point1.x - point3.x)) / ((point4.y - point3.y) * (point2.x - point1.x) - (point4.x - point3.x) * (point2.y - point1.y));

        // if uA and uB are between 0-1, lines are colliding
        if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
        {
            return true;
        }
        return false;
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

    enum TileSelectionMode
    {
        FromCenter = 0,
        AnyCollision = 1
    }
    #endregion
}

[Serializable]
public enum Tools
{
    None = 0,
    SelectTool = 1,
    MapTool = 2,
    WallTool = 3
}
