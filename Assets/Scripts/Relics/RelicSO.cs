using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality
{
    OneStar,
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar
}

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Create New Item")]
public class RelicSO : ScriptableObject
{
    private int maxLevel = 10;

    [SerializeField][Range(1, 10)] private int level;
    [SerializeField] private Sprite icon;
    [SerializeField] private Quality quality;
    [SerializeField][TextArea(2, 10)] private string description;

    [SerializeField] private Unit.Stats minLevelStats;
    [SerializeField] private Unit.Stats maxLevelStats;

    public Sprite Icon => icon;
    public string Description => description;
    
    public Unit.Stats CurrentStats
    {
        get
        {
            float percent = (maxLevel + 1) / 100f * (level - 1);
            int health = Mathf.RoundToInt(((maxLevelStats.InitialHealth - minLevelStats.InitialHealth) * percent) + minLevelStats.InitialHealth);
            int damage = Mathf.RoundToInt(((maxLevelStats.InitialDamage - minLevelStats.InitialDamage) * percent) + minLevelStats.InitialDamage);
            int speed = Mathf.RoundToInt(((maxLevelStats.InitialSpeed - minLevelStats.InitialSpeed) * percent) + minLevelStats.InitialSpeed);
            Unit.Stats currentStats = new Unit.Stats(health, damage, speed);
            return currentStats;
        }
    }
}
