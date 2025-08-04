using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public float waitBetweenLevels = 0f;

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

    IEnumerator LoadLevelCoroutine(int sceneNum)
    {
        yield return new WaitForSeconds(waitBetweenLevels);
        if (sceneNum <= SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(sceneNum);
        else
        {
            Debug.Log("Scene level not found. Returning to main");
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator ReloadSceneCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int sceneNum)
    {
        StartCoroutine(LoadLevelCoroutine(sceneNum));
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadLevelCoroutine(nextSceneIndex));
    }

    public void ReloadLevel()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }
}
