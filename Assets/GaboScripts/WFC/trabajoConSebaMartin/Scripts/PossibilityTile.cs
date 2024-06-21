using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PossibilityTile : MonoBehaviour
{
    [SerializeField]
    List<TileType> localPossibleTileTypes;

    [HideInInspector]
    public bool isCollapsed;
    [HideInInspector]
    public int gridI;
    [HideInInspector]
    public int gridJ;

    SebaTile resultingTile;
    DataContainer data;

    public List<TileType> GetLocalPossibleTileTypes() => localPossibleTileTypes;
    public SebaTile GetResultingTile() => resultingTile;

    public void SetGridPosition(int i, int j) { gridI = i; gridJ = j; }

    private void Awake()
    {
        data = FindObjectOfType<DataContainer>();
        localPossibleTileTypes = new List<TileType>(data.GetTileTypes());
        isCollapsed = false;
    }

    public void CollapseTile(TileType tileType)
    {
        localPossibleTileTypes.Clear();
        localPossibleTileTypes.Add(tileType);

        resultingTile = data.GetTile(tileType).GetComponent<SebaTile>();
        Instantiate<SebaTile>(resultingTile, transform);
        isCollapsed = true;
    }
    public void CollapseTile()
    {
        if (GetLocalPossibleTileTypes().Count != 1) { Debug.LogError("Valores posibles de largo distinto a 1 al intentar colapsar!"); }
        TileType tileType = GetLocalPossibleTileTypes()[0];
        CollapseTile(tileType);
    }
}
