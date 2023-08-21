using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedLogic.Experimental.Generation;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GridSettings settings;

    private GridLogic<GameObject> gridLogic;

    private void Start()
    {
        gridLogic = new GridLogic<GameObject>(settings);
        gridLogic.CreateGrid(OnSpawnCell);

        Destroy(gridLogic.gridCells[gridLogic.GetCellFromGrid(4, 0)]);
        Destroy(gridLogic.gridCells[gridLogic.GetCellFromGrid(3, settings.Size.y - 1)]);
    }

    private void OnSpawnCell(Cell cell, Vector2 position)
    {
        GameObject node = Instantiate(nodePrefab, transform);
        node.transform.position = new Vector3(position.x, transform.position.y, position.y);
        gridLogic.gridCells.Add(cell, node);
    }
}
