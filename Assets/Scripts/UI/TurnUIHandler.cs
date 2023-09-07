using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUIHandler : LevelComponent
{
    [SerializeField] private GameObject emptyTurnAnchorPrefab;
    [SerializeField] private TurnUI turnUIPrefab;
    [SerializeField] private Transform turnUIParent;
    [SerializeField] private Transform turnObjectParent;

    private TurnList<Turn> turns;
    private List<Transform> turnAnchors;
    private Dictionary<Turn, TurnUI> turnUIs;

    protected override void OnControllerInitialized()
    {
        turnUIs = new Dictionary<Turn, TurnUI>();
        turns = level.Turns;
        turnAnchors = new List<Transform>();

        for (int i = 0; i < turns.Count; i++)
        {
            CreateTurnObject(turns[i]);
        }

        turns.OnCreated += Turns_OnCreated;
        turns.OnRemoved += Turns_OnRemoved;
    }

    private void Turns_OnCreated(Turn turn)
    {
        CreateTurnObject(turn);
    }

    private void Turns_OnRemoved(Turn turn)
    {
        TurnUI turnUI = turnUIs[turn];
        Transform turnAnchor = turnUI.Anchor;
        
        turns.Remove(turn);
        turnAnchors.Remove(turnAnchor);

        Destroy(turnUI.gameObject);
        Destroy(turnAnchor.gameObject);
    }

    private void CreateTurnObject(Turn turn)
    {
        GameObject turnAnchor = Instantiate(emptyTurnAnchorPrefab, turnUIParent);
        TurnUI turnUI = Instantiate(turnUIPrefab, turnObjectParent);

        LayoutRebuilder.ForceRebuildLayoutImmediate(turnUIParent.GetComponent<RectTransform>());

        turnUI.SetActionValue(turn.ActionValue);
        turnUI.SetTurnName(turn.TurnObject.name);
        turnUI.SetAnchor(turnAnchor.transform);

        turnUIs.Add(turn, turnUI);
        turnAnchors.Add(turnAnchor.transform);
    }
}
