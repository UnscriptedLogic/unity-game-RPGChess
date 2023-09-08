using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class TurnList<T> : IList<T> where T : Turn
{
    public List<T> turns = new List<T>();

    public T this[int index] { get => turns[index]; set => turns[index] = value; }
    public int Count => turns.Count;
    public bool IsReadOnly => false;

    public event Action<T> OnCreated;
    public event Action<T> OnInserted;
    public event Action<T> OnRemoved;

    public void Add(T item)
    {
        turns.Add(item);
        OnCreated?.Invoke(item);
    }

    public void Insert(int index, T item)
    {
        turns.Insert(index, item);
        OnInserted?.Invoke(item);
    }

    public bool Remove(T item)
    {
        bool value = turns.Remove(item);

        if (value)
        {
            OnRemoved?.Invoke(item);
        }

        return value;
    }

    public void RemoveAt(int index)
    {
        T turnToRemove = turns[index];
        turns.RemoveAt(index);
        OnRemoved?.Invoke(turnToRemove);
    }

    public void Clear() => turns.Clear();
    public bool Contains(T item) => turns.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => turns.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => turns.GetEnumerator();
    public int IndexOf(T item) => turns.IndexOf(item);
    IEnumerator IEnumerable.GetEnumerator() => turns.GetEnumerator();

    public void SortDescending()
    {
        turns.Sort((turnA, turnB) =>
        {
            if (turnA.ActionValue < turnB.ActionValue)
            {
                return 1;
            }
            else if (turnA.ActionValue > turnB.ActionValue)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        });
    }
}

public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public event EventHandler OnLevelInitialized;

    [Header("Grid Settings")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GridSettings settings;

    private GridLogic<GameObject> gridLogic;

    public GridSettings GridSettings => settings;

    [Header("Entity Settings")]
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private float instantiateOffset = 0.075f;

    private int waveIndex;

    private List<UnitData> playerTeam;
    private List<UnitData> enemyTeam;
    private List<UnitData> allTeam;

    public class UnitData
    {
        public Cell unitCellPosition;
        public UnitBehaviour unit;
    }

    public LayerMask UnitLayer => unitLayer;

    [Header("Turn System Settings")]
    private Turn currentTurn;
    private TurnList<Turn> turns;
    private TurnList<Turn> subTurns;

    [SerializeField] private Color highlightColor;

    public TurnList<Turn> Turns => turns;
    public TurnList<Turn> SubTurns => subTurns;
    public Turn CurrentTurn => currentTurn;

    public Color HighLightColor => highlightColor;

    public static event EventHandler<Turn> OnTurnBegan;
    public static event EventHandler<Turn> OnTurnEnded;

    public static event EventHandler<Turn> OnSubTurnBegan;
    public static event EventHandler<Turn> OnSubTurnEnded;

    [Header("Healthbar Settings")]
    [SerializeField] private UnitHPBarHandler hpBarHandler;

    private void Start()
    {
        turns = new TurnList<Turn>();
        subTurns = new TurnList<Turn>();

        //Grid Creation
        gridLogic = new GridLogic<GameObject>(settings);
        gridLogic.CreateGrid(OnSpawnCell);

        //Team Creation
        waveIndex = 0;
        List<Unit> playerTeamData = GameManager.PlayerTeam[0];
        List<Unit> enemyTeamData = GameManager.EnemyWaves[waveIndex];

        allTeam = new List<UnitData>();

        playerTeam = new List<UnitData>();
        CreateTeam(playerTeamData, 0, 1, ref playerTeam, Vector3.forward);

        enemyTeam = new List<UnitData>();
        CreateTeam(enemyTeamData, settings.Size.y - 1, 0, ref enemyTeam, Vector3.back);

        void CreateTeam(List<Unit> teamData, int yOffset, int xOffset, ref List<UnitData> team, Vector3 faceDir)
        {
            for (int i = 0; i < teamData.Count; i++)
            {
                GameObject unit = teamData[i].CreateUnit(ref turns, ref subTurns, ref gridLogic.gridCells);
                Cell cell = gridLogic.GetCellFromGrid((i * 3) + xOffset, yOffset);
                unit.transform.position = gridLogic.gridCells[cell].transform.position + (Vector3.up * instantiateOffset);
                unit.transform.forward = faceDir;

                hpBarHandler.CreateHealthBar(unit.GetComponent<UnitBehaviour>());

                UnitData unitData = new UnitData()
                {
                    unitCellPosition = cell,
                    unit = unit.GetComponent<UnitBehaviour>(),
                };

                team.Add(unitData);
                allTeam.Add(unitData);
            }
        }

        //Turn System
        UnitBehaviour.OnAnyUnitDead += UnitBehaviour_OnUnitDead;

        for (int i = 0; i < playerTeam.Count; i++)
        {
            Turn turn = new Turn(
                    actionValue: playerTeam[i].unit.Stats.Speed,
                    turnObject: playerTeam[i].unit.gameObject
                );

            turns.Add(turn);
        }

        for (int i = 0; i < enemyTeam.Count; i++)
        {
            Turn turn = new Turn(
                    actionValue: enemyTeam[i].unit.Stats.Speed,
                    turnObject: enemyTeam[i].unit.gameObject
                );

            turns.Add(turn);
        }

        turns.SortDescending();

        StartCoroutine(GameTurns());

        OnLevelInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void UnitBehaviour_OnUnitDead(object sender, Cell cell)
    {
        UnitBehaviour unitBehaviour = (UnitBehaviour)sender;
        TryRemoveUnit(ref playerTeam, unitBehaviour);
        TryRemoveUnit(ref enemyTeam, unitBehaviour);
        TryRemoveUnit(ref allTeam, unitBehaviour);

        for (int i = 0; i < turns.Count; i++)
        {
            if (turns[i].TurnObject == unitBehaviour.gameObject)
            {
                turns.RemoveAt(i);
            }
        }

        void TryRemoveUnit(ref List<UnitData> team, UnitBehaviour unit)
        {
            for (int i = 0; i < team.Count; i++)
            {
                if (team[i].unit == unit)
                {
                    team.RemoveAt(i);
                }
            }
        }
    }

    private IEnumerator GameTurns()
    {
        while (turns.Count > 0)
        {
            while (subTurns.Count > 0)
            {
                UnitBehaviour unit = subTurns[0].TurnObject.GetComponent<UnitBehaviour>();

                OnSubTurnBegan?.Invoke(this, subTurns[0]);

                currentTurn = subTurns[0];
                yield return StartCoroutine(unit.TriggerSubTurn());

                yield return new WaitForSeconds(1f);

                OnSubTurnEnded?.Invoke(this, subTurns[0]);
                subTurns.RemoveAt(0);
            }

            yield return new WaitForSeconds(0.5f);

            UnitBehaviour unitBehaviour = turns[0].TurnObject.GetComponent<UnitBehaviour>();

            OnTurnBegan?.Invoke(this, turns[0]);

            currentTurn = turns[0];
            yield return StartCoroutine(unitBehaviour.TurnAction());

            if (unitBehaviour.Stats.Health > 0f)
            {
                turns.Add(new Turn(turns[0]));
            }

            yield return new WaitForSeconds(0.5f);

            OnTurnEnded?.Invoke(this, turns[0]);

            turns.RemoveAt(0);
        }

        //Game over
    }

    private void OnSpawnCell(Cell cell, Vector2 position)
    {
        GameObject node = Instantiate(nodePrefab, transform);
        node.GetComponent<Node>().Initialize(cell);
        node.transform.position = new Vector3(position.x, transform.position.y, position.y);
        gridLogic.gridCells.Add(cell, node);
    }
}

public static class UnitDataExtensionMethods
{
    public static Cell GetUnitCellPosition(this List<LevelController.UnitData> unitDatas, UnitBehaviour unitBehaviour)
    {
        for (int i = 0; i < unitDatas.Count; i++)
        {
            if (unitDatas[i].unit == unitBehaviour)
            {
                return unitDatas[i].unitCellPosition;
            }
        }

        return default(Cell);
    }
}
