using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectNumbersHandler : MonoBehaviour
{
    [SerializeField] private EffectUI hitEffectPrefab;
    [SerializeField] private Transform damageNumbersScreen;

    private void Start()
    {
        UnitBehaviour.OnAnyUnitRecievedDamage += UnitBehaviour_OnAnyUnitRecievedDamage;
    } 

    private void UnitBehaviour_OnAnyUnitRecievedDamage(object sender, UnitRecievedDamageArgs args)
    {
        UnitBehaviour unitBehaviour = (UnitBehaviour)sender;
        Vector3 position = Camera.main.WorldToScreenPoint(unitBehaviour.transform.position + Vector3.up);
        EffectUI hitEffect = Instantiate(hitEffectPrefab, position, Quaternion.Euler(Vector3.up * 180f), damageNumbersScreen);
        hitEffect.SetText(args.damage);
    }
}
