using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WFCTile
{
    List<string> possibleTileIds = new List<string>();
    public int i;
    public int j;
    private WFCTrainer trainer;
    public WFCTile(int i, int j)
    {
        this.i = i;
        this.j = j;
        trainer = Object.FindObjectOfType<ManagerReferences>().wfcManager.trainer;
    }
    public List<string> GetPossibleTileIds() { return possibleTileIds; }
    public void AddPossibleTileId(string id) { possibleTileIds.Add(id); }
    public void AddPossibleTileIds(IEnumerable<string> ids) { possibleTileIds.AddRange(ids); }
    public void RemovePossibleTileId(string id) { possibleTileIds.Remove(id); }
    public void ClearPossibleTileIds() { possibleTileIds.Clear(); }
    public int GetEntropy() { return possibleTileIds.Count; }

    public bool IsCollapsed()
    {
        if (possibleTileIds.Count == 1)
        {
            return true;
        }
        else if (possibleTileIds.Count == 0)
        {
            Debug.LogWarning("Tile " + i + "," + j + " has 0 possibilities!");
            return false;
        }
        else return false;
    }

    public bool CanBeCollapsed() { return possibleTileIds.Count != 0; }

    //TODO: TERMINARRRR
    // Collapse using relative frequency between remaining possible tiles
    public void Collapse()
    {
        // Warn for uncollapsable tile
        if (!CanBeCollapsed())
        {
            Debug.LogError($"Can't collapse {i},{j}!"); return;
        }

        // Collapse using aggregated frequency trick
        // 1) Get total frequency of current possible tiles
        int totalFrequency = 0;
        foreach (string id in possibleTileIds)
        {
            totalFrequency += trainer.tileFrequencies[id];
        }
        // 2) Choose random based on relative frequencies
        int chosenValue = Random.Range(1, totalFrequency);
        int currentAggregatedFrequency = 0;
        foreach (string id in possibleTileIds)
        {
            currentAggregatedFrequency += trainer.tileFrequencies[id];
            // Check if chosen
            if (chosenValue <= currentAggregatedFrequency)
            {
                _Collapse(id);
                return;
            }
        }
    }
    public void Collapse(string id)
    {
        _Collapse(id);
    }
    private void _Collapse(string id)
    {
        ClearPossibleTileIds();
        AddPossibleTileId(id);
    }

    // DEBUG: Collapse into random tile id
    private void _RandomCollapse()
    {
        int randIndex = Random.Range(0, possibleTileIds.Count);
        string chosenId = possibleTileIds[randIndex];
        ClearPossibleTileIds();
        AddPossibleTileId(chosenId);
    }

}
