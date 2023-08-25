using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class UnitBehaviour : MonoBehaviour
{
    public bool isPlayerControlled;

    protected Unit.Stats stats;
    protected bool inputGiven;
    protected bool isUnitsTurn;
    protected Unit unitData;

    public Unit.Stats Stats => stats;

    public void Initialize(Unit unitData, Unit.Stats baseStats)
    {
        this.unitData = unitData;
        stats = baseStats;
    }

    private void Update()
    {
        if (!isUnitsTurn) return;

        if (Input.GetMouseButtonDown(0))
        {
            inputGiven = true;
        }
    }

    public virtual List<List<Cell>> GetPossibleMovementTiles(Cell currentPosition, GridSettings settings, Dictionary<Cell, GameObject> gridCells)
    {
        return UnitMovement.GetPossibleRookTiles(currentPosition, settings, gridCells);
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
    }
    
    protected virtual void OnTurnEnd() 
    {
        isUnitsTurn = false;
    }
}
