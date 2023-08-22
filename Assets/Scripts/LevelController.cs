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
                GameObject unit = Instantiate(teamData[i].ObjectPrefab);
                unit.transform.position = gridLogic.gridCells[gridLogic.GetCellFromGrid((i * 3) + xOffset, yOffset)].transform.position + (Vector3.up * instantiateOffset);
                unit.transform.forward = faceDir;
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
