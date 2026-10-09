using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SpawnSlotType type = SpawnSlotType.ground;
    public bool occupied {  get; private set; }
    public void Free() => occupied = false;
    public void Occupy() => occupied = true;



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = type switch
        {
            SpawnSlotType.ground => Color.green,
            SpawnSlotType.air => Color.cyan,
            SpawnSlotType.ceiling => Color.magenta,
            _ => Color.white
        };
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawRay(transform.position, transform.right * 1f);
    }
#endif
}
