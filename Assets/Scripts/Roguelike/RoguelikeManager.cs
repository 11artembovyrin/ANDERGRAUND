using System;
using UnityEngine;

public class RoguelikeManager : MonoBehaviour 
{
    public static RoguelikeManager Instance { get; private set; }


    [Header("Run Settings")]
    [SerializeField] private int defaultSeed = 12345;
    [SerializeField] private bool randomSeedEachRun = true;
    [SerializeField] private int startDepth;


    public int CurrentDepth { get; private set; }
    public int DepthIncrease = 1;
    public int Seed {  get; private set; }
    public System.Random Rng { get; private set; }
    public bool IsRunActive { get; private set; }


    public event Action<int> RunStarted;
    public event Action<int> DepthChanged;
    public event Action<bool> RunEnded;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentDepth = startDepth;
    }

    private void Start()
    {
        StartNewRun();
    }


    public void StartNewRun(int? seedOverride = null)
    {
        if (seedOverride.HasValue)
        {
            Seed = seedOverride.Value;
        }
        else if (randomSeedEachRun)
        {
            Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        else
        {
            Seed = defaultSeed;
        }

        Rng = new System.Random(Seed);

        CurrentDepth = startDepth;
        IsRunActive = true;

        Debug.Log($"[RoguelikeManager] Начало забега. Сид: {Seed}");


        RunStarted?.Invoke(Seed);
        DepthChanged?.Invoke(CurrentDepth);
    }
    


    public void IncreaseDepth()
    {
        if (!IsRunActive) return;
        CurrentDepth += DepthIncrease;
        Debug.Log($"[RoguelikeManager] Глубина: {CurrentDepth}");
        DepthChanged?.Invoke(CurrentDepth);
    }


    public void EndRun(bool win)
    {
        if (!IsRunActive) return;
        IsRunActive = false;
        Debug.Log($"[RoguelikeManager] Забег завершён. Победа: {win}");
        RunEnded?.Invoke(win);
    }


    public void ResetRun()
    {
        IsRunActive = false;
        CurrentDepth = 1;
        Rng = null;
    }

}
