using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;

public static class UnitMovement
{
    public static List<List<Cell>> GetRookMovementTiles(Cell currentCell, GridSettings gridSettings, Dictionary<Cell, GameObject> gridCells)
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

    public static List<List<Cell>> GetBishopMovementTiles(Cell currentCell, GridSettings gridSettings, Dictionary<Cell, GameObject> gridCells)
    {
        List<Cell> topLeft = new List<Cell>();
        for (int x = currentCell.GridCoords.x - 1, y = currentCell.GridCoords.y + 1; x >= 0 && y < gridSettings.Size.y; x--, y++)
        {
            topLeft.Add(gridCells.GetCellFromGrid(x, y));
        }

        List<Cell> topRight = new List<Cell>();
        for (int x = currentCell.GridCoords.x + 1, y = currentCell.GridCoords.y + 1; x < gridSettings.Size.x && y < gridSettings.Size.y; x++, y++)
        {
            topRight.Add(gridCells.GetCellFromGrid(x, y));
        }

        List<Cell> bottomLeft = new List<Cell>();
        for (int x = currentCell.GridCoords.x - 1, y = currentCell.GridCoords.y - 1; x >= 0 && y >= 0; x--, y--)
        {
            bottomLeft.Add(gridCells.GetCellFromGrid(x, y));
        }

        List<Cell> bottomRight = new List<Cell>();
        for (int x = currentCell.GridCoords.x + 1, y = currentCell.GridCoords.y - 1; x < gridSettings.Size.x && y >= 0; x++, y--)
        {
            bottomRight.Add(gridCells.GetCellFromGrid(x, y));
        }

        List<List<Cell>> potentialTiles = new List<List<Cell>>()
        {
            topLeft,
            topRight,
            bottomLeft,
            bottomRight
        };

        return potentialTiles;
    }

    public static List<List<Cell>> GetKnightMovementTiles(Cell currentCell, GridSettings gridSettings, Dictionary<Cell, GameObject> gridCells)
    {
        void AddCell(int x, int y, ref List<Cell> list)
        {
            if (x.IsWithinRange(new Vector2Int(0, gridSettings.Size.x - 1)) && y.IsWithinRange(new Vector2Int(0, gridSettings.Size.y - 1)))
            {
                list.Add(gridCells.GetCellFromGrid(x, y));
            }
        }

        List<Cell> forward = new List<Cell>();
        AddCell(currentCell.GridCoords.x + 1, currentCell.GridCoords.y + 2, ref forward);
        AddCell(currentCell.GridCoords.x - 1, currentCell.GridCoords.y + 2, ref forward);

        List<Cell> backward = new List<Cell>();
        AddCell(currentCell.GridCoords.x + 1, currentCell.GridCoords.y - 2, ref backward);
        AddCell(currentCell.GridCoords.x - 1, currentCell.GridCoords.y - 2, ref backward);

        List<Cell> left = new List<Cell>();
        AddCell(currentCell.GridCoords.x - 2, currentCell.GridCoords.y + 1, ref left);
        AddCell(currentCell.GridCoords.x - 2, currentCell.GridCoords.y - 1, ref left);

        List<Cell> right = new List<Cell>();
        AddCell(currentCell.GridCoords.x + 2, currentCell.GridCoords.y + 1, ref right);
        AddCell(currentCell.GridCoords.x + 2, currentCell.GridCoords.y - 1, ref right);

        List<List<Cell>> potentialTiles = new List<List<Cell>>()
        {
            forward,
            backward,
            left,
            right
        };

        return potentialTiles;
    }

}
