using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefenition", menuName = "Scriptable Objects/EnemyDefenition")]
public class EnemyDefinition : ScriptableObject
{
    public string id;
    public GameObject prefab;
    public int cost = 1;
    public int minDifficulty = 1;
    public int maxPerRoom = 5;
    public float weight = 1;
    [Range(0f, 1f)] public float WeightBiasToCheap = 0.5f;
    public SpawnSlotType allowedSpawnSlots = SpawnSlotType.any;

    
}


[System.Flags]
public enum SpawnSlotType
{
    none = 0,
    ground = 1 << 0,
    air = 1 << 1,
    ceiling = 1 << 2,
    any = ~0
}