using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClass
{
    int width;
    int height;
    public Tile[,] Grid;

    public GridClass(int width, int height)
    {
        this.width = width;
        this.height = height;
        Grid = new Tile[width, height];
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight() 
    {
        return height;
    }
    
}

public class Tile
{
    public int Id;
    bool selected;

    public Tile()
    {
        Id = 0;
        selected = false;
    }
}
