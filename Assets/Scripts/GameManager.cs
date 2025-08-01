using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void AllTowersOn();
    public static AllTowersOn onAllTowersOn;

    public delegate void PathCompleted();
    public static PathCompleted onPathCompleted;

    public delegate void LevelRestarted();
    public static LevelRestarted onLevelRestarted;

    public delegate void StartPathTracing();
    public static StartPathTracing onStartPathTracing;

    public delegate void FinishPathTracing();
    public static FinishPathTracing onFinishPathTracing;

    [Header("Completion Check")]
    public GameObject[] towersArray;
    static int towersArrayLength;

    public enum LoopQuality
    {
        Good,
        Okay,
        Bad,
        Amazing,
    }

    LoopQuality playerLoopQuality;

    private void Awake()
    {
        towersArrayLength = towersArray.Length;
    }

    public void SetPlayerLoopQuality(LoopQuality quality)
    {
        playerLoopQuality = quality;
    }

    public LoopQuality GetPlayerLoopQuality()
    {
        return playerLoopQuality;
    }

    public static int GetTowersArrayLength()
    {
        return towersArrayLength;
    }
}
