using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    private readonly Dictionary<GameObject, ObjectPool<Enemy>> _pools = new();
    private readonly Dictionary<Enemy, GameObject> _instanceToPrefab = new();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Enemy SpawnEnemy(EnemyDefinition definition, SpawnPoint spawnPoint)
    {
        var pool = GetOrCreateNewPool(definition.prefab);
        var enemy = pool.Get();

        enemy.transform.SetPositionAndRotation(
            spawnPoint.transform.position,
            spawnPoint.transform.rotation);

        enemy.gameObject.SetActive(true);

        _instanceToPrefab[enemy] = definition.prefab;
        enemy.Died += OnEnemyDied;

        return enemy;
    }

    public void Despawn(Enemy enemy)
    {
        if (enemy == null) return;


        if (_instanceToPrefab.TryGetValue(enemy, out var prefab))
        {
            _instanceToPrefab.Remove(enemy);
            GetOrCreateNewPool(prefab).Release(enemy);
        }
    }


    private ObjectPool<Enemy> GetOrCreateNewPool(GameObject prefab)
    {
        if(_pools.TryGetValue(prefab, out var pool)) 
        { 
            return pool; 
        }

        pool = new ObjectPool<Enemy>(
            createFunc: () =>
            {
                var go = Instantiate(prefab, transform);
                return go.GetComponent<Enemy>();
            },
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            defaultCapacity: 8,
            maxSize: 64
            );

        _pools[prefab] = pool;
        return pool;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        Despawn(enemy);
    }
}
