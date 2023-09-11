using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;

public class UnitDealtDamageArgs : EventArgs
{
    public UnitBehaviour reciever;
    public int damage;
}

public class UnitRecievedDamageArgs : EventArgs
{
    public UnitBehaviour dealer;
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

    protected LevelController levelController;

    protected Unit.Stats stats;
    protected bool inputGiven;
    protected bool isUnitsTurn;
    protected Unit unitData;
    protected List<List<Cell>> movementSet;
    protected bool canMove;

    protected Dictionary<Cell, GameObject> gridCells;

    protected TurnList<Turn> turns;
    protected TurnList<Turn> subTurns;

    protected UnitBehaviour targettedUnit;
    protected Cell targettedCell;

    protected List<List<Cell>> cellSet;

    protected LayerMask unitLayer;
    protected Color highlightColor;

    public delegate IEnumerator TriggerSubTurnDelegate();
    public TriggerSubTurnDelegate TriggerSubTurn;

    public static event EventHandler OnAnyUnitHPZero;
    public static event EventHandler<UnitDealtDamageArgs> OnAnyUnitDealtDamage;
    public static event EventHandler<UnitRecievedDamageArgs> OnAnyUnitRecievedDamage;
    public static event EventHandler<Cell> OnAnyUnitDead;

    public Unit.Stats Stats => stats;
    public Cell CurrentCell => gridCells.GetCellFromWorldPosition(new Vector2(transform.position.x, transform.position.z));

    public void Initialize(Unit unitData, Unit.Stats baseStats, ref TurnList<Turn> turns, ref TurnList<Turn> subTurns, ref Dictionary<Cell, GameObject> gridCells)
    {
        levelController = LevelController.instance;

        this.unitData = unitData;
        stats = new Unit.Stats(baseStats);

        this.gridCells = gridCells;

        this.turns = turns;
        this.subTurns = subTurns;

        stats.HealthHandler.OnEmpty += HealthHandler_OnEmpty;

        unitLayer = levelController.UnitLayer;
        highlightColor = levelController.HighLightColor;

        LevelController.OnTurnBegan += LevelController_OnTurnBegan;
    }

    private void OnDestroy()
    {
        stats.HealthHandler.OnEmpty -= HealthHandler_OnEmpty;
        LevelController.OnTurnBegan -= LevelController_OnTurnBegan;
    }

    private void LevelController_OnTurnBegan(object sender, Turn turn)
    {
        if (turn.TurnObject == gameObject)
        {
            canMove = true;
        }
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

        GetPossibleMovementTileParams movementSettings = new GetPossibleMovementTileParams()
        {
            currentPosition = gridCells.GetCellFromWorldPosition(new Vector2(transform.position.x, transform.position.z)),
            settings = LevelController.instance.GridSettings
        };

        cellSet = GetPossibleMovementTiles(movementSettings, out List<List<Cell>> modifiedCells);

        if (!canMove)
        {
            for (int x = cellSet.Count - 1; x >= 0; x--)
            {
                for (int y = cellSet[x].Count - 1; y >= 0; y--)
                {
                    Cell cell = cellSet[x][y];
                    Collider[] colliders = Physics.OverlapSphere(new Vector3(cell.WorldCoords.x, 0f, cell.WorldCoords.y), 0.75f, unitLayer);

                    if (colliders.Length == 0)
                    {
                        cellSet[x].Remove(cell);
                    }
                }

                if (cellSet[x].Count == 0)
                {
                    cellSet.Remove(cellSet[x]);
                }
            } 
        }

        if (cellSet.Count == 0)
        {
            EndTurn();
            return;
        }

        PaintValidMovementNodes();

        if (!isPlayerControlled)
        {
            List<Cell> direction = new List<Cell>();

            int counter = 10;
            while (direction.Count <= 0 || counter > 0)
            {
                direction = cellSet.GetRandomElement();
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
        if (teamIndex == 0)
        {
            Node.OnAnyNodeSelected -= Node_OnAnyNodeSelected;
        }

        Collider[] colliders = Physics.OverlapSphere(new Vector3(cell.WorldCoords.x, 0f, cell.WorldCoords.y), 0.75f, unitLayer);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                UnitBehaviour unitBehaviour = colliders[i].GetComponent<UnitBehaviour>();
                if (unitBehaviour != null)
                {
                    unitBehaviour.DamageUnit(this, stats.Damage);
                    targettedUnit = unitBehaviour;
                    targettedCell = cell;
                    if (unitBehaviour.stats.Health <= 0f)
                    {
                        TriggerSubTurn = KilledAUnit;
                        subTurns.Add(new Turn(0, gameObject));
                    }

                    //OnAnyUnitDealtDamage?.Invoke(this, new UnitDealtDamageArgs()
                    //{
                    //    reciever = unitBehaviour,
                    //    damage = stats.Damage
                    //});

                    EndTurn();
                    break;
                }
            }
        }
        else
        {
            canMove = false;

            subTurns.Add(new Turn(0, gameObject));
            TriggerSubTurn = TurnAction;

            MoveToCell(cell, () =>
            {
                EndTurn();
            });
        }
    }

    public void DamageUnit(UnitBehaviour inflictor, int amount)
    {
        stats.HealthHandler.Modify(ModifyType.Subtract, amount);
        OnAnyUnitRecievedDamage(this, new UnitRecievedDamageArgs()
        {
            dealer = inflictor,
            damage = amount
        });
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



        for (int x = 0; x < cellSet.Count; x++)
        {
            for (int y = 0; y < cellSet[x].Count; y++)
            {
                gridCells[cellSet[x][y]].GetComponentInChildren<MeshRenderer>().material.color = Color.grey;
            }
        }
    }


    private void PaintValidMovementNodes()
    {
        for (int x = 0; x < cellSet.Count; x++)
        {
            for (int y = 0; y < cellSet[x].Count; y++)
            {
                gridCells[cellSet[x][y]].GetComponentInChildren<MeshRenderer>().material.color = highlightColor;
            }
        }
    }

    protected void EndTurn()
    {
        inputGiven = true;
    }
}
