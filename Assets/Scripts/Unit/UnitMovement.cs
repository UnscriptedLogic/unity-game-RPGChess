using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public static class UnitMovement
{
    public static List<List<Cell>> GetPossibleRookTiles(Cell currentCell, GridSettings gridSettings, Dictionary<Cell, GameObject> gridCells)
    {
        List<Cell> forward = new List<Cell>();
        for (int y = currentCell.GridCoords.y + 1; y < gridSettings.Size.y; y++)
        {
            forward.Add(gridCells.GetCellFromGrid(currentCell.GridCoords.x, y));
        }

        List<Cell> left = new List<Cell>();
        for (int x = currentCell.GridCoords.x - 1; x >= 0; x--)
        {
            left.Add(gridCells.GetCellFromGrid(x, currentCell.GridCoords.y));
        }

        List<Cell> right = new List<Cell>();
        for (int x = currentCell.GridCoords.x + 1; x < gridSettings.Size.x; x++)
        {
            right.Add(gridCells.GetCellFromGrid(x, currentCell.GridCoords.y));
        }

        List<Cell> backward = new List<Cell>();
        for (int y = currentCell.GridCoords.y - 1; y >= 0; y--)
        {
            backward.Add(gridCells.GetCellFromGrid(currentCell.GridCoords.x, y));
        }

        List<List<Cell>> potentialTiles = new List<List<Cell>>() 
        { 
            left,
            right,
            forward,
            backward
        };

        return potentialTiles;
    }
}
