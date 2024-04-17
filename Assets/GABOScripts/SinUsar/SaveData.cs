using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Tile data
    int height;
    int width;
    Tile[] tiles;

    public SaveData(Tile[] tiles, int height, int width)
    {
        this.tiles = tiles;
        this.height = height;
        this.width = width;
    }
}
