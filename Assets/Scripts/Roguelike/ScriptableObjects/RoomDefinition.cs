using UnityEngine;

[CreateAssetMenu(fileName = "RoomDefenition", menuName = "Scriptable Objects/RoomDefenition")]
public class RoomDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;
    [Min(0f)] public float selectionWeight = 1f;


    [Header("Prefab")]
    public GameObject prefab;


    [Header("Budget")]
    public int baseBudget = 5;
    public int budgetPerDepth = 1;


    [Header("Enemies")]
    public EnemyPool pool;

    public int GetBudgetForDepth(int depth)
    {
        return baseBudget + depth * budgetPerDepth;
    }
}