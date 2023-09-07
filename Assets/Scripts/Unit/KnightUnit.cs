using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class KnightUnit : UnitBehaviour
{
    public override List<List<Cell>> GetPossibleMovementTiles(GetPossibleMovementTileParams movementParams, out List<List<Cell>> modifiedSet)
    {
        movementSet = UnitMovement.GetKnightMovementTiles(movementParams.currentPosition, movementParams.settings, gridCells);

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
                        movementSet[i].RemoveAt(j);
                    }
                }
            }
        }

        modifiedSet = movementSet;
        return movementSet;
    }
}
