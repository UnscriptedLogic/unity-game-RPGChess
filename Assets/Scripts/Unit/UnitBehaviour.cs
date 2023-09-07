using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;

public class UnitDealtDamageArgs : EventArgs
{
    public UnitBehaviour reciever;
    public int damage;
}

public class UnitBehaviour : MonoBehaviour
{
    public class GetPossibleMovementTileParams
    {
        public Cell currentPosition;
        public GridSettings settings;
    }

    [SerializeField] protected bool isPlayerControlled;
    public int teamIndex;

    protected Unit.Stats stats;
    protected bool inputGiven;
    protected bool isUnitsTurn;
    protected Unit unitData;
    protected List<List<Cell>> movementSet;

    protected Dictionary<Cell, GameObject> gridCells;

    protected TurnList<Turn> turns;
    protected TurnList<Turn> subTurns;

    protected UnitBehaviour targettedUnit;
    protected Cell targettedCell;

    [SerializeField] protected LayerMask unitLayer;

    public delegate IEnumerator TriggerSubTurnDelegate();
    public TriggerSubTurnDelegate TriggerSubTurn;

    public static event EventHandler OnAnyUnitHPZero;
    public static event EventHandler<UnitDealtDamageArgs> OnAnyUnitDealtDamage;
    public static event EventHandler<Cell> OnAnyUnitDead;

    public Unit.Stats Stats => stats;
    public Cell CurrentCell => gridCells.GetCellFromWorldPosition(new Vector2(transform.position.x, transform.position.z));

    public void Initialize(Unit unitData, Unit.Stats baseStats, ref TurnList<Turn> turns, ref TurnList<Turn> subTurns, ref Dictionary<Cell, GameObject> gridCells)
    {
        this.unitData = unitData;
        stats = new Unit.Stats(baseStats);

        this.gridCells = gridCells;

        this.turns = turns;
        this.subTurns = subTurns;

        stats.HealthHandler.OnEmpty += HealthHandler_OnEmpty;
    }

    private void HealthHandler_OnEmpty(object sender, EventArgs e)
    {
        TriggerSubTurn = UnitKilled;
        subTurns.Add(new Turn(0, gameObject));

        OnAnyUnitHPZero?.Invoke(this, EventArgs.Empty);
    }

    public virtual List<List<Cell>> GetPossibleMovementTiles(GetPossibleMovementTileParams movementParams, out List<List<Cell>> modifiedSet)
    {
        movementSet = new List<List<Cell>>();

        modifiedSet = movementSet;
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
            List<Cell> direction = new List<Cell>();

            int counter = 10;
            while (direction.Count <= 0 || counter > 0)
            {
                direction = movementSet.GetRandomElement();
                counter--;
            }

            Cell cell = direction.GetRandomElement();

            InteractWithCell(cell);
        }
        else
        {
            Node.OnAnyNodeSelected += Node_OnAnyNodeSelected;
        }
    }

    protected void InteractWithCell(Cell cell)
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(cell.WorldCoords.x, 0f, cell.WorldCoords.y), 0.75f, unitLayer);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                UnitBehaviour unitBehaviour = colliders[i].GetComponent<UnitBehaviour>();
                if (unitBehaviour != null)
                {
                    unitBehaviour.stats.HealthHandler.Modify(ModifyType.Subtract, stats.Damage);
                    targettedUnit = unitBehaviour;
                    targettedCell = cell;
                    if (unitBehaviour.stats.Health <= 0f)
                    {
                        TriggerSubTurn = KilledAUnit;
                        subTurns.Add(new Turn(0, gameObject));
                    }

                    OnAnyUnitDealtDamage?.Invoke(this, new UnitDealtDamageArgs()
                    {
                        reciever = unitBehaviour,
                        damage = stats.Damage
                    });

                    inputGiven = true;
                    break;
                }
            }
        }
        else
        {
            MoveToCell(cell, () =>
            {
                inputGiven = true;
            });
        }
    }

    protected void MoveToCell(Cell cell, Action OnComplete)
    {
        Vector2 worldCoords = cell.WorldCoords;
        transform.DOMove(new Vector3(worldCoords.x, transform.position.y, worldCoords.y), 0.5f).OnComplete(() => OnComplete()).SetEase(Ease.InOutExpo);
    }

    protected void Node_OnAnyNodeSelected(object sender, EventArgs e)
    {
        Node node = (Node)sender;

        for (int i = 0; i < movementSet.Count; i++)
        {
            List<Cell> direction = movementSet[i];
            if (direction.Contains(node.Cell))
            {
                InteractWithCell(node.Cell);
                return;
            }
        }
    }

    protected virtual IEnumerator KilledAUnit()
    {
        bool hasMoved = false;
        MoveToCell(targettedCell, () =>
        {
            hasMoved = true;
        });

        yield return new WaitUntil(() => hasMoved);

        TriggerSubTurn = null;
    }

    protected virtual IEnumerator UnitKilled()
    {
        yield return null;

        OnAnyUnitDead?.Invoke(this, CurrentCell);
        Destroy(gameObject);

        TriggerSubTurn = null;
    }

    protected virtual void OnTurnEnd() 
    {
        isUnitsTurn = false;

        if (teamIndex == 0)
        {
            Node.OnAnyNodeSelected -= Node_OnAnyNodeSelected;
        }
    }
}
