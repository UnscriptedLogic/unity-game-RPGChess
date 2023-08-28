using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;

public class UnitBehaviour : MonoBehaviour
{
    public bool isPlayerControlled;

    protected Unit.Stats stats;
    protected bool inputGiven;
    protected bool isUnitsTurn;
    protected Unit unitData;
    protected List<List<Cell>> movementSet;

    public Unit.Stats Stats => stats;

    private void HealthHandler_OnModified(object sender, IntHandlerEventArgs e)
    {
        if (e.currentValue <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Unit unitData, Unit.Stats baseStats)
    {
        this.unitData = unitData;
        stats = new Unit.Stats(baseStats);

        stats.HealthHandler.OnModified += HealthHandler_OnModified;
    }

    private void Update()
    {
        if (!isUnitsTurn) return;
        if (!isPlayerControlled) return;

        if (Input.GetMouseButtonDown(0))
        {
            inputGiven = true;
        }
    }

    public virtual List<List<Cell>> GetPossibleMovementTiles(Cell currentPosition, GridSettings settings, Dictionary<Cell, GameObject> gridCells)
    {
        movementSet = new List<List<Cell>>();

        switch (unitData.UnitMovementType)
        {
            case Unit.MovementType.Bishop:
                movementSet = UnitMovement.GetBishopMovementTiles(currentPosition, settings, gridCells);
                break;
            case Unit.MovementType.Knight:
                movementSet = UnitMovement.GetKnightMovementTiles(currentPosition, settings, gridCells);
                break;
            case Unit.MovementType.Rook:
                movementSet = UnitMovement.GetRookMovementTiles(currentPosition, settings, gridCells);
                break;
            case Unit.MovementType.None:
                break;
        }

        return movementSet;
    }

    public IEnumerator TurnAction()
    {
        OnTurnBegin();

        yield return new WaitUntil(() => inputGiven);
        inputGiven = false;

        OnTurnEnd();
    }

    protected virtual void OnTurnBegin()
    {
        isUnitsTurn = true;

        if (!isPlayerControlled)
        {
            inputGiven = true;
        }
    }
    
    protected virtual void OnTurnEnd() 
    {
        isUnitsTurn = false;

        List<Cell> direction = new List<Cell>();

        int counter = 10;
        while (direction.Count <= 0 || counter > 0)
        {
            direction = movementSet.GetRandomElement();
            counter--;
        }

        Cell cell = direction.GetRandomElement();

        Vector2 worldCoords = cell.WorldCoords;
        transform.position = new Vector3(worldCoords.x, transform.position.y, worldCoords.y);
        
        stats.HealthHandler.Modify(ModifyType.Subtract, 1);
    }
}
