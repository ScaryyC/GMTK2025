using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject creditsScreen; 

    public void ShowCredits()
    {
        if (creditsScreen != null)
        {
            creditsScreen.SetActive(true);
        }
    }

    public void HideCredits()
    {
        if (creditsScreen != null)
        {
            creditsScreen.SetActive(false);
        }
    }
}
