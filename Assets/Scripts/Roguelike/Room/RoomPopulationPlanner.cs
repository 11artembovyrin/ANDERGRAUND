using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public static class RoomPopulationPlanner
{
    public static List<SpawnEntry> CreatePopulationPlan(
        IReadOnlyList<SpawnPoint> spawnPoints,
        IReadOnlyList<EnemyDefinition> enemyPool,
        int budget,
        int roomDifficulty,
        System.Random rng)
    {
        var result = new List<SpawnEntry>();
        var freePoints = new List<SpawnPoint>(spawnPoints);
        var perEnemyCount = new Dictionary<EnemyDefinition, int>();
        var excluded = new HashSet<EnemyDefinition>();

        

        while (freePoints.Count > 0 && budget > 0)
        {
            var candidates = new List<EnemyDefinition>();

            foreach (var enemy in enemyPool)
            {
                if (enemy == null) continue;
                if (excluded.Contains(enemy)) continue;
                if (enemy.cost > budget) continue;
                if (enemy.minDifficulty > roomDifficulty) continue;

                int count = perEnemyCount.GetValueOrDefault(enemy, 0);
                if (count >= enemy.maxPerRoom) continue;

                candidates.Add(enemy);
            }


            if (candidates.Count == 0) break;

            var chosen = PickWeighted(candidates, budget, perEnemyCount, rng);


            var matchingSlots = new List<SpawnPoint>();
            for (int i = 0; i < freePoints.Count; i++)
            {
                if ((freePoints[i].type & chosen.allowedSpawnSlots) != 0)
                    matchingSlots.Add(freePoints[i]);
            }

            if (matchingSlots.Count == 0)
            {
                excluded.Add(chosen);
                continue;
            }

            var slot = matchingSlots[rng.Next(matchingSlots.Count)];


            freePoints.Remove(slot);
            budget -= chosen.cost;
            perEnemyCount[chosen] = perEnemyCount.GetValueOrDefault(chosen, 0) + 1;
            result.Add(new SpawnEntry(chosen, slot));
        }
        

        return result;
    }

    private static EnemyDefinition PickWeighted(
        List<EnemyDefinition> candidates, int remainingBudget, Dictionary<EnemyDefinition, int> perEnemyCount, System.Random rng)
    {
        float repeatPenality = 0.5f; // sam vvodi

        float total = 0f;
        var weights = new float[candidates.Count];

        for(int i = 0; i < candidates.Count; i++)
        {
            var enemy = candidates[i];
            float weight = enemy.weight;

            //float ratio = (float)enemy.cost / Mathf.Max(1, remainingBudget); похуй, не работает
            //weight *= Mathf.Lerp(1, 1 - enemy.WeightBiasToCheap, ratio);

            int count = perEnemyCount.GetValueOrDefault(enemy, 0);
            weight *= 1f / (1f + count * repeatPenality);

            weights[i] = Mathf.Max(0.0001f, weight);
            total += weights[i];
        }

        float roll = (float)rng.NextDouble() * total;

        for (int i = 0; i < candidates.Count; i++)
        {
            roll -= weights[i];
            if (roll <= 0f) return candidates[i];
        }

        return candidates[candidates.Count - 1];
    }
}
