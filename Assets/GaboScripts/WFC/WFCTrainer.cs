using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewWFCTrainer", menuName = "WFCTrainer")]
[System.Serializable]
public class WFCTrainer : ScriptableObject
{
    // TRAINING
    // Get all images from ImageDatabase
    // Create dictionary to save tile frequency, associated tiles and their dir.
    // Inside , use training data:
    // --From each training image, check each tile.
    // --Add 1 to its frequency.    
    // FOR EACH TILE
    // For each training tile, check surrounding tiles.
    // If any of those tiles is new to the current one,
    // add to associated tiles with their dir.

    // RECOMMENDATION TILE WHEN COLLAPSING
    // To get recommendation (when collapsing least entropy tile),
    // use the frequencies of each remaining possibility:
    // -- Order remaining from least to most frequent.
    // -- Random number from 0 to SUM OF THEIR FREQUENCIES.
    // -- Use aggregated frequency trick to choose.

    public Dictionary<string, Dictionary<string, WFCManager.WFCDirection>> tiles;
    public List<GridClass> debugTrainingMaps = new List<GridClass>();

    // TODO: Terminar
    public void Train()
    {

        LoadTrainingMaps();
    }

    // TODO: Corregir BUG de Grids vacios al cargar
    private void LoadTrainingMaps()
    {
        string mapsPath = "Assets/Maps";
        //Object[] rawMaps = AssetDatabase.LoadAllAssetsAtPath(mapsPath);
        string[] mapGuids = AssetDatabase.FindAssets("t:TextAsset", new string[] { mapsPath });

        List<GridClass> maps = new List<GridClass>();
        foreach (string mapGuid in mapGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(mapGuid);
            TextAsset currTextAssetMap = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (currTextAssetMap != null)
            {
                string json = currTextAssetMap.text;
                GridHelper gh = JsonUtility.FromJson<GridHelper>(json);
                GridClass currMap = gh.ConvertToGridClass();
                maps.Add(currMap);
            }
            else
            {
                Debug.LogWarning("Couldn't cast object " + path + " as Text Asset!");
            }
        }

        debugTrainingMaps.AddRange(maps);
    }
}
