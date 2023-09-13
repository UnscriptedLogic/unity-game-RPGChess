using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Create New Item")]
public class RelicSO : ScriptableObject
{
    [SerializeField] private Unit.Stats statsModifier;
    [SerializeField] private Sprite icon;
    [SerializeField][TextArea(2, 10)] private string description;

    public Unit.Stats StatsModifier => statsModifier;
    public Sprite Icon => icon;
    public string Description => description;
}
