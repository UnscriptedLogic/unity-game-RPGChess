using UnityEngine;
using UnscriptedLogic;

[CreateAssetMenu(fileName = "New Unit", menuName = "ScriptableObjects/Create New Unit")]
public class Unit : ScriptableObject
{
    public enum MovementType
    {
        Bishop,
        Knight,
        Rook,
        None
    }

    [System.Serializable]
    public class Stats
    {
        [SerializeField] private int health;
        [SerializeField] private int damage;
        [SerializeField] private int speed;

        private IntHandler healthHandler;
        private IntHandler damageHandler;
        private IntHandler speedHandler;

        public int Health => healthHandler.Value;
        public int Damage => damageHandler.Value;
        public int Speed => speedHandler.Value;

        public IntHandler HealthHandler => healthHandler;
        public IntHandler DamageHandler => damageHandler;
        public IntHandler SpeedHandler => speedHandler;

        public Stats()
        {
            healthHandler = new IntHandler(health);
            damageHandler = new IntHandler(damage);
            speedHandler = new IntHandler(speed);

            healthHandler.SetMinimum(0);
            damageHandler.SetMinimum(0);
            speedHandler.SetMaximum(0);
        }

        public Stats(int health = 0, int damage = 0, int speed = 0)
        {
            healthHandler = new IntHandler(health);
            damageHandler = new IntHandler(damage);
            speedHandler = new IntHandler(speed);

            healthHandler.SetMinimum(0);
            damageHandler.SetMinimum(0);
            speedHandler.SetMaximum(0);
        }

        public Stats(Stats stats)
        {
            healthHandler = new IntHandler(stats.health);
            damageHandler = new IntHandler(stats.damage);
            speedHandler = new IntHandler(stats.speed);

            healthHandler.SetMinimum(0);
            damageHandler.SetMinimum(0);
            speedHandler.SetMaximum(0);
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

    [SerializeField] private MovementType movementType;
    [SerializeField] private Stats baseStats;
    [SerializeField] private GameObject objectPrefab;

    public Stats BaseStats => baseStats;
    public GameObject ObjectPrefab => objectPrefab;
    public MovementType UnitMovementType => movementType;

    public GameObject CreateUnit()
    {
        GameObject unitObject = Instantiate(objectPrefab);
        unitObject.GetComponent<UnitBehaviour>().Initialize(this, baseStats);
        return unitObject;
    }
}