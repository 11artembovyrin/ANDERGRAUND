using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyTracker : MonoBehaviour 
{
    private readonly List<Enemy> _alive = new();
    public event Action AllEnemiesDead;

    public int AliveCount => _alive.Count;


    public void Register(Enemy enemy)
    {
        if (enemy == null) return;
        if (_alive.Contains(enemy)) return;

        _alive.Add(enemy);
        enemy.Died += OnEnemyDied;
    }


    private void OnEnemyDied(Enemy enemy)
    {
        enemy.Died -= OnEnemyDied;
        _alive.Remove(enemy);

        if (_alive.Count == 0)
            AllEnemiesDead?.Invoke();
    }


    public void Clear()
    {
        foreach (var enemy in _alive)
            if (enemy != null) enemy.Died -= OnEnemyDied;
        _alive.Clear();
    }

    private void OnDisable()
    {
        Clear();
    }

}
