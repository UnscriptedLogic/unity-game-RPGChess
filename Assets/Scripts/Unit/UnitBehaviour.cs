using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour
{
    public bool isPlayerControlled;
    
    protected Unit.Stats stats;
    protected bool inputGiven;

    public Unit.Stats Stats => stats;

    public void Initialize(Unit.Stats baseStats)
    {
        stats = baseStats;
    }

    public IEnumerator TurnAction()
    {
        OnTurnBegin();

        yield return new WaitUntil(() => inputGiven);
        inputGiven = false;

        OnTurnEnd();
    }

    protected virtual void OnTurnBegin() { }
    protected virtual void OnTurnEnd() { }
}
