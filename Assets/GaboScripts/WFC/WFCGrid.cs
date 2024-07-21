using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
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
    private int uncollapsableReplacementCount = 0;

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

    // Returns WFC-modified grid. Can be filtered by category name.
    public GridClass GetWFC(GridClass gc, string category = null)
    {
        //DEBUG
        category = "Furniture";
        //FIN DEBUG

        // Make class-wide reference to current GridClass parameter.
        currentGridClass = gc;

        // Check if any tiles selected, otherwise return as is.
        if (!IsAnyTileSelected(gc)) { Debug.LogWarning("No tiles selected."); return gc; }

        // Initial fill of tiles
        Fill(gc);

        // If category was given, filter:
        if (category != null)
        {
            FilterByCategory(category);
        }

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

        // if selected tiles were replaced to "none" due to being uncollapsable, return to original ids.
        ReplaceUncollapsablesWithOriginals();

        // If non-selected tiles were modified, they recover their original ids.
        ResetNonSelectedTileIds();

        // DEBUG: Info
        Debug.Log($"{uncollapsableReplacementCount} tile replacements were done.");

        // Assign values to GridClass
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (Grid[i, j].IsEntropyMoreThanZero() && Grid[i, j].IsCollapsed())
                {
                    gc.Grid[i, j].Id = Grid[i, j].GetPossibleTileIds()[0];
                }
                else if (Grid[i, j].IsEntropyMoreThanZero() && !Grid[i, j].IsCollapsed())
                {
                    Debug.LogError($"Error: WFCTile {i},{j} remains collapsable after exiting cycle!");
                }
                else if (!Grid[i, j].IsEntropyMoreThanZero())
                {
                    Debug.Log($"Tile {i},{j} still can't be collapsed after finishing!");
                }
            }
        }

        // Return modified GridClass
        return gc;
    }

    // Filter according to category V2.0
    private void FilterByCategory(string category)
    {
        // Obtain ImageDnd's from the chosen category
        List<ImageDnd> chosenRawImages = imageManager.db.categories.Find(cat => cat.categoryName == category).images;
        List<ImageDnd> chosenSubImages = new();
        // Add sub-images of each macro-tile image
        foreach (ImageDnd chosenRawImage in chosenRawImages)
        {
            // if single tile, add itself.
            if (chosenRawImage.rows == 1 && chosenRawImage.columns == 1)
            {
                chosenSubImages.Add(chosenRawImage);
            }
            // Else, add sub-images
            else
            {
                foreach (string chosenSubImageId in chosenRawImage.subImageIds)
                {
                    chosenSubImages.Add(imageManager.db.GetImage(chosenSubImageId));
                }
            }
        }

        // Remove possibilities that are not in chosen images
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!Grid[i, j].IsCollapsed() && Grid[i, j].IsEntropyMoreThanZero())
                {
                    Stack<string> toRemove = new();
                    List<string> currentPossibleIds = Grid[i, j].GetPossibleTileIds();
                    foreach (string possibilityId in currentPossibleIds)
                    {
                        // Check if possibility exists in chosen sub-images. "None" is an exception.
                        if (possibilityId != "none" && !chosenSubImages.Exists(img => img.Id == possibilityId))
                        {
                            toRemove.Push(possibilityId);
                        }
                    }
                    // Remove unchosen possibilities from the tile
                    foreach (string removableId in toRemove)
                    {
                        Grid[i, j].RemovePossibleTileId(removableId);
                    }
                    // Uncollapsable check
                    if (!Grid[i, j].IsEntropyMoreThanZero())
                    {
                        // If uncollapsable, replace temporarily with "None".
                        Grid[i, j].CollapseWithoutPropagation("none");//currentGridClass.Grid[i, j].Id);
                        Grid[i, j].IsReplacedDuetoUncollapsability = true;
                        Debug.LogWarning($"FilterByCategory(): Uncollapsable filtered tile at {i},{j} replaced temporarily with \"none\" to continue propagation.");
                        uncollapsableReplacementCount++;
                    }
                }
            }
        }
    }

    #region FAILED FilterByCategory 1.0
    //// Filter according to category
    //private void FilterByCategory(string category)
    //{
    //    // Obtain ImageDnd's from the chosen category
    //    List<ImageDnd> chosenImages = imageManager.db.categories.Find(cat => cat.categoryName == category).images;
    //    // Create trainer copy and set as new trainer (to avoid modifying the original)
    //    trainer = trainer.AssociationsAndFrequenciesOnlyDeepCopy();
    //    // Remove any tile associations which lead to non-chosen-category tiles...
    //    // 1. ...From the trainer
    //    foreach (string trainerId in trainer.tileAssociations.Keys)
    //    {
    //        Stack<WFCTrainer.AssociationTuple> toRemove = new();
    //        for (int i = 0; i < trainer.tileAssociations[trainerId].Count; i++)
    //        {
    //            if (!chosenImages.Exists(img => img.Id == trainer.tileAssociations[trainerId][i].id))
    //            {
    //                toRemove.Push(trainer.tileAssociations[trainerId][i]);
    //            }
    //        }
    //        // Remove unchosen tuples from the associations
    //        foreach (WFCTrainer.AssociationTuple removableTuple in toRemove)
    //        {
    //            trainer.tileAssociations[trainerId].Remove(removableTuple);
    //        }
    //    }

    //    // 2. ...From the current tiles
    //    for (int i = 0; i < Height; i++)
    //    {
    //        for (int j = 0; j < Width; j++)
    //        {
    //            Stack<string> toRemove = new();
    //            List<string> currentPossibleIds = Grid[i, j].GetPossibleTileIds();
    //            foreach (string possibilityId in currentPossibleIds)
    //            {
    //                if (!chosenImages.Exists(img => img.Id == possibilityId))
    //                {
    //                    toRemove.Push(possibilityId);
    //                }
    //            }
    //            // Remove unchosen possibilities from the tile
    //            foreach (string removableId in toRemove)
    //            {
    //                Grid[i, j].RemovePossibleTileId(removableId);
    //            }
    //        }
    //    }
    //}
    #endregion

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
                if (currTile.IsCollapsed() || !currTile.IsEntropyMoreThanZero()) { continue; }
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
    private void CollapseTile(int i, int j, string id = null)
    {
        if (Grid[i, j].IsEntropyMoreThanZero() && !Grid[i, j].IsCollapsed())
        {
            // Id given or not?
            if (id != null)
            {
                Grid[i, j].CollapseWithoutPropagation(id); // If id given, collapse to it
                Propagate(Grid[i, j]);
            }
            else
            {
                Grid[i, j].CollapseWithoutPropagation(); // Else, choose from between remaining options
                Propagate(Grid[i, j]);
            }
        }
        // If tile given is already collapsed, propagate. (TODO: Ver si funca)
        else if (Grid[i, j].IsCollapsed())
        {
            Propagate(Grid[i, j]);
        }
        // If tile entropy is zero for whatever reason, abort //TODO: Ver si funciona
        else if (!Grid[i, j].IsEntropyMoreThanZero())
        {
            //Debug.LogWarning($"CollapseTile(): Tile {i},{j}," +
            //                 $"originally {imageManager.db.GetImage(currentGridClass.Grid[i, j].Id).sprite.name}" +
            //                 $"is uncollapsable!");
            return;
        }
    }

    # region Failed propagation for uncollapsable tile s
    //// Propagate an uncollapsable tile that got fixed with the original content.
    //// Unlike Propagate(): Collapsed tiles, if selected, are considered anyway,
    //// in case they need fixing.
    //// ERROR: loop infinito!!!!!!!!!!!!*****************
    //private void PropagateReplacedUncollapsable(WFCTile replacedTile)
    //{
    //    Queue<WFCTile> pending = new();
    //    pending.Enqueue(replacedTile);

    //    while (pending.Count > 0)// && debugSteps > 0)
    //    {
    //        WFCTile currTile = pending.Dequeue();
    //        // Propagate, but including already collapsed tiles
    //        // UP
    //        if (currTile.i > 0)
    //        {
    //            WFCTile neighbourTile = Grid[currTile.i - 1, currTile.j];
    //            if (currentGridClass.Grid[currTile.i - 1, currTile.j].selected && neighbourTile.IsEntropyMoreThanZero())
    //            {
    //                bool shouldBeQueued = DirectionalSinglePropagation(
    //                    currTile, WFCManager.WFCDirection.UP);
    //                if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
    //            }
    //        }
    //        // DOWN
    //        if (currTile.i < Height - 1)
    //        {
    //            WFCTile neighbourTile = Grid[currTile.i + 1, currTile.j];
    //            if (currentGridClass.Grid[currTile.i + 1, currTile.j].selected && neighbourTile.IsEntropyMoreThanZero())
    //            {
    //                bool shouldBeQueued = DirectionalSinglePropagation(
    //                    currTile, WFCManager.WFCDirection.DOWN);
    //                if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
    //            }
    //        }
    //        // LEFT
    //        if (currTile.j > 0)
    //        {
    //            WFCTile neighbourTile = Grid[currTile.i, currTile.j - 1];
    //            if (currentGridClass.Grid[currTile.i, currTile.j - 1].selected && neighbourTile.IsEntropyMoreThanZero())
    //            {
    //                bool shouldBeQueued = DirectionalSinglePropagation(
    //                    currTile, WFCManager.WFCDirection.LEFT);
    //                if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
    //            }
    //        }
    //        // RIGHT
    //        if (currTile.j < Width - 1)
    //        {
    //            WFCTile neighbourTile = Grid[currTile.i, currTile.j + 1];
    //            if (currentGridClass.Grid[currTile.i, currTile.j + 1].selected && neighbourTile.IsEntropyMoreThanZero())
    //            {
    //                bool shouldBeQueued = DirectionalSinglePropagation(
    //                    currTile, WFCManager.WFCDirection.RIGHT);
    //                if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
    //            }
    //        }
    //    }
    //}
    #endregion

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
                if (!neighbourTile.IsCollapsed() && neighbourTile.IsEntropyMoreThanZero())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.UP);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
            }
            // DOWN
            if (currTile.i < Height - 1)
            {
                WFCTile neighbourTile = Grid[currTile.i + 1, currTile.j];
                if (!neighbourTile.IsCollapsed() && neighbourTile.IsEntropyMoreThanZero())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.DOWN);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
            }
            // LEFT
            if (currTile.j > 0)
            {
                WFCTile neighbourTile = Grid[currTile.i, currTile.j - 1];
                if (!neighbourTile.IsCollapsed() && neighbourTile.IsEntropyMoreThanZero())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.LEFT);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
                }
            }
            // RIGHT
            if (currTile.j < Width - 1)
            {
                WFCTile neighbourTile = Grid[currTile.i, currTile.j + 1];
                if (!neighbourTile.IsCollapsed() && neighbourTile.IsEntropyMoreThanZero())
                {
                    bool shouldBeQueued = DirectionalSinglePropagation(
                        currTile, WFCManager.WFCDirection.RIGHT);
                    if (shouldBeQueued) { pending.Enqueue(neighbourTile); }
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
        if (!objective.IsEntropyMoreThanZero())
        {
            // If uncollapsable, replace with "none" temporarily
            objective.CollapseWithoutPropagation("none");//"currentGridClass.Grid[objective.i, objective.j].Id);
            objective.IsReplacedDuetoUncollapsability = true;
            Debug.LogWarning($"Collapsing uncollapsable tile at {objective.i},{objective.j} " +
                $"temporarily to \"none\" to continue propagation, due to {origin.i}{origin.j}");
            uncollapsableReplacementCount++;
        }

        return objectiveShouldbeQueued;
    }

    // Fill V2.0
    private void Fill(GridClass gc)
    {
        // Clear tiles
        Clear();

        // Selected tiles start with every possibility.
        // Non-selected tiles start collapsed with their set tile,
        // without propagation (for now).
        for (int i = 0; i < gc.height; i++)
        {
            for (int j = 0; j < gc.width; j++)
            {
                if (gc.Grid[i, j].selected)
                {
                    foreach (string id in trainer.tileAssociations.Keys)
                    {
                        Grid[i, j].AddPossibleTileId(id);
                    }
                }
                else
                {
                    Grid[i, j].CollapseWithoutPropagation(gc.Grid[i, j].Id);
                }
            }
        }

        // Iterate propagation of 'non-selected', now collapsed, tiles.
        // This will leave only the 'selected' tiles to be collapsed
        // (E.g.: By least entropy, in some other function).
        for (int i = 0; i < gc.height; i++)
        {
            for (int j = 0; j < gc.width; j++)
            {
                if (!gc.Grid[i, j].selected)
                {
                    CollapseTile(i, j, gc.Grid[i, j].Id);
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

    // Replace tiles marked with "none" due to being uncollapsable with their original tiles.
    // Note: To be used after all propagation has been done.
    private void ReplaceUncollapsablesWithOriginals()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (Grid[i, j].IsReplacedDuetoUncollapsability)
                {
                    Grid[i, j].CollapseWithoutPropagation(currentGridClass.Grid[i,j].Id);
                }
            }
        }
    }

    private bool IsGridCollapsedAsMuchAsPossible()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!Grid[i, j].IsCollapsed() && Grid[i, j].IsEntropyMoreThanZero())
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
        //int entropyZeroCounter = 0;
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
                    //if (Grid[i, j].GetEntropy() == 0) { entropyZeroCounter++; }

                    //Collapse to original id
                    Grid[i, j].CollapseWithoutPropagation(currGCTile.Id);
                }
            }
        }
        Debug.Log($"{resetCounter} non-selected tiles were reset to their originals when finishing."); //{entropyZeroCounter} of them had zero entropy.");
    }

    // Check for uncollapsable tiles (entropy=0)
    private bool IsAnyTileUncollapsable()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (!Grid[i, j].IsEntropyMoreThanZero())
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
