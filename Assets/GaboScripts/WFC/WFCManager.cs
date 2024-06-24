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

        ImageDatabase db = GetComponent<ManagerReferences>().database;
        db.Initialize();

        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.height; j++)
            {
                if (grid.Grid[i, j].selected)
                {
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
