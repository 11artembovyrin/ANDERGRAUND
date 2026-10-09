using UnityEngine;

public enum RoomState
{
    Unvisited,    // игрок ещё не входил
    Populating,   // идёт заселение (короткая фаза)
    Active,       // бой идёт, двери закрыты
    Cleared       // все враги мертвы, двери открыты
}
