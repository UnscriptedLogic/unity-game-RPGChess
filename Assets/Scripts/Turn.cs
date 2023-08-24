using UnityEngine;

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
}