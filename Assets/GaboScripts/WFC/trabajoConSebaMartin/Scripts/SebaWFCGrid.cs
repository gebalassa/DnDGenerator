using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SebaTrabajo
{
    public class WFCGrid : MonoBehaviour
    {
        enum WFCDirection { UP, DOWN, LEFT, RIGHT };

        public int height;
        public int width;
        public PossibilityTile possibilityTilePrefab;

        PossibilityTile[,] grid;
        DataContainer data;

        void Start()
        {
            // Add reference to Data Container
            data = FindObjectOfType<DataContainer>();
            // Initialize empty grid
            grid = new PossibilityTile[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[i, j] = Instantiate(possibilityTilePrefab, transform);
                    grid[i, j].transform.SetLocalPositionAndRotation(new Vector2(j, -i), Quaternion.identity);
                    grid[i, j].SetGridPosition(j, i);
                }
            }

            // Start WFC
            WFC();
        }

        void WFC()
        {
            (PossibilityTile, TileType) firstPTileTuple = SetAndCollapseFirstRandom();
            PropagateFrom(firstPTileTuple.Item1, firstPTileTuple.Item2);
            CollapseAllValid();

            int count = 5000; //TODO borrarrrr
            while (!IsCollapsed() && count != 0)//!IsCollapsed())
            {

                PossibilityTile currPTile = LeastEntropyPTile();
                int range = currPTile.GetLocalPossibleTileTypes().Count;
                TileType tileType = currPTile.GetLocalPossibleTileTypes()[Random.Range(0, range)];
                currPTile.CollapseTile(tileType);

                PropagateFrom(currPTile, tileType);
                CollapseAllValid();
                count--;
            }
            // truco sucio todo: borrar
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (!grid[i, j].isCollapsed)
                    {
                        int randIndex = Random.Range(0, grid[i, j].GetLocalPossibleTileTypes().Count);
                        grid[i, j].CollapseTile(grid[i, j].GetLocalPossibleTileTypes()[randIndex]);
                    }
                }
            }

        }


        // Set random first tile and return it with the chosen type
        (PossibilityTile, TileType) SetAndCollapseFirstRandom()
        {
            int posX = Random.Range(0, width);
            int posY = Random.Range(0, height);
            PossibilityTile firstPTile = grid[posY, posX];

            int chosenTileTypeIndex = Random.Range(0, firstPTile.GetLocalPossibleTileTypes().Count);
            TileType chosenTileType = firstPTile.GetLocalPossibleTileTypes()[chosenTileTypeIndex];
            firstPTile.CollapseTile(chosenTileType);

            return (firstPTile, chosenTileType);
        }

        void CollapseAllValid()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // DEBUG
                    if (grid[i, j].GetLocalPossibleTileTypes().Count == 0) { Debug.LogError("Cero valores posibles al intentar colapsar!"); throw new Exception(); }
                    // FIN DEBUG
                    else if (grid[i, j].GetLocalPossibleTileTypes().Count == 1 && !grid[i, j].isCollapsed)
                    {
                        grid[i, j].CollapseTile();
                    }
                    // truco sucio borrar //TODO
                    else if (grid[i, j].GetLocalPossibleTileTypes().Count < 15 && !grid[i, j].isCollapsed)
                    {
                        int randomTileTypeIndex = Random.Range(0, grid[i, j].GetLocalPossibleTileTypes().Count);
                        grid[i, j].CollapseTile(grid[i, j].GetLocalPossibleTileTypes()[randomTileTypeIndex]);
                    }
                }
            }
        }

        // NOTA****: Lo mas probable es que no haya funcionado porque
        // tendria que haber sido un Queue y NO un Stack
        void PropagateFrom(PossibilityTile pTile, TileType tileType)
        {
            Stack<PossibilityTile> pStack = new Stack<PossibilityTile>();
            pStack.Push(pTile);

            while (pStack.Count > 0)
            {
                PossibilityTile currPTile = pStack.Pop();
                bool shouldBeStacked = false;

                if (currPTile.gridI > 0) { shouldBeStacked = DirectionalSinglePropagation(currPTile, WFCDirection.UP); }
                if (shouldBeStacked) { pStack.Push(grid[currPTile.gridI - 1, currPTile.gridJ]); shouldBeStacked = false; }

                if (currPTile.gridI < height - 1) { DirectionalSinglePropagation(currPTile, WFCDirection.DOWN); }
                if (shouldBeStacked) { pStack.Push(grid[currPTile.gridI + 1, currPTile.gridJ]); shouldBeStacked = false; }

                if (currPTile.gridJ > 0) { DirectionalSinglePropagation(currPTile, WFCDirection.LEFT); }
                if (shouldBeStacked) { pStack.Push(grid[currPTile.gridI, currPTile.gridJ - 1]); shouldBeStacked = false; }

                if (currPTile.gridJ < width - 1) { DirectionalSinglePropagation(currPTile, WFCDirection.RIGHT); }
                if (shouldBeStacked) { pStack.Push(grid[currPTile.gridI, currPTile.gridJ + 1]); shouldBeStacked = false; }
            }
        }

        bool DirectionalSinglePropagation(PossibilityTile pOrigin, WFCDirection direction)
        {
            // Selección de PossibilityTile a constreñir
            PossibilityTile pObjective;
            switch (direction)
            {
                case WFCDirection.UP:
                    pObjective = grid[pOrigin.gridI - 1, pOrigin.gridJ];
                    break;
                case WFCDirection.DOWN:
                    pObjective = grid[pOrigin.gridI + 1, pOrigin.gridJ];
                    break;
                case WFCDirection.LEFT:
                    pObjective = grid[pOrigin.gridI, pOrigin.gridJ - 1];
                    break;
                case WFCDirection.RIGHT:
                    pObjective = grid[pOrigin.gridI, pOrigin.gridJ + 1];
                    break;
                default:
                    pObjective = Instantiate(possibilityTilePrefab);
                    break;
            }

            // Proceso de remoción
            List<TileType> tileTypesToRemove = new List<TileType>();
            foreach (TileType ttObjective in pObjective.GetLocalPossibleTileTypes())
            {
                bool tileTypeIsValid = false;
                foreach (TileType ttOrigin in pOrigin.GetLocalPossibleTileTypes())
                {
                    SebaTile currTile = data.GetTile(ttOrigin).GetComponent<SebaTile>();
                    List<TileType> tilesToCheck = new List<TileType>();
                    switch (direction)
                    {
                        // Top y Bottom se intercambian ya que origen en Tiles
                        // está en esq. inf. izq.*****************
                        case WFCDirection.UP:
                            tilesToCheck = currTile.GetDefaultValidBottomNeighbours();
                            break;
                        case WFCDirection.DOWN:
                            tilesToCheck = currTile.GetDefaultValidTopNeighbours();
                            break;
                        case WFCDirection.LEFT:
                            tilesToCheck = currTile.GetDefaultValidLeftNeighbours();
                            break;
                        case WFCDirection.RIGHT:
                            tilesToCheck = currTile.GetDefaultValidRightNeighbours();
                            break;
                    }
                    // Si contiene el TileType...
                    if (tilesToCheck.Contains(ttObjective))
                    {
                        tileTypeIsValid = true;
                        break;
                    }
                }
                if (!tileTypeIsValid) { tileTypesToRemove.Add(ttObjective); }
            }
            if (tileTypesToRemove.Count > 0)
            {
                foreach (TileType tt in tileTypesToRemove)
                {
                    if (pObjective.GetLocalPossibleTileTypes().Count == 1)
                    {
                        return true; // TRUCO SUCIO, //TODO: ARREGLAR
                    }
                    if (pObjective.GetLocalPossibleTileTypes().Count == 0)
                    {
                        Debug.LogError("Cero valores posibles al intentar REMOVER!");
                    }
                    pObjective.GetLocalPossibleTileTypes().Remove(tt);
                    if (pObjective.GetLocalPossibleTileTypes().Count == 0)
                    {
                        Debug.LogError("Cero valores posibles al intentar REMOVER!");
                    }
                }

                return true;
            }
            else { return false; }
        }

        // Return grid pTile with least number of possible tiles
        PossibilityTile LeastEntropyPTile()
        {
            int currLowestEntropy = 99999999;
            PossibilityTile currChosenPTile = grid[0, 0];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (grid[i, j].GetLocalPossibleTileTypes().Count < currLowestEntropy && !grid[i, j].isCollapsed)
                    {
                        currLowestEntropy = grid[i, j].GetLocalPossibleTileTypes().Count;
                        currChosenPTile = grid[i, j];
                    }
                    // Azar en caso de igual entropia
                    else if (grid[i, j].GetLocalPossibleTileTypes().Count == currLowestEntropy && !grid[i, j].isCollapsed)
                    {
                        int change = Random.Range(0, 2);
                        currChosenPTile = change == 1 ? grid[i, j] : currChosenPTile;
                    }
                }
            }
            return currChosenPTile;
        }


        // Verifies if grid is collapsed completely or not
        bool IsCollapsed()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    PossibilityTile currTile = grid[i, j];
                    if (!currTile.isCollapsed)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //

    }
}