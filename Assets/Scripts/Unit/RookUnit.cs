using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class RookUnit : UnitBehaviour
{
    public override List<List<Cell>> GetPossibleMovementTiles(GetPossibleMovementTileParams movementParams, out List<List<Cell>> modifiedSet)
    {
        movementSet = UnitMovement.GetRookMovementTiles(movementParams.currentPosition, movementParams.settings, gridCells);

        for (int i = 0; i < movementSet.Count; i++)
        {
            for (int j = 0; j < movementSet[i].Count; j++)
            {
                Vector3 position = new Vector3(movementSet[i][j].WorldCoords.x, 0f, movementSet[i][j].WorldCoords.y);

                Collider[] colliders = Physics.OverlapSphere(position, 0.5f, unitLayer);
                for (int k = 0; k < colliders.Length; k++)
                {
                    UnitBehaviour unitBehaviour = colliders[k].GetComponent<UnitBehaviour>();

                    if (unitBehaviour.teamIndex == teamIndex)
                    {
                        movementSet[i].RemoveRange(j, movementSet[i].Count - j);
                    }
                    else
                    {
                        movementSet[i].RemoveRange(j + 1, movementSet[i].Count - j - 1);
                    }
                }
            }
        }

        modifiedSet = movementSet;
        return movementSet;
    }
}
