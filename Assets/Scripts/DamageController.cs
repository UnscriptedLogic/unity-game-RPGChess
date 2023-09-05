using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public static DamageController instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void DealDamageToUnit(UnitBehaviour dealer, UnitBehaviour receiver)
    {

    }
}
