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

    private List<UnitBehaviour> playerTeam;
    private List<UnitBehaviour> enemyTeam;

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

        playerTeam = new List<UnitBehaviour>();
        CreateTeam(playerTeamData, 0, 1, ref playerTeam, Vector3.forward);

        enemyTeam = new List<UnitBehaviour>();
        CreateTeam(enemyTeamData, settings.Size.y - 1, 0, ref enemyTeam, Vector3.back);

        void CreateTeam(List<Unit> teamData, int yOffset, int xOffset, ref List<UnitBehaviour> team, Vector3 faceDir)
        {
            for (int i = 0; i < teamData.Count; i++)
            {
                GameObject unit = teamData[i].CreateUnit();
                unit.transform.position = gridLogic.gridCells[gridLogic.GetCellFromGrid((i * 3) + xOffset, yOffset)].transform.position + (Vector3.up * instantiateOffset);
                unit.transform.forward = faceDir;
                team.Add(unit.GetComponent<UnitBehaviour>());
            }
        }

        //Turn System
        turns = new List<Turn>();
        for (int i = 0; i < playerTeam.Count; i++)
        {
            turns.Add(new Turn(
                    actionValue: playerTeam[i].Stats.Speed,
                    turnObject: playerTeam[i].gameObject
                ));
        }

        for (int i = 0; i < enemyTeam.Count; i++)
        {
            turns.Add(new Turn(
                    actionValue: enemyTeam[i].Stats.Speed,
                    turnObject: enemyTeam[i].gameObject
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
    }

    private void OnSpawnCell(Cell cell, Vector2 position)
    {
        GameObject node = Instantiate(nodePrefab, transform);
        node.transform.position = new Vector3(position.x, transform.position.y, position.y);
        gridLogic.gridCells.Add(cell, node);
    }
}
