using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHPBarHandler : MonoBehaviour
{
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Vector3 offsetPosition;

    public void CreateHealthBar(UnitBehaviour unitBehaviour)
    {
        GameObject healthbar = Instantiate(healthbarPrefab, unitBehaviour.transform);
        healthbar.transform.position = unitBehaviour.transform.position + offsetPosition;
        
        healthbar.GetComponent<HealthBarUI>().Initialize(new HealthBarUI.InitSettings(unitBehaviour));
    }
}
