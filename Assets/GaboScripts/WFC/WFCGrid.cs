using System.Collections.Generic;
using UnityEngine;
using static WFCManager;

[System.Serializable]
public class WFCGrid
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public WFCTile[,] Grid { get { return wfcGrid; } set { wfcGrid = value; } }

    private WFCTile[,] wfcGrid;
    private WFCTrainer trainer = Object.FindAnyObjectByType<ManagerReferences>().wfcManager.trainer;
    private ImageManager imageManager = Object.FindAnyObjectByType<ManagerReferences>().imageManager;
    private GridClass currentGridClass;

    //DEBUG
    //private int debugSteps = 5000;
    //FIN DEBUG

    public WFCGrid(int height, int width)
    {
        this.Height = height;
        this.Width = width;
        wfcGrid = new WFCTile[height, width];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                wfcGrid[i, j] = new WFCTile(i, j);
            }
        }
    }

    public GridClass GetWFC(GridClass gc)
    {
        // Make class-wide reference to current GridClass parameter.
        currentGridClass = gc;

        // Check if any tiles selected, otherwise return as is.
        if (!IsAnyTileSelected(gc)) { Debug.LogWarning("No tiles selected."); return gc; }

        // Initial fill of tiles
        Fill(gc);

        // 1st Check for uncollapsables, in which case simply abort
        if (IsAnyTileUncollapsable())
        {
            Debug.LogWarning("Check after Fill(): Uncollapsable tiles!");
            return gc;
        }

        // Cycle between collapsing random and propagating until fully collapsed
        // or if it can't be collapsed anymore.
        while (!IsGridCollapsedAsMuchAsPossible())
        {
            CollapseLeastEntropy();

            // 2nd check for uncollapsables, in which case simply abort.
            if (IsAnyTileUncollapsable())
            {
                Debug.LogWarning("Check after CollapseLeastEntropy(): Uncollapsable tiles!");
                return gc;
            }
        }

        // If non-selected tiles were modified, they recover their original ids.
        ResetNonSelectedTileIds();

        // Assign values to GridClass
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (Grid[i, j].CanBeCollapsed() && Grid[i, j].IsCollapsed())
                {
                    gc.Grid[i, j].Id = Grid[i, j].GetPossibleTileIds()[0];
                }
                else if (Grid[i, j].CanBeCollapsed() && !Grid[i, j].IsCollapsed())
                {
                    Debug.LogError($"Error: WFCTile {i},{j} remains collapsable after exiting cycle!");
                }
                else if (!Grid[i, j].CanBeCollapsed())
                {
                    //Debug.Log($"Tile {i},{j} can't be collapsed!");
                }
            }
        }

        // Return modified GridClass
        return gc;
    }

    // Collapse least entropy tile and propagate
    private void CollapseLeastEntropy()
    {
        List<WFCTile> uncollapsedTiles = GetUncollapsedByEntropy();
        if (uncollapsedTiles.Count == 0) { Debug.LogWarning("No uncollapsed tiles!"); return; }
        WFCTile chosenTile = uncollapsedTiles[0];
        //chosenTile.Collapse();
        //Propagate(chosenTile);
        CollapseTile(chosenTile.i, chosenTile.j);
    }
    private void CollapseTile(int i, int j)
    {
        if (Grid[i, j].CanBeCollapsed() && !Grid[i, j].IsCollapsed())
        {
            Grid[i, j].CollapseWithoutPropagation(); // Chooses from between remaining options
            Propagate(Grid[i, j]);
        }
        // If tile entropy is zero for whatever reason, abort //TODO: Ver si funciona
        else if (!Grid[i, j].CanBeCollapsed())
        {
            //Debug.LogWarning($"CollapseTile(): Tile {i},{j}," +
            //                 $"originally {imageManager.db.GetImage(currentGridClass.Grid[i, j].Id).sprite.name}" +
            //                 $"is uncollapsable!");
            return;
        }
    }

    private void Propagate(WFCTile collapsedTile)
    {
        Queue<WFCTile> pending = new();
        pending.Enqueue(collapsedTile);

        while (pending.Count > 0)// && debugSteps > 0)
        {
            //DEBUG
            //debugSteps--;
            //FIN DEBUG
            WFCTile currTile = pending.Dequeue();

            // Check directions for uncollapsed tiles, then propagate.
            // UP
            if (currTile.i > 0)
            {
                WFCTile neighbourTile = Grid[currTile.i - 1, currTile.j];
                if (!neighbourTile.IsCollapsed() && neighbourTile.CanBeCollapsed())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.UP);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
                //Uncollapsable check
                if (!neighbourTile.CanBeCollapsed())
                {
                    //Debug.LogWarning($"Propagate(): {neighbourTile.i},{neighbourTile.j} is now uncollapsable.");
                    return;
                }
            }
            // DOWN
            if (currTile.i < Height - 1)
            {
                WFCTile neighbourTile = Grid[currTile.i + 1, currTile.j];
                if (!neighbourTile.IsCollapsed() && neighbourTile.CanBeCollapsed())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.DOWN);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
                //Uncollapsable check
                if (!neighbourTile.CanBeCollapsed())
                {
                    //Debug.LogWarning($"Propagate(): {neighbourTile.i},{neighbourTile.j} is now uncollapsable.");
                    return;
                }
            }
            // LEFT
            if (currTile.j > 0)
            {
                WFCTile neighbourTile = Grid[currTile.i, currTile.j - 1];
                if (!neighbourTile.IsCollapsed() && neighbourTile.CanBeCollapsed())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.LEFT);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
                //Uncollapsable check
                if (!neighbourTile.CanBeCollapsed())
                {
                    //Debug.LogWarning($"Propagate(): {neighbourTile.i},{neighbourTile.j} is now uncollapsable.");
                    return;
                }
            }
            // RIGHT
            if (currTile.j < Width - 1)
            {
                WFCTile neighbourTile = Grid[currTile.i, currTile.j + 1];
                if (!neighbourTile.IsCollapsed() && neighbourTile.CanBeCollapsed())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.RIGHT);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
                //Uncollapsable check
                if (!neighbourTile.CanBeCollapsed())
                {
                    //Debug.Log($"Propagate(): {neighbourTile.i},{neighbourTile.j} is now uncollapsable.");
                    return;
                }
            }
        }
    }

    private bool DirectionalSinglePropagation(WFCTile origin, WFCManager.WFCDirection direction)
    {
        // WFCTile to constrain
        WFCTile objective = new(-1, -1);
        bool objectiveShouldbeQueued = false;
        switch (direction)
        {
            case WFCDirection.UP:
                objective = Grid[origin.i - 1, origin.j];
                break;
            case WFCDirection.DOWN:
                objective = Grid[origin.i + 1, origin.j];
                break;
            case WFCDirection.LEFT:
                objective = Grid[origin.i, origin.j - 1];
                break;
            case WFCDirection.RIGHT:
                objective = Grid[origin.i, origin.j + 1];
                break;
        }

        // Constrain objective tile using origin tile (obtain tiles to remove)
        List<string> toRemove = new();
        foreach (string currObjectiveId in objective.GetPossibleTileIds())
        {
            bool isObjectiveAllowed = false;
            foreach (string currOriginId in origin.GetPossibleTileIds())
            {
                // Verify if current objective id is still allowed
                List<string> allowedIds = trainer.GetAllowedNeighbours(currOriginId,
                    direction);
                if (allowedIds.Contains(currObjectiveId))
                {
                    isObjectiveAllowed = true;
                    break;
                }
            }
            if (!isObjectiveAllowed)
            {
                toRemove.Add(currObjectiveId);
            }
        }
        // if there was change (removals), objectiveShouldBeQueued = true
        if (toRemove.Count != 0)
        {
            objectiveShouldbeQueued = true;
        }
        // Remove each non-allowed tile
        foreach (string removableId in toRemove)
        {
            objective.RemovePossibleTileId(removableId);
        }

        // Check if the objective became uncollapsable
        if (!objective.CanBeCollapsed())
        {
            Debug.LogWarning($"SingleTilePropagation(): Tile {objective.i},{objective.j}," +
                 $"originally {imageManager.db.GetImage(currentGridClass.Grid[objective.i, objective.j].Id).sprite.name}" +
                 $" is uncollapsable due to {origin.i},{origin.j}!");
            return false;
        }

        //// Prevent 0-possibility tile by collapsing to "None" if selected,
        //// or its original tile if non-selected.
        //// TODO: Probar si funciona
        //Tile currentGridClassTile = currentGridClass.Grid[objective.i, objective.j];
        //if (!objective.CanBeCollapsed())
        //{
        //    if (currentGridClassTile.selected)
        //    {
        //        objective.Collapse("none");
        //    }
        //    else
        //    {
        //        objective.Collapse(currentGridClassTile.Id);
        //    }
        //}

        return objectiveShouldbeQueued;
    }

    // List with uncollapsed (& collapsable) tiles ordered by least to most entropy
    public List<WFCTile> GetUncollapsedByEntropy()
    {
        List<WFCTile> uncollapsedTiles = new();
        int currLeastEntropy = 999999;
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                WFCTile currTile = Grid[i, j];
                if (currTile.IsCollapsed() || !currTile.CanBeCollapsed()) { continue; }
                else if (currTile.GetEntropy() < currLeastEntropy)
                {
                    uncollapsedTiles.Insert(0, currTile);
                    currLeastEntropy = currTile.GetEntropy(); // Update lowest
                }
                else
                {
                    int initialCount = uncollapsedTiles.Count;
                    for (int u = 0; u < initialCount; u++)
                    {
                        if (currTile.GetEntropy() <= uncollapsedTiles[u].GetEntropy())
                        {
                            uncollapsedTiles.Insert(u, currTile);
                            break;
                        }
                        // If most entropy, insert last
                        else if (u == initialCount - 1)
                        {
                            uncollapsedTiles.Add(currTile);
                        }
                    }
                }
            }
        }
        return uncollapsedTiles;
    }

    // Fill V2.0
    private void Fill(GridClass gc)
    {
        // Clear tiles
        Clear();

        // All start with every tile
        for (int i = 0; i < gc.height; i++)
        {
            for (int j = 0; j < gc.width; j++)
            {
                foreach (string id in trainer.tileAssociations.Keys)
                {
                    Grid[i, j].AddPossibleTileId(id);
                }
            }
        }

        // Iterate collapsing of 'non-selected tiles'
        for (int i = 0; i < gc.height; i++)
        {
            for (int j = 0; j < gc.width; j++)
            {
                if (!gc.Grid[i, j].selected)
                {
                    CollapseTile(i, j);
                }
            }
        }
    }

    #region failed fill 1.0
    // Initial filling of grid with possibilities
    //public void Fill(GridClass gc)
    //{
    //    // Clear tiles
    //    Clear();

    //    // All 'selected' start with every tile
    //    for (int i = 0; i < gc.height; i++)
    //    {
    //        for (int j = 0; j < gc.width; j++)
    //        {
    //            if (gc.Grid[i,j].selected)
    //            {
    //                foreach (string id in trainer.tileAssociations.Keys)
    //                {
    //                    wfcGrid[i,j].AddPossibleTileId(id);
    //                }
    //            }
    //        }
    //    }

    //    // First, collapse each non-selected tile before anything else.
    //    while (!IsGridCollapsedAsMuchAsPossible())
    //    {
    //        CollapseLeastEntropy();
    //    }




    //    //-----------------
    //    #region ignore for now
    //    //    for (int i = 0; i < gc.height; i++)
    //    //    {
    //    //        for (int j = 0; j < gc.width; j++)
    //    //        {
    //    //            Tile currGridClassTile = gc.Grid[i, j];
    //    //            // Two cases: Selected/Not Selected
    //    //            // 1) NOT Selected: Collapse (single possible tile id)
    //    //            if (!currGridClassTile.selected)
    //    //            {
    //    //                wfcGrid[i, j].ClearPossibleTileIds();
    //    //                wfcGrid[i, j].AddPossibleTileId(currGridClassTile.Id);
    //    //            }
    //    //            // 2) Selected: Add possible tiles according to neighbours
    //    //            else
    //    //            {
    //    //                List<string> possibleTileIds = FindPossibleTileIds(gc, i, j);
    //    //                wfcGrid[i, j].AddPossibleTileIds(possibleTileIds);
    //    //            }
    //    //        }
    //    //    }
    //    //}

    //    //// Obtain tiles valid for all neighbouring tiles.
    //    //private List<string> FindPossibleTileIds(GridClass gc, int i, int j)
    //    //{
    //    //    List<string> possibleTileIds = new List<string>();

    //    //    // Get allowed tiles from first valid neighbour,
    //    //    // then check if its allowed by the others.
    //    //    bool isInitiallyFilled = false;
    //    //    //UP
    //    //    if (i > 0)
    //    //    {
    //    //        Tile neighbourTile = gc.Grid[i - 1, j];
    //    //        // "Selected" tiles do not interfere
    //    //        if (!neighbourTile.selected)
    //    //        {
    //    //            List<string> allowedNeighbourss = trainer.GetAllowedNeighbours(
    //    //                neighbourTile.Id,
    //    //                WFCManager.WFCDirection.DOWN); // Opposing side
    //    //            possibleTileIds.AddRange(allowedNeighbourss);
    //    //            isInitiallyFilled = true;
    //    //        }
    //    //    }
    //    //    // DOWN
    //    //    if (i < gc.height - 1)
    //    //    {
    //    //        Tile neighbourTile = gc.Grid[i + 1, j];
    //    //        // Selected tiles are considered empty
    //    //        if (!neighbourTile.selected)
    //    //        {
    //    //            List<string> allowedNeighbours = trainer.GetAllowedNeighbours(
    //    //                neighbourTile.Id,
    //    //                WFCManager.WFCDirection.UP); // Opposing side
    //    //            if (!isInitiallyFilled)
    //    //            {
    //    //                possibleTileIds.AddRange(allowedNeighbours);
    //    //                isInitiallyFilled = true;
    //    //            }
    //    //            else
    //    //            {
    //    //                List<string> toRemove = new List<string>();
    //    //                // Obtain removable
    //    //                foreach (string possibleTileId in possibleTileIds)
    //    //                {
    //    //                    if (!allowedNeighbours.Contains(possibleTileId))
    //    //                    {
    //    //                        toRemove.Add(possibleTileId);
    //    //                    }
    //    //                }
    //    //                // Remove
    //    //                foreach (string idToRemove in toRemove)
    //    //                {
    //    //                    possibleTileIds.Remove(idToRemove);
    //    //                }
    //    //            }
    //    //        }

    //    //    }
    //    //    // LEFT
    //    //    if (j > 0)
    //    //    {
    //    //        Tile neighbourTile = gc.Grid[i, j - 1];
    //    //        // Selected tiles are considered empty
    //    //        if (!neighbourTile.selected)
    //    //        {
    //    //            List<string> allowedNeighbours = trainer.GetAllowedNeighbours(
    //    //                neighbourTile.Id,
    //    //                WFCManager.WFCDirection.RIGHT); // Opposing side
    //    //            if (!isInitiallyFilled)
    //    //            {
    //    //                possibleTileIds.AddRange(allowedNeighbours);
    //    //                isInitiallyFilled = true;
    //    //            }
    //    //            else
    //    //            {
    //    //                List<string> toRemove = new List<string>();
    //    //                // Obtain removable
    //    //                foreach (string possibleTileId in possibleTileIds)
    //    //                {
    //    //                    if (!allowedNeighbours.Contains(possibleTileId))
    //    //                    {
    //    //                        toRemove.Add(possibleTileId);
    //    //                    }
    //    //                }
    //    //                // Remove
    //    //                foreach (string idToRemove in toRemove)
    //    //                {
    //    //                    possibleTileIds.Remove(idToRemove);
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //    // RIGHT
    //    //    if (j < gc.width - 1)
    //    //    {
    //    //        Tile neighbourTile = gc.Grid[i, j + 1];
    //    //        // Selected tiles are considered empty
    //    //        if (!neighbourTile.selected)
    //    //        {
    //    //            List<string> allowedNeighbours = trainer.GetAllowedNeighbours(
    //    //                neighbourTile.Id,
    //    //                WFCManager.WFCDirection.LEFT); // Opposing side
    //    //            if (!isInitiallyFilled)
    //    //            {
    //    //                possibleTileIds.AddRange(allowedNeighbours);
    //    //                isInitiallyFilled = true;
    //    //            }
    //    //            else
    //    //            {
    //    //                List<string> toRemove = new List<string>();
    //    //                // Obtain removable
    //    //                foreach (string possibleTileId in possibleTileIds)
    //    //                {
    //    //                    if (!allowedNeighbours.Contains(possibleTileId))
    //    //                    {
    //    //                        toRemove.Add(possibleTileId);
    //    //                    }
    //    //                }
    //    //                // Remove
    //    //                foreach (string idToRemove in toRemove)
    //    //                {
    //    //                    possibleTileIds.Remove(idToRemove);
    //    //                }
    //    //            }
    //    //        }
    //    //    } 

    //    //// Return tiles allowed by every side.
    //    //return possibleTileIds;
    //    #endregion

    //    return null;
    //}
    #endregion

    private bool IsGridCollapsedAsMuchAsPossible()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!Grid[i, j].IsCollapsed() && Grid[i, j].CanBeCollapsed())
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsAnyTileSelected(GridClass gc)
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (gc.Grid[i, j].selected == true) return true;
            }
        }
        return false;
    }

    private void ResetNonSelectedTileIds()
    {
        int resetCounter = 0;
        int entropyZeroCounter = 0;
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Tile currGCTile = currentGridClass.Grid[i, j];
                if (!currGCTile.selected &&
                    (Grid[i, j].GetEntropy() == 0 || currGCTile.Id != Grid[i, j].GetPossibleTileIds()[0]))
                {
                    //Counters
                    resetCounter++;
                    if (Grid[i, j].GetEntropy() == 0) { entropyZeroCounter++; }

                    //Collapse to original id
                    Grid[i, j].CollapseWithoutPropagation(currGCTile.Id);
                }
            }
        }
        Debug.Log($"{resetCounter} non-selected tiles were reset to their originals when finishing. {entropyZeroCounter} of them had zero entropy.");
    }

    // Check for uncollapsable tiles (entropy=0)
    private bool IsAnyTileUncollapsable()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!Grid[i, j].CanBeCollapsed())
                {
                    //Debug.LogWarning($"IsAnyTileUncollapsable(): Tile {i},{j}," +
                    //                 $"originally {imageManager.db.GetImage(currentGridClass.Grid[i, j].Id).sprite.name}" +
                    //                 $" is uncollapsable!");
                    return true;
                }
            }
        }
        return false;
    }

    private void Clear()
    {
        Grid = new WFCTile[Height, Width];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Grid[i, j] = new WFCTile(i, j);
            }
        }
    }
}
