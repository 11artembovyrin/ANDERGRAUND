using System.Collections.Generic;
using UnityEngine;

public class RoomLayout : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spawnPoints;
    public IReadOnlyList<SpawnPoint> Points => spawnPoints;

    public void FreeAllPoints()
    {
        if (spawnPoints == null) return;
        foreach (SpawnPoint point in spawnPoints)
        {
            if (point != null) point.Free();
        }
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
    }
#endif
}
