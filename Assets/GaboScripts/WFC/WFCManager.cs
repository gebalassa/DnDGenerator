using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCManager : MonoBehaviour
{
    //TODO: TERMINARRRRR
    public GridClass GetWFC(GridClass grid)
    {
        GridClass newGrid = new GridClass(grid.width, grid.height);

        //DEBUG
        newGrid = _GetRandom(grid);
        //FINDEBUG

        return newGrid;
    }

    // DEBUG: Get random image.
    private GridClass _GetRandom(GridClass grid)
    {
        GridClass newGrid = new GridClass(grid.width, grid.height);

        for (int i = 0; i < grid.height; i++)
        {
            for (int j = 0; j < grid.width; j++)
            {
                if (grid.Grid[i, j].selected)
                {
                    ImageDatabase db = GetComponent<ImageManager>().db;
                    newGrid.Grid[i, j].Id = db.GetRandomImage().Id;
                    newGrid.Grid[i, j].selected = true;
                }
                else
                {
                    newGrid.Grid[i, j].Id = grid.Grid[i, j].Id;
                    newGrid.Grid[i, j].selected = false;
                }
            }
        }
        return newGrid;
    }

}
