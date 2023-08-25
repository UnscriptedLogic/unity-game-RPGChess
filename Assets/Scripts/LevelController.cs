using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class LevelController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GridSettings settings;

    private GridLogic<GameObject> gridLogic;

    [Header("Entity Settings")]
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

    [Header("Turn System Settings")]
    private List<Turn> turns;

    private void Start()
    {
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
                GameObject unit = teamData[i].CreateUnit();
                Cell cell = gridLogic.GetCellFromGrid((i * 3) + xOffset, yOffset);
                unit.transform.position = gridLogic.gridCells[cell].transform.position + (Vector3.up * instantiateOffset);
                unit.transform.forward = faceDir;

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
        turns = new List<Turn>();
        for (int i = 0; i < playerTeam.Count; i++)
        {
            turns.Add(new Turn(
                    actionValue: playerTeam[i].unit.Stats.Speed,
                    turnObject: playerTeam[i].unit.gameObject
                ));
        }

        for (int i = 0; i < enemyTeam.Count; i++)
        {
            turns.Add(new Turn(
                    actionValue: enemyTeam[i].unit.Stats.Speed,
                    turnObject: enemyTeam[i].unit.gameObject
                ));
        }

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

        StartCoroutine(GameTurns());
    }

    private IEnumerator GameTurns()
    {
        for (int i = 0; i < turns.Count; i++)
        {
            UnitBehaviour unitBehaviour = turns[i].TurnObject.GetComponent<UnitBehaviour>();
            List<List<Cell>> cellSet = unitBehaviour.GetPossibleMovementTiles(allTeam.GetUnitCellPosition(unitBehaviour), settings, gridLogic.gridCells);

            for (int x = 0; x < cellSet.Count; x++)
            {
                for (int y = 0; y < cellSet[x].Count; y++)
                {
                    gridLogic.gridCells[cellSet[x][y]].GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                }
            }

            yield return StartCoroutine(unitBehaviour.TurnAction());

            for (int x = 0; x < cellSet.Count; x++)
            {
                for (int y = 0; y < cellSet[x].Count; y++)
                {
                    gridLogic.gridCells[cellSet[x][y]].GetComponentInChildren<MeshRenderer>().material.color = Color.grey;
                }
            }
        }
    }

    private void OnSpawnCell(Cell cell, Vector2 position)
    {
        GameObject node = Instantiate(nodePrefab, transform);
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
