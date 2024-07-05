using AnotherFileBrowser.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    
    GridClass _grid;
    ImageDatabase database;

    [SerializeField] float gizmosSize;

    [SerializeField] Tilemap backgroundMap;
    [SerializeField] Tilemap assetsMap;

    [SerializeField] UnityEngine.Tilemaps.Tile defaultTile;
    [SerializeField] UnityEngine.Tilemaps.Tile selectedTile;
    [SerializeField] UnityEngine.Tilemaps.Tile wallTile;

    [SerializeField] OutputCameraScript outputCamera;

    //[SerializeField] bool debugGrid = false;

    string saveFilePath = null;

    private void Awake()
    {
        _grid = new GridClass(width,height);
        PaintBackgroundMap();
        outputCamera.ResizeCamera(this);
        database = GetComponent<ManagerReferences>().database;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            LogFileManager.Write();
            SaveGrid();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(LoadGrid())
            {
                Debug.Log("Map loaded succesfully");
                LogFileManager.logString += "Map loaded succesfully\n";
                PaintBackgroundMap();
                PaintAssetMap();
            }
        }
    }

    void SaveGrid()
    {
        string json = JsonUtility.ToJson( new GridHelper(_grid), true );
        
        if(saveFilePath != null)
        {
            File.WriteAllText(saveFilePath, json);
        }
        else
        {
            string saveFilePath = EditorUtility.SaveFilePanel("Choose location to save map", "Assets/GridSystem", "", "json");
            if (saveFilePath.Length == 0) { return; }
            File.WriteAllText(saveFilePath, json);
        }//*/
    }

    bool LoadGrid()
    {
        var bp = new BrowserProperties();
        bp.filter = "Map files (*.json) | *.json";
        bp.filterIndex = 0;

        string loadFilePath = "";

        //Get path through explorer
        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            loadFilePath = path;
            LogFileManager.logString += path + "\n";
            Debug.Log(path);
        });

        if (loadFilePath.Length == 0) { return false; }

        string json = File.ReadAllText(loadFilePath);
        GridHelper gh = JsonUtility.FromJson<GridHelper>(json);

        GridClass loadedGrid = gh.ConvertToGridClass();
        if(loadedGrid != null)
        {
            width = loadedGrid.width;
            height = loadedGrid.height;
            _grid = loadedGrid;

            return true;
        }
        return false;
    }

    #region PAINT MAP FUNCTIONS
    /// <summary>
    /// Changes one tile to a new state
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="newState">true if selected</param>
    public void ChangeTileState(int x, int y, bool newState)
    {
        _grid.Grid[x,y].selected = newState;
    }
    /// <summary>
    /// Paints all the selected tiles from the grid to the background tilemap
    /// </summary>
    public void PaintBackgroundMap()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (_grid.Grid[i, j].selected)
                {
                    backgroundMap.SetTile(new Vector3Int(i, j), selectedTile);
                }
                else
                {
                    backgroundMap.SetTile(new Vector3Int(i, j), defaultTile);
                }
            }
        }
    }
    /// <summary>
    /// Paint one tile in the asset tilemap using an imageDnd
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="_imageDnd"></param>
    public void PaintAssetTile(int x, int y, ImageDnd _imageDnd)
    {
        _grid.Grid[x,y].Id = _imageDnd.Id;

        UnityEngine.Tilemaps.Tile _tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
        _tile.sprite = _imageDnd.sprite;

        assetsMap.SetTile(new Vector3Int(x, y), _tile);
    }
    /// <summary>
    /// Sets one tile of wall in the grid and paints it in the asset tilemap
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void PaintAssetWall(int x, int y)
    {
        _grid.Grid[x, y].Id = "wall";

        assetsMap.SetTile(new Vector3Int(x, y), wallTile);
    }
    /// <summary>
    /// Cleans one tile in the grid and the asset tilemap
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void EraseAssetTile(int x, int y)
    {
        _grid.Grid[x, y].Id = "none";
        assetsMap.SetTile(new Vector3Int(x, y), ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>());
    }
    /// <summary>
    /// Paints all the tiles from the grid in the asset tilemap
    /// </summary>
    public void PaintAssetMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(_grid.Grid[i, j].Id == "wall")
                {
                    assetsMap.SetTile(new Vector3Int(i, j), wallTile);
                    continue;
                }

                if (_grid.Grid[i, j].Id != "none")
                {
                    ImageDnd _imageDnd = database.GetImage(_grid.Grid[i, j].Id);

                    //Id not found in db
                    if(_imageDnd == null) 
                    {
                        _grid.Grid[i, j].Id = "none";
                        continue; 
                    }

                    UnityEngine.Tilemaps.Tile _tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
                    _tile.sprite = _imageDnd.sprite;

                    assetsMap.SetTile(new Vector3Int(i, j), _tile);
                }
                else
                {
                    assetsMap.SetTile(new Vector3Int(i, j), new UnityEngine.Tilemaps.Tile());
                }
            }
        }
    }
    #endregion

    #region ACCESS FUNCTIONS
    public GridClass GridClass()
    {
        return _grid;
    }
    public void GridClass(GridClass newGrid)
    {
        Debug.LogWarning("Overwritting grid");
        _grid = newGrid;
    }
    public Tilemap GetBackgroundMap()
    {
        return backgroundMap;
    }
    public Tilemap GetAssetMap()
    {
        return assetsMap;
    }
    public Vector2Int GetDimensions()
    {
        return new Vector2Int(width, height);
    }
    #endregion
}

public class LogFileManager
{
    static public string logString = "";

    static public void Write()
    {
        FileInfo directory = new FileInfo(Application.dataPath + "/LogFiles/Log.txt");
        directory.Directory.Create();

        int logCounter = 0;
        string fileName = "/Log.txt";
        FileInfo file = new FileInfo(directory.Directory + fileName);
        while (file.Exists)
        {
            logCounter++;
            fileName = "/Log" + logCounter.ToString() + ".txt";
            file = new FileInfo(directory.Directory + fileName);
        }
        File.WriteAllText(directory.Directory + fileName, logString);
    }
}
