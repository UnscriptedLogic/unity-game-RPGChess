using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "ScriptableObjects/Create New Unit")]
public class Unit : ScriptableObject
{
    [System.Serializable]
    public class Stats
    {
        [SerializeField] private int health;
        [SerializeField] private int damage;
        [SerializeField] private int speed;

        public int Health => health;
        public int Damage => damage;
        public int Speed => speed;

        public Stats(int health = 0, int damage = 0, int speed = 0)
        {
            this.health = health;
            this.damage = damage;
            this.speed = speed;
        }

        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
                (
                    a.Health + b.Health,
                    a.Damage + b.Damage,
                    a.Speed + b.Speed
                );
        }

        public static Stats operator -(Stats a, Stats b)
        {
            return new Stats
                (
                    a.Health - b.Health,
                    a.Damage - b.Damage,
                    a.Speed - b.Speed
                );
        }
    }

    [SerializeField] private Stats baseStats;
    [SerializeField] private GameObject objectPrefab;

    public Stats BaseStats => baseStats;
    public GameObject ObjectPrefab => objectPrefab;

    public GameObject CreateUnit()
    {
        GameObject unitObject = Instantiate(objectPrefab);
        unitObject.GetComponent<UnitBehaviour>().Initialize(baseStats);
        return unitObject;
    }
}