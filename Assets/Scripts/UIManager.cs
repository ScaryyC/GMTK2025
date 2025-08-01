using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI interactText;
    public TextMeshProUGUI topLeftText;
    public TextMeshProUGUI bigText;

    public string badMessage = "Not even close...";
    public string okayMessage = "Close enough";
    public string goodMessage = "Great job!";
    public string amazingMessage = "Right on the dot!";

    private void Awake()
    {
        if (interactText != null)
        {
            interactText.enabled = false;
        }

        if (bigText != null)
        {
            bigText.enabled = false;
        }

        if (topLeftText != null)
        {
            topLeftText.enabled = false;
        }
    }

    private void OnEnable()
    {
        GameManager.onAllTowersOn += ShowEndableText;
        GameManager.onPathCompleted += ShowResults;
    }

    private void OnDisable()
    {
        GameManager.onAllTowersOn -= ShowEndableText;
        GameManager.onPathCompleted -= ShowResults;
    }

    void ShowEndableText()
    {
        EnableTopLeftText(true);
    }

    void ShowResults()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("GameManager not found");
        }
        EnableTopLeftText(false);
        EnableInteractText(false);
        EnableBigText(true);
        switch (gm.GetPlayerLoopQuality())
        {
            case GameManager.LoopQuality.Bad:
                SetBigText(badMessage);
                break;
            case GameManager.LoopQuality.Okay:
                SetBigText(okayMessage);
                break;
            case GameManager.LoopQuality.Good:
                SetBigText(goodMessage);
                break;
            case GameManager.LoopQuality.Amazing:
                SetBigText(amazingMessage);
                break;
        }
    }

    public void SetInteractText(string message)
    {
        if (interactText == null)
        {
            Debug.Log("interact text is null when setting it");
            return;
        }

        interactText.text = message;
    }

    public void SetBigText(string message)
    {
        if (bigText == null)
        {
            Debug.Log("big text is null when setting it");
            return;
        }

        bigText.text = message;
    }

    public void SetTopLeftText(string message)
    {
        if (topLeftText == null)
        {
            Debug.Log("top left text is null when setting it");
            return;
        }

        topLeftText.text = message;
    }

    public void EnableInteractText(bool state)
    {
        interactText.enabled = state;
    }

    public void EnableBigText(bool state)
    {
        bigText.enabled = state;
    }

    public void EnableTopLeftText(bool state)
    {
        topLeftText.enabled = state;
    }
}
