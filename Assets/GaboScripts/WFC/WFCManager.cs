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

    //TODO: TERMINAR
    private GridClass _GetWFC(GridClass grid)
    {
        // PROCEDURE
        // Receive grid.
        // Check "selected" tiles, all the others are considered collapsed.
        // Save in �priority list? from least to most entropy,
        // according to surrounding tiles.
        // Collapse least entropy tile using WFCTrainer from remaining posibs.
        // Propagate (Use PropagateFrom, LeastEntropyPTile, IsCollapsed)
        // Repeat until all are collapsed. If one tile cant be collapsed, ignore
        // and go to next one.


        //PossibilityTile[,] pTiles = new PossibilityTile[grid.width, grid.height];


        return null;
    }

    // DEBUG: Get random image.
    private GridClass _GetRandom(GridClass grid)
    {
        GridClass newGrid = new GridClass(grid.width, grid.height);

        ImageDatabase db = GetComponent<ManagerReferences>().database; //TODO: CAMBIAR!!! GetComponent<ImageManager>()
        db.Initialize();

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
