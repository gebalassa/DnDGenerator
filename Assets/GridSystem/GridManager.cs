using AnotherFileBrowser.Windows;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class GridManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    
    GridClass _grid;
    ImageDatabase database;

    [SerializeField] Tilemap backgroundMap;
    [SerializeField] Tilemap assetsMap;

    [SerializeField] UnityEngine.Tilemaps.Tile defaultTile;
    [SerializeField] UnityEngine.Tilemaps.Tile selectedTile;
    [SerializeField] UnityEngine.Tilemaps.Tile wallTile;

    [SerializeField] OutputCameraScript outputCamera;

    SaveLoadScript saveLoadScr = null;

    private void Awake()
    {
        //Obtener base de datos
        database = GetComponent<ManagerReferences>().database;

        //Get grid from AuxManager
        if (!GetGridFromAux())
        {
            LogFileManager.logString += "ERROR: AuxManager's SaveLoadScript returned null\n";
            _grid = new GridClass(width, height);
        }

        //Paint background
        PaintBackgroundMap();
        //Paint assets
        PaintAssetMap();
        //Adjust outputCamera
        outputCamera.ResizeCamera(this);
    }

    bool GetGridFromAux()
    {
        //Get script reference
        saveLoadScr = GameObject.Find("AuxManager").GetComponent<SaveLoadScript>();
        if(saveLoadScr == null) { return false; }

        //Copy values
        _grid = saveLoadScr.GetGrid();

        Vector2Int dimensions = saveLoadScr.GetDimensions();
        width = dimensions.x;
        height = dimensions.y;

        return true;
    }

    public void CallForSaveGrid()
    {
        if (saveLoadScr != null)
        {
            saveLoadScr.SaveGrid();
        }
        else
        {
            LogFileManager.logString += "ERROR: Can't save map without AuxManager's SaveLoadScript reference\n";
        }
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
        assetsMap.SetTile(new Vector3Int(x, y), new UnityEngine.Tilemaps.Tile());
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
                    assetsMap.SetTile(new Vector3Int(i, j), ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>());
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

    #region MISC
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
