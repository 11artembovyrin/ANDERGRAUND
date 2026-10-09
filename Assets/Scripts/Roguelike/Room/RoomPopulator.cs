using UnityEngine;

public class RoomPopulator : MonoBehaviour 
{
    [SerializeField] private RoomLayout layout;
    [SerializeField] private EnemyTracker tracker;
    [SerializeField] private EnemyPool defaultPool;


    public bool Populate(RoomDefinition definition, int depth, System.Random rng)
    {

        if (definition == null)
        {
            Debug.LogError($"[RoomPopulator] Нет RoomDefinition на {name}", this);
            return false;
        }

        layout.FreeAllPoints();

        int budget = definition.GetBudgetForDepth(depth);
        var pool = definition.pool != null ? definition.pool : defaultPool;

        if (pool == null || pool.Enemies.Count == 0)
        {
            Debug.LogWarning($"[RoomPopulator] Пустой пул врагов у {definition.id}", this);
            return false;
        }

        var plan = RoomPopulationPlanner.CreatePopulationPlan(
            layout.Points,
            pool.Enemies,
            budget,
            depth,
            rng);

        if (plan.Count == 0)
        {
            Debug.LogWarning($"[RoomPopulator] План пуст у {definition.id}", this);
            return false;
        }

        foreach (var entry in plan)
        {
            entry.Point.Occupy();
            var enemy = EnemyFactory.Instance.SpawnEnemy(entry.Definition, entry.Point);
            tracker.Register(enemy);
        }

        return true;
    }
}
