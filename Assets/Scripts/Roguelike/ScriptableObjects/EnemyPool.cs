using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPool", menuName = "Scriptable Objects/EnemyPool")]
public class EnemyPool : ScriptableObject
{
    public List<EnemyDefinition> Enemies = new();
}
