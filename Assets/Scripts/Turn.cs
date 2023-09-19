using UnityEngine;

[System.Serializable]
public class Turn
{
    private int actionValue;
    private GameObject turnObject;

    public int ActionValue => actionValue;
    public GameObject TurnObject => turnObject;

    public Turn(int actionValue, GameObject turnObject)
    {
        this.actionValue = actionValue;
        this.turnObject = turnObject;
    }

    public Turn(Turn turn)
    {
        this.actionValue = turn.actionValue;
        this.turnObject = turn.turnObject;
    }
}