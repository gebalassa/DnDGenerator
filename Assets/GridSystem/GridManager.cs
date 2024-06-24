using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    
    GridClass _grid;

    [SerializeField] float gizmosSize;

    [SerializeField] Tilemap backgroundMap;
    [SerializeField] Tilemap assetsMap;

    [SerializeField] UnityEngine.Tilemaps.Tile defaultTile;
    [SerializeField] UnityEngine.Tilemaps.Tile selectedTile;

    [SerializeField] ImageDatabase database;
    [SerializeField] OutputCameraScript outputCamera;

    [SerializeField] bool debugGrid = false;

    bool onRunTime = false;

    string saveFilePath = null;

    private void Awake()
    {
        onRunTime = true;

        _grid = new GridClass(width,height);
        PaintBackgroundMap();
        outputCamera.ResizeCamera(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGrid();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GridClass aux = LoadGrid();
            if(aux != null)
            {
                _grid = aux;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (onRunTime && debugGrid)
        {
            Gizmos.color = Color.green;
            Vector3 vectorSize = new Vector3(gizmosSize, gizmosSize);

            for (int i = 0; i < _grid.GetWidth(); i++)
            {
                for (int j = 0; j < _grid.GetHeight(); j++)
                {
                    Vector3 position = new Vector3(i * gizmosSize, j * -gizmosSize);

                    Gizmos.DrawWireCube(position, vectorSize);

                    Handles.Label(position, _grid.Grid[i,j].Id.ToString());
                }
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
            string saveFilePath = EditorUtility.SaveFilePanel("Choose location to save map", "Assets/GridSystem", "", ".json");
            if (saveFilePath.Length == 0) { return; }
            File.WriteAllText(saveFilePath, json);
        }
    }

    GridClass LoadGrid()
    {
        string saveFilePath = EditorUtility.OpenFilePanel("Choose location to load map", "Assets/GridSystem",".json");
        if(saveFilePath.Length == 0) { return null; }

        string json = File.ReadAllText(saveFilePath);
        GridHelper gh = JsonUtility.FromJson<GridHelper>(json);

        GridClass loadedGrid = gh.ConvertToGridClass();
        width = loadedGrid.width;
        height = loadedGrid.height;
        return loadedGrid;
    }


    public void ChangeTileState(int x, int y, bool newState)
    {
        _grid.Grid[x,y].selected = newState;
    }

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

    public void PaintAssetTile(int x, int y, ImageDnd _imageDnd)
    {
        _grid.Grid[x,y].Id = _imageDnd.Id;

        UnityEngine.Tilemaps.Tile _tile = new UnityEngine.Tilemaps.Tile();
        _tile.sprite = _imageDnd.sprite;

        assetsMap.SetTile(new Vector3Int(x, y), _tile);
    }

    public void EraseAssetTile(int x, int y)
    {
        _grid.Grid[x, y].Id = "none";
        assetsMap.SetTile(new Vector3Int(x, y), new UnityEngine.Tilemaps.Tile());
    }

    public void PaintAssetMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (_grid.Grid[i, j].Id != "none")
                {
                    ImageDnd _imageDnd = database.GetImage(_grid.Grid[i, j].Id);

                    UnityEngine.Tilemaps.Tile _tile = new UnityEngine.Tilemaps.Tile();
                    _tile.sprite = _imageDnd.sprite;

                    assetsMap.SetTile(new Vector3Int(i, j), _tile);
                }
            }
        }
    }

<<<<<<< Updated upstream
    public GridClass GridClass()
    {
        return _grid;
    }

    public void GridClass(GridClass newGrid)
    {
        Debug.LogWarning("Overwritting grid");
        _grid = newGrid;
    }
=======
    public GridClass GetGridClass()
    {
        return _grid;
    }//*/
>>>>>>> Stashed changes

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
}
