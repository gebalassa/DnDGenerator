using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class SaveManager : ScriptableObject
{

    public Tile[] SaveData(Tile[,] tiles)
    {
        int height = tiles.GetLength(0);
        int width = tiles.GetLength(1);

        Tile[] tiles1D = Parse2DTo1D(tiles);

        SaveData data = new SaveData(tiles1D, height, width);

        //TODO TERMINAR LO DE SERIAIZACION

        return tiles1D;
    }

    public Tile[,] LoadData(string path)
    {
        //TODO HACER
        return null;
    }

    private Tile[] Parse2DTo1D(Tile[,] raw2D)
    {
        Tile[] parsed1D = new Tile[raw2D.Length];
        int counter = 0;
        for (int i = 0; i < raw2D.GetLength(0); i++)
        {
            for(int j = 0; j < raw2D.GetLength(1); j++)
            parsed1D[counter] = raw2D[i, j];
            counter++;
        }
        return parsed1D;
    }



}
