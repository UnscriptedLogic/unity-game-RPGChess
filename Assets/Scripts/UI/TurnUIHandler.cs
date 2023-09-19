using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUIHandler : LevelComponent
{
    [SerializeField] private Transform currentTurnAnchor;

    [Header("Turn Settings")]
    [SerializeField] private TurnUI turnUIPrefab;
    [SerializeField] private GameObject emptyTurnAnchorPrefab;
    [SerializeField] private Transform turnUIParent;
    [SerializeField] private Transform turnObjectParent;

    [Header("Subturn Settings")]
    [SerializeField] private TurnUI subTurnUIPrefab;
    [SerializeField] private GameObject emptySubTurnAnchorPrefab;
    [SerializeField] private Transform subTurnUIParent;
    [SerializeField] private Transform subTurnObjectParent;

    private TurnUI currentTurnUIObject;

    private List<Transform> turnAnchors;
    private Dictionary<Turn, TurnUI> turnUIs;

    private List<Transform> subTurnAnchors;
    private Dictionary<Turn, TurnUI> subTurnUIs;

    protected override void OnControllerInitialized()
    {
        turnUIs = new Dictionary<Turn, TurnUI>();
        subTurnUIs = new Dictionary<Turn, TurnUI>();

        TurnList<Turn> turns = level.Turns;
        TurnList<Turn> subTurns = level.SubTurns;
        
        turnAnchors = new List<Transform>();
        subTurnAnchors = new List<Transform>();

        for (int i = 0; i < turns.Count; i++)
        {
            CreateTurnObject(turns[i], turnUIParent, turnObjectParent);
        }

        turns.OnCreated += Turns_OnCreated;
        turns.OnRemoved += Turns_OnRemoved;

        subTurns.OnCreated += SubTurns_OnCreated;
        subTurns.OnRemoved += SubTurns_OnRemoved;

        LevelController.OnTurnBegan += LevelController_OnTurnBegan;
        LevelController.OnTurnEnded += LevelController_OnTurnEnded;

        LevelController.OnSubTurnBegan += LevelController_OnSubTurnBegan;
        LevelController.OnSubTurnEnded += LevelController_OnTurnEnded;
    }

    private void LevelController_OnSubTurnBegan(object sender, Turn subTurn)
    {
        //Remove first turn item
        RemoveSubturnObject(subTurn);

        //Add current turn item to the spotlight turn
        currentTurnUIObject = Instantiate(turnUIPrefab, currentTurnAnchor);

        currentTurnUIObject.SetActionValue(subTurn.ActionValue);
        currentTurnUIObject.SetTurnName(subTurn.TurnObject.name);
        currentTurnUIObject.SetAnchor(currentTurnAnchor.transform);
    }

    private void LevelController_OnTurnBegan(object sender, Turn turn)
    {
        //Remove first turn item
        RemoveTurnObject(turn);

        //Add current turn item to the spotlight turn
        currentTurnUIObject = Instantiate(turnUIPrefab, currentTurnAnchor);

        currentTurnUIObject.SetActionValue(turn.ActionValue);
        currentTurnUIObject.SetTurnName(turn.TurnObject.name);
        currentTurnUIObject.SetAnchor(currentTurnAnchor.transform);
    }

    private void LevelController_OnTurnEnded(object sender, Turn turn)
    {
        Destroy(currentTurnUIObject.gameObject);
    }

    private void SubTurns_OnCreated(Turn subTurn)
    {
        CreateSubTurnObject(subTurn, subTurnUIParent, subTurnObjectParent);
    }

    private void SubTurns_OnRemoved(Turn subTurn)
    {
        if (level.CurrentTurn == subTurn) return;

        RemoveSubturnObject(subTurn);
    }

    private void Turns_OnCreated(Turn turn)
    {
        CreateTurnObject(turn, turnUIParent, turnObjectParent);
    }

    private void Turns_OnRemoved(Turn turn)
    {
        if (level.CurrentTurn == turn) return;

        RemoveTurnObject(turn);
    }

    private void RemoveTurnObject(Turn turn)
    {
        if (!turnUIs.ContainsKey(turn)) return;

        TurnUI turnUI = turnUIs[turn];
        Transform turnAnchor = turnUI.Anchor;

        turnAnchors.Remove(turnAnchor);

        Destroy(turnUI.gameObject);
        Destroy(turnAnchor.gameObject);

        LayoutRebuilder.ForceRebuildLayoutImmediate(turnUIParent.GetComponent<RectTransform>());
    }

    private void RemoveSubturnObject(Turn turn)
    {
        if (!subTurnUIs.ContainsKey(turn)) return;

        TurnUI turnUI = subTurnUIs[turn];
        Transform turnAnchor = turnUI.Anchor;

        subTurnAnchors.Remove(turnAnchor);

        Destroy(turnUI.gameObject);
        Destroy(turnAnchor.gameObject);

        LayoutRebuilder.ForceRebuildLayoutImmediate(subTurnUIParent.GetComponent<RectTransform>());
    }

    private void CreateTurnObject(Turn turn, Transform anchorParent, Transform objectParent)
    {
        GameObject turnAnchor = Instantiate(emptyTurnAnchorPrefab, anchorParent);
        TurnUI turnUI = Instantiate(turnUIPrefab, objectParent);

        LayoutRebuilder.ForceRebuildLayoutImmediate(anchorParent.GetComponent<RectTransform>());

        turnUI.SetActionValue(turn.ActionValue);
        turnUI.SetTurnName(turn.TurnObject.name);
        turnUI.SetAnchor(turnAnchor.transform);

        turnUIs.Add(turn, turnUI);
        turnAnchors.Add(turnAnchor.transform);
    }

    private void CreateSubTurnObject(Turn turn, Transform anchorParent, Transform objectParent)
    {
        GameObject turnAnchor = Instantiate(emptySubTurnAnchorPrefab, anchorParent);
        TurnUI turnUI = Instantiate(subTurnUIPrefab, objectParent);

        LayoutRebuilder.ForceRebuildLayoutImmediate(anchorParent.GetComponent<RectTransform>());

        turnUI.SetTurnName(turn.TurnObject.name);
        turnUI.SetAnchor(turnAnchor.transform);

        subTurnUIs.Add(turn, turnUI);
        subTurnAnchors.Add(turnAnchor.transform);
    }
}
