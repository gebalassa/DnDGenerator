using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCManager : MonoBehaviour
{
    public enum WFCDirection { UP, DOWN, LEFT, RIGHT };
    public WFCTrainer trainer;

    //TODO: TERMINARRRRR
    public GridClass GetWFC(GridClass grid)
    {
        GridClass newGrid = new GridClass(grid.width, grid.height);

        //DEBUG
        newGrid = _GetWFC(grid);//_GetRandom(grid);
        //FINDEBUG

        return newGrid;
    }
    //TODO: TERMINAR
    private GridClass _GetWFC(GridClass grid)
    {
        #region OBSOLETE PROCEDURE
        // PROCEDURE
        // Receive grid.
        // Check "selected" tiles, all the others are considered collapsed.
        // Save in ?priority list? from least to most entropy,
        // according to surrounding tiles.
        // Collapse least entropy tile using WFCTrainer from remaining posibs.
        // Propagate (Use PropagateFrom, LeastEntropyPTile, IsCollapsed)
        // Repeat until all are collapsed. If one tile cant be collapsed, ignore
        // and go to next one.
        #endregion

        WFCGrid wfcGrid = new WFCGrid(grid.width, grid.height);
        GridClass newGrid = wfcGrid.GetWFC(grid);

        return newGrid;
    }

    // DEBUG: Get random grid.
    private GridClass _GetRandom(GridClass grid)
    {
        GridClass newGrid = new GridClass(grid.width, grid.height);

        ImageDatabase db = FindAnyObjectByType<ManagerReferences>().imageManager.db;

        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.height; j++)
            {
                if (grid.Grid[i, j].selected)
                {
                    ImageDnd randomImage = db.GetRandomImage();
                    string randomImageId = "";
                    if (randomImage.rows != 1 || randomImage.columns != 1)
                    {
                        int randIndex = UnityEngine.Random.Range(0, randomImage.subImageIds.Count);
                        randomImageId = randomImage.subImageIds[randIndex];
                    }
                    else
                    {
                        randomImageId = randomImage.Id;
                    }
                    newGrid.Grid[i, j].Id = randomImageId;
                    //DEBUG
                    Debug.Log("randomImageId: ->" + randomImageId + "<- subSpriteName: " + db.GetImage(randomImageId).sprite.name + " ParentImageArrayDimensions: " + randomImage.rows + "x" + randomImage.columns);
                    //FIN DEBUG
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
