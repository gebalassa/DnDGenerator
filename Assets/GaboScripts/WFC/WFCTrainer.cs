using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Ookii.Dialogs;

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

    [SerializedDictionary("ID", "Association List")]
    public SerializedDictionary<string, List<AssociationTuple>> tileAssociations = new();
    [SerializedDictionary("ID", "Frequency")]
    public SerializedDictionary<string, int> tileFrequencies = new();
    public List<GridClassNameWrapper> trainingMaps = new List<GridClassNameWrapper>();

    // TODO: Terminar
    public void Train()
    {
        Clear();
        LoadTrainingMaps();
        PopulateTilesFromLoadedMaps();
    }

    // Load maps from maps folder
    private void LoadTrainingMaps()
    {
        // Obtain map GUIDs
        string mapsPath = "Assets/Maps";
        string[] mapGuids = AssetDatabase.FindAssets("t:TextAsset", new string[] { mapsPath });
        if (mapGuids.Length == 0) { Debug.LogWarning("Couldn't find any maps!"); }

        // Create GridClassNameWrapper objects (GridClass objects with names)
        List<GridClassNameWrapper> maps = new List<GridClassNameWrapper>();
        foreach (string mapGuid in mapGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(mapGuid);
            TextAsset currTextAssetMap = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (currTextAssetMap != null)
            {
                string json = currTextAssetMap.text;
                GridHelper gh = JsonUtility.FromJson<GridHelper>(json);
                GridClass currMap = gh.ConvertToGridClass();
                maps.Add(new GridClassNameWrapper(currMap, currTextAssetMap.name));
            }
            else
            {
                Debug.LogWarning("Couldn't cast object " + path + " as Text Asset!");
            }
        }

        // Add all to map list
        trainingMaps.AddRange(maps);
    }

    // Get associated tiles for every tile, with direction
    // *****NOTA****: Por la forma rara en que instancia
    // ***(hacia arriba primero en la columna, y asi hacia la derecha)
    // ***entonces i depende del width, y j del height. Arriba=IZQ, Derecha=UP, etc.
    private void PopulateTilesFromLoadedMaps()
    {
        foreach (GridClassNameWrapper trainingMap in trainingMaps)
        {
            for (int i = 0; i < trainingMap.gc.width; i++)
            {
                for (int j = 0; j < trainingMap.gc.height; j++)
                {
                    Tile currentTile = trainingMap.gc.Grid[i, j];
                    var newNeighbours = GetNeighbourhood(i, j, currentTile.Id, trainingMap.gc);

                    // Create frequency if new, otherwise add 1 to frequency
                    if (!tileFrequencies.ContainsKey(currentTile.Id))
                    {
                        tileFrequencies.Add(currentTile.Id, 1);
                    }
                    else
                    {
                        tileFrequencies[currentTile.Id] += 1;
                    }
                    // Create associations list if new, otherwise add neighbour (if new, too)
                    foreach (AssociationTuple neighbour in newNeighbours)
                    {
                        if (!tileAssociations.ContainsKey(currentTile.Id))
                        {
                            tileAssociations.Add(currentTile.Id, new());
                            tileAssociations[currentTile.Id].Add(neighbour);
                        }
                        else if (!tileAssociations[currentTile.Id].Contains(neighbour))
                        {
                            tileAssociations[currentTile.Id].Add(neighbour);
                        }
                    }
                }
            }
        }
    }

    // Neighborhood of a single tile
    // *****NOTA****: Por la forma rara en que instancia
    // ***(hacia arriba primero en la columna, y asi hacia la derecha)
    // ***entonces i depende del width, y j del height. Arriba=IZQ, Derecha=UP, etc.
    private List<AssociationTuple> GetNeighbourhood(int i, int j, string id, GridClass gc)
    {
        List<AssociationTuple> neighbourhood = new();
        // Check each direction (ignore walls and empty for training)
        // UP
        if (i > 0)
        {
            neighbourhood.Add(new(gc.Grid[i - 1, j].Id, WFCManager.WFCDirection.UP));
        }
        // DOWN
        if (i < gc.width - 1)
        {
            neighbourhood.Add(new(gc.Grid[i + 1, j].Id, WFCManager.WFCDirection.DOWN));
        }
        // LEFT
        if (j > 0)
        {
            neighbourhood.Add(new(gc.Grid[i, j - 1].Id, WFCManager.WFCDirection.LEFT));
        }
        // RIGHT
        if (j < gc.height - 1)
        {
            neighbourhood.Add(new(gc.Grid[i, j + 1].Id, WFCManager.WFCDirection.RIGHT));
        }
        return neighbourhood;
    }

    private void Clear()
    {
        tileAssociations = new();
        tileFrequencies = new();
        trainingMaps = new List<GridClassNameWrapper>();
    }

    private bool IsWall(string id) { return id == "wall"; }
    private bool IsNone(string id) { return id == "none"; }

    [Serializable]
    public class AssociationTuple : IEquatable<AssociationTuple>
    {
        [SerializeField]
        string id;
        [SerializeField]
        WFCManager.WFCDirection direction;
        public AssociationTuple(string id, WFCManager.WFCDirection direction)
        {
            this.id = id;
            this.direction = direction;
        }
        public bool Equals(AssociationTuple other)
        {
            if (other == null) { return false; }
            else if (this.id == other.id && this.direction == other.direction) { return true; }
            else { return false; }
        }
    }
    // To print GridClass instances with names (based on json file name) in Inspector
    [Serializable]
    public class GridClassNameWrapper
    {
        public string Name;
        [NonSerialized]
        public GridClass gc;
        public GridClassNameWrapper(GridClass gc, string name)
        {
            this.gc = gc;
            this.Name = name;
        }
    }
}
