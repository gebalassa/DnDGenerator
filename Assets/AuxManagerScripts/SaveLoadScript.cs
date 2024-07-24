using AnotherFileBrowser.Windows;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class SaveLoadScript : MonoBehaviour
{
    int width = 10;
    int height = 10;

    GridClass _grid;

    string saveFilePath = null;

    public bool dontLoad = false;

    private void Awake()
    {
        GameObject go = GameObject.Find("AuxManager");
        if (go != gameObject)
        {
            dontLoad = true;

            Destroy(go);

        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            LogFileManager.Write();
        }
    }

    public void SetWidthFromTMP(TextMeshProUGUI width)
    {
        string aux = "";
        foreach (var c in width.text.Substring(0, width.text.Length - 1))
        {
            Debug.Log("[" + c + "]");
            aux += c;
        }//*/
        this.width = Int32.Parse(aux);
    }
    public void SetHeightFromTMP(TextMeshProUGUI height)
    {
        string aux = "";
        foreach (var c in height.text.Substring(0, height.text.Length - 1))
        {
            Debug.Log("[" + c + "]");
            aux += c;
        }
        this.height = Int32.Parse(aux);
    }

    public void NewGrid()
    {
        _grid = new GridClass(width, height);
    }
    public void SaveGrid()
    {
        string json = JsonUtility.ToJson(new GridHelper(_grid), true);

        if (saveFilePath != null)
        {
            if (File.Exists(saveFilePath))
            {
                File.WriteAllText(saveFilePath, json);
                return;
            }
            saveFilePath = null;
        }

        string path = StandaloneFileBrowser.SaveFilePanel("Save File", saveFilePath, "map", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            saveFilePath = path;
        }
    }
    public void LoadAndChangeScene(int sceneIndex)
    {
        if (LoadGrid())
        {
            SceneChangeScript.ChangeScene(sceneIndex);
        }
    }
    public bool LoadGrid()
    {
        //Get path through explorer
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "json", false);
        if (paths.Length > 0)
        {
            string loadFilePath = paths[0];

            LogFileManager.logString += paths[0] + "\n";
            Debug.Log(paths[0]);

            if (loadFilePath.Length == 0)
            {
                LogFileManager.logString += "ERROR: path lenght empty\n";
                return false;
            }

            string json = File.ReadAllText(loadFilePath);
            GridHelper gh = JsonUtility.FromJson<GridHelper>(json);

            GridClass loadedGrid = gh.ConvertToGridClass();
            if (loadedGrid != null)
            {
                width = loadedGrid.width;
                height = loadedGrid.height;
                _grid = loadedGrid;
                
                LogFileManager.logString += "Map loaded succesfully\n";
                return true;
            }
        }
        LogFileManager.logString += "ERROR: Map couldn't load\n";
        return false;
    }

    public GridClass GetGrid()
    {
        return _grid;
    }
    public Vector2Int GetDimensions()
    {
        return new Vector2Int(width, height);
    }
}
