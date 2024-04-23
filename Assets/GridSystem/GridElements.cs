using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GridClass
{
    public readonly int width;
    public readonly int height;
    public Tile[,] Grid;

    public GridClass(int width, int height)
    {
        this.width = width;
        this.height = height;
        Grid = new Tile[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Grid[i,j] = new Tile();
            }
        }
    }
    public GridClass(int width, int height, bool random)
    {
        this.width = width;
        this.height = height;
        Grid = new Tile[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (random)
                {
                    Grid[i, j] = new Tile(UnityEngine.Random.Range(0, 10));
                }
                else
                {
                    Grid[i, j] = new Tile();
                }
            }
        }
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

[Serializable]
public class Tile
{
    public int Id;
    bool selected;

    public Tile()
    {
        Id = 0;
        selected = false;
    }

    public Tile(int id)
    {
        Id = id;
        selected = false;
    }

    public Tile(Tile t)
    {
        Id = t.Id;
        selected = false;
    }
}


//CLASE PARA SERIALIZAR UN OBJETO ClassHelper
//No se debe usar como referencia, su funcion es unicamente ayudar a serializar
//la clase.
[Serializable]
public class GridHelper
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] List<Tile> Grid;

    public GridHelper(GridClass g)
    {
        width = g.width;
        height = g.height;
        Grid = new List<Tile>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Grid.Add(new Tile(g.Grid[i,j].Id));
            }
        }
    }

    public GridClass ConvertToGridClass()
    {
        GridClass gc = new GridClass(width, height);

        int a = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Debug.Log(i + " " + j);
                gc.Grid[i, j] = new Tile(Grid[a]);
                a++;
            }
        }

        return gc;
    }
}
