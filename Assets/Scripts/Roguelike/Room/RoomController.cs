using System;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomController : MonoBehaviour 
{
    [SerializeField] private RoomTrigger trigger;
    [SerializeField] private EnemyTracker tracker;
    [SerializeField] private RoomPopulator populator;
    [SerializeField] private DoorController[] doors;



    public RoomState State { get; private set; } = RoomState.Unvisited;
    public RoomDefinition Definition; //{ get; private set; }

    public event Action<RoomController> RoomCleared;


    private void Start()
    {
        Setup(Definition);
    }



    public void Setup(RoomDefinition definition)
    {
        Definition = definition;

        if (doors != null)
        {
            foreach (var door in doors)
            {
                if (door != null)
                    door.Setup(this);
            }
        }
    }

    private void OnEnable()
    {
        if (trigger != null) trigger.PlayerEntered += OnPlayerEntered;
        if (tracker != null) tracker.AllEnemiesDead += OnAllEnemiesDead;
    }

    private void OnDisable()
    {
        if (trigger != null) trigger.PlayerEntered -= OnPlayerEntered;
        if (tracker != null) tracker.AllEnemiesDead -= OnAllEnemiesDead;
    }


    private void OnPlayerEntered()
    {
        if (State != RoomState.Unvisited) return;

        StartPopulating();
    }


    private void OnAllEnemiesDead()
    {
        if (State != RoomState.Active) return;

        foreach (var door in doors)
        {
            door.ForceOpen();
        }
        RoguelikeManager.Instance.IncreaseDepth();
        MarkCleared();
    }



    private void StartPopulating()
    {
        State = RoomState.Populating;

        if (Definition == null)
        {
            Debug.Log("Хуево все");
            State = RoomState.Cleared;
            OpenAllDoors();
            RoomCleared?.Invoke(this);
            return;
        }

        int depth = RoguelikeManager.Instance != null
            ? RoguelikeManager.Instance.CurrentDepth
            : 1;

        var rng = RoguelikeManager.Instance != null
            ? RoguelikeManager.Instance.Rng
            : new System.Random();


        bool spawned = populator.Populate(Definition, depth, rng);


        if (!spawned)
        {
            Debug.Log("net niche");
            State = RoomState.Cleared;
            OpenAllDoors();
            RoomCleared?.Invoke(this);
            return;
        }

        CloseAllDoors();
        State = RoomState.Active;
    }

    private void MarkCleared()
    {
        State = RoomState.Cleared;

        OpenAllDoors();

        RoomCleared?.Invoke(this);
    }


    private void OpenAllDoors()
    {
        if (doors == null) return;

        foreach (var door in doors)
        {
            if (door != null)
                //door.ForceOpen();
                continue;       
        }
    }


    private void CloseAllDoors()
    {
        if (doors == null) return;

        foreach (var door in doors)
        {
            if (door != null)
                door.Close();
        }
    }
}
