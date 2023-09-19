using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestData : MonoBehaviour
{
    [SerializeField] private List<Unit> playerTeam;
    [SerializeField] private List<Unit> enemyTeam;

    private void Awake()
    {
        GameManager.SetPlayerWave(new List<List<Unit>>()
        {
            playerTeam
        });

        GameManager.SetEnemyWave(new List<List<Unit>>()
        {
            enemyTeam
        });
    }
}
