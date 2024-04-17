using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    
    GridClass _grid;

    [SerializeField] float size;
    [SerializeField] GameObject sphere;

    bool onRunTime = false;

    string saveFilePath = null;

    private void Awake()
    {
        onRunTime = true;

        _grid = new GridClass(width,height);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _grid = new GridClass(width, height, true);
        }
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
        if (onRunTime)
        {
            Gizmos.color = Color.green;
            Vector3 vectorSize = new Vector3(size, size);

            for (int i = 0; i < _grid.GetWidth(); i++)
            {
                for (int j = 0; j < _grid.GetHeight(); j++)
                {
                    Vector3 position = new Vector3(i * size, j * -size);

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
            string saveFilePath = EditorUtility.SaveFilePanel("Choose direction to save map", "Assets/GridSystem", "", ".json");
            if (saveFilePath.Length == 0) { return; }
            File.WriteAllText(saveFilePath, json);
        }
    }

    GridClass LoadGrid()
    {
        string saveFilePath = EditorUtility.OpenFilePanel("Choose direction to load map", "Assets/GridSystem",".json");
        if(saveFilePath.Length == 0) { return null; }

        string json = File.ReadAllText(saveFilePath);
        GridHelper gh = JsonUtility.FromJson<GridHelper>(json);

        return gh.ConvertToGridClass();
    }
}
